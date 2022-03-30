using OGame_FleetOptymalizer_AI_ConsoleApp.AI.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes
{
	public class EvolutionaryAlgorithm : IAlgorithm
	{
		private readonly ICombatSimulator combatSimulator;
		private readonly IUnitForcesFactory unitForcesFactory;
		private readonly IDataIOHandler dataIOHandler;
		private readonly FastRandom randomizer;

		private List<long> randomFitnessRollingTable;
		private Fleet maxFleet;
		private int bestFitness;
		private int noChangeStreakCounter;

		public EvolutionaryAlgorithm()
		{
			this.combatSimulator = new CombatSimulator();
			this.unitForcesFactory = new UnitForcesFactory();
			this.randomizer = new FastRandom();
			this.dataIOHandler = new DataIOHandler();
		}

		public IOutputData Process(IConfigurationData configuration, IInputData input, IGameData gameData)
		{
			this.randomFitnessRollingTable = new List<long>(Enumerable.Repeat(0L, configuration.GenerationSize + 1));
			this.maxFleet = this.GetMaxFleet(input);

			var defenderUnitForces = this.unitForcesFactory.CreateDefender(input, gameData);
			var generation = this.InitializeFirstGeneration(configuration);
			Individual theBestIndividual;
			var generationCounter = 0;

			do
			{
				theBestIndividual = this.CalculateFitnessValuesAndGetBestIndividual(configuration, input, gameData, generation, defenderUnitForces);

				if (generationCounter % 1 == 0)
				{
					Console.WriteLine($"Generation: {generationCounter++}\t Best fitness: {theBestIndividual.FitnessValue}");
				}
				if (generationCounter % 10 == 0)
				{
					var outputData = this.GetOutputData(theBestIndividual);

					this.dataIOHandler.SaveOutput(outputData, $@".\Output\generation_{generationCounter}");
				}

				this.ResetRandomFitnessRollingTable(configuration, generation);

				generation = this.CreateNewGeneration(configuration, generation, theBestIndividual);

				GC.Collect();
			}
			while (!this.IsTheStopCaseMet(configuration, theBestIndividual));

			return this.GetOutputData(theBestIndividual);
		}

		private Individual CalculateFitnessValuesAndGetBestIndividual(IConfigurationData configuration, IInputData input, IGameData gameData, List<Individual> generation, UnitForces defenderUnitForces)
		{
			object locker = new object();
			var nextIndividualIndex = 0;
			var threadFitnessCalculation = new ThreadFitnessCalculation(configuration, input, gameData, unitForcesFactory, combatSimulator, generation, defenderUnitForces, locker, ref nextIndividualIndex);

			var threads = new Thread[configuration.ThreadNumber];
			for (var i = 0; i < threads.Length; i++)
			{
				threads[i] = new Thread(new ThreadStart(threadFitnessCalculation.Calculate));
				threads[i].Start();
			}

			for (var i = 0; i < threads.Length; i++)
			{
				threads[i].Join();
			}

			Individual bestIndividual = generation[0];

			foreach (var individual in generation)
			{
				if (individual.FitnessValue > bestIndividual.FitnessValue)
				{
					bestIndividual = individual;
				}
			}

			return bestIndividual;
		}

		private Fleet GetMaxFleet(IInputData input)
		{
			var fleet = new Fleet();

			fleet.FleetUnits[UnitType.SmallCargo] = input.AttackerData.SmallCargo;
			fleet.FleetUnits[UnitType.LargeCargo] = input.AttackerData.LargeCargo;
			fleet.FleetUnits[UnitType.LightFighter] = input.AttackerData.LightFighter;
			fleet.FleetUnits[UnitType.HeavyFighter] = input.AttackerData.HeavyFighter;
			fleet.FleetUnits[UnitType.Cruiser] = input.AttackerData.Cruiser;
			fleet.FleetUnits[UnitType.Battleship] = input.AttackerData.Battleship;
			fleet.FleetUnits[UnitType.ColonyShip] = input.AttackerData.ColonyShip;
			fleet.FleetUnits[UnitType.Recycler] = input.AttackerData.Recycler;
			fleet.FleetUnits[UnitType.Probe] = input.AttackerData.Probe;
			fleet.FleetUnits[UnitType.Bomber] = input.AttackerData.Bomber;
			fleet.FleetUnits[UnitType.Destroyer] = input.AttackerData.Destroyer;
			fleet.FleetUnits[UnitType.Deathstar] = input.AttackerData.Deathstar;
			fleet.FleetUnits[UnitType.Battlecruiser] = input.AttackerData.Battlecruiser;
			fleet.FleetUnits[UnitType.Reaper] = input.AttackerData.Reaper;
			fleet.FleetUnits[UnitType.Pathfinder] = input.AttackerData.Pathfinder;

			return fleet;
		}

		private void ResetRandomFitnessRollingTable(IConfigurationData configuration, List<Individual> generation)
		{
			var fitnessMinValueAdjustment = generation.Min(x => x.FitnessValue);

			// To adjust negative fitness values (if exist) we move all values
			//  so they start min (fixed) range - 100
			fitnessMinValueAdjustment = fitnessMinValueAdjustment > 0
				? 0
				: -fitnessMinValueAdjustment + 100;

			for (int i = 0; i < configuration.GenerationSize; i++)
			{
				this.randomFitnessRollingTable[i + 1] = this.randomFitnessRollingTable[i] + generation[i].FitnessValue + fitnessMinValueAdjustment;
			}
		}

		private List<Individual> CreateNewGeneration(IConfigurationData configuration, List<Individual> generation, Individual theBestIndividual)
		{
			var newGeneration = this.PrepareNewGeneration(configuration, theBestIndividual);

			while (newGeneration.Count < configuration.GenerationSize)
			{
				if (this.randomizer.Next0to100() < configuration.MutationChanceProbablityPercentage)
				{
					newGeneration.Add(this.PerformMutation(configuration, generation));
				}
				else
				{
					newGeneration.Add(this.PerformCrossover(generation));
				}
			}

			return newGeneration;
		}

		private IOutputData GetOutputData(Individual theBestIndividual)
		{
			return new OutputData()
			{
				WinnerIndividual = theBestIndividual
			};
		}

		private Individual PerformMutation(IConfigurationData configuration, List<Individual> generation)
		{
			var fleetToMutate = this.PickRandomIndividualBasedOnFitness(generation).Fleet.Copy();
			var unitTypeToMutate = (UnitType)this.randomizer.Next((int)UnitType.SmallCargo, (int)UnitType.Pathfinder + 1);
			var units = fleetToMutate.FleetUnits[unitTypeToMutate];

			if (units == 0)
			{
				fleetToMutate.FleetUnits[unitTypeToMutate] = GetRandomPercentageValueOfNumber(randomizer, this.maxFleet.FleetUnits[unitTypeToMutate]);
			}
			else
			{
				if (this.randomizer.Next0to100() < configuration.ChanceOfMutationToZeroPercentage)
				{
					fleetToMutate.FleetUnits[unitTypeToMutate] = 0;
				}
				else if (this.randomizer.Next0to100() < configuration.ChanceOfMutationToMaxPercentage)
				{
					fleetToMutate.FleetUnits[unitTypeToMutate] = this.maxFleet.FleetUnits[unitTypeToMutate];
				}
				else
				{
					int newUnitValue;
					do
					{
						newUnitValue = fleetToMutate.FleetUnits[unitTypeToMutate];
						var modificationPercentage = this.randomizer.Next(configuration.MinMutationModificationPercentage, configuration.MaxMutationModificationPercentage + 1);
						var modificationValue = Math.Max(1, CalculationHelper.GetPercentageValue(units, modificationPercentage));

						if (this.randomizer.NextBool())
						{
							newUnitValue += modificationValue;
						}
						else
						{
							newUnitValue -= modificationValue;
						}

					} while (newUnitValue > this.maxFleet.FleetUnits[unitTypeToMutate]);

					fleetToMutate.FleetUnits[unitTypeToMutate] = newUnitValue;
				}
			}

			return new Individual(fleetToMutate, 0);
		}

		private Individual PerformCrossover(List<Individual> generation)
		{
			var leftParentFleet = this.PickRandomIndividualBasedOnFitness(generation).Fleet;
			var rightParentFleet = this.PickRandomIndividualBasedOnFitness(generation).Fleet;
			var childFleet = new Fleet();

			foreach (var unitType in this.maxFleet.FleetUnits.Keys)
			{
				var parentToGetUnitFrom = this.randomizer.NextBool() ? leftParentFleet : rightParentFleet;

				childFleet.FleetUnits[unitType] = parentToGetUnitFrom.FleetUnits[unitType];
			}

			return new Individual(childFleet, 0);
		}

		private List<Individual> PrepareNewGeneration(IConfigurationData configuration, Individual theBestIndividual)
		{
			return new List<Individual>(configuration.GenerationSize)
			{
				theBestIndividual
			};
		}

		private bool IsTheStopCaseMet(IConfigurationData configuration, Individual theBestIndividual)
		{
			if (theBestIndividual.FitnessValue > this.bestFitness)
			{
				this.bestFitness = theBestIndividual.FitnessValue;
				this.noChangeStreakCounter = 0;
			}
			else
			{
				this.noChangeStreakCounter++;
			}

			return this.noChangeStreakCounter >= configuration.MaxNumberOfGenerationsWithoutFitnessImprovement;
		}

		private List<Individual> InitializeFirstGeneration(IConfigurationData configuration)
		{
			var randomGeneration = new List<Individual>(configuration.GenerationSize)
			{
				new Individual(this.maxFleet.Copy(), fitnessValue: 0)
			};

			while (randomGeneration.Count < configuration.GenerationSize)
			{
				var randomFleet = new Fleet();

				foreach (var unitType in this.maxFleet.FleetUnits.Keys)
				{
					randomFleet.FleetUnits[unitType] = GetRandomPercentageValueOfNumber(randomizer, this.maxFleet.FleetUnits[unitType]);
				}

				randomGeneration.Add(new Individual(randomFleet, fitnessValue: 0));
			}

			return randomGeneration;
		}

		private Individual PickRandomIndividualBasedOnFitness(List<Individual> generation)
		{
			var randomFitnessChance = NextLong(randomizer, 0, this.randomFitnessRollingTable.Last());

			// Modified binary search to search for matching range

			int leftIndex = 0;
			int rightIndex = this.randomFitnessRollingTable.Count - 1;

			while (rightIndex-leftIndex != 1)
			{
				int middleIndex = (rightIndex + leftIndex) / 2;

				if (this.randomFitnessRollingTable[middleIndex] > randomFitnessChance)
				{
					rightIndex = middleIndex;
				}
				else
				{
					leftIndex = middleIndex;
				}
			}

			return generation[leftIndex];
		}

		private static long NextLong(FastRandom random, long min, long max)
		{
			byte[] buf = new byte[8];
			random.NextBytes(buf);
			long longRand = BitConverter.ToInt64(buf, 0);

			return Math.Abs(longRand % (max + 1 - min)) + min;
		}

		private static int GetRandomPercentageValueOfNumber(FastRandom random, int number)
		{
			return (int)Math.Round((double)random.Next0to100() * number / 100);
		}
	}
}
