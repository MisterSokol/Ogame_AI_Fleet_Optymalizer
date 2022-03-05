﻿using OGame_FleetOptymalizer_AI_ConsoleApp.AI.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes
{
	public class EvolutionaryAlgorithm : IAlgorithm
	{
		private readonly ICombatSimulator combatSimulator;
		private readonly IUnitForcesFactory unitForcesFactory;

		private List<long> randomFitnessRollingTable;
		private Fleet maxFleet;
		private int bestFitness;
		private int noChangeStreakCounter;

		public EvolutionaryAlgorithm()
		{
			this.combatSimulator = new CombatSimulator();
			this.unitForcesFactory = new UnitForcesFactory();
		}

		public IOutputData Process(IConfigurationData configuration, IInputData input, IGameData gameData)
		{
			this.randomFitnessRollingTable = new List<long>(Enumerable.Repeat(0L, configuration.GenerationSize + 1));
			this.maxFleet = this.GetMaxFleet(input);
			var defenderUnitForces = this.unitForcesFactory.CreateDefender(input, gameData);

			var generation = this.InitializeFirstGeneration(configuration, input);
			Individual theBestIndividual;
			var generationCounter = 0;

			do
			{
				theBestIndividual = this.CalculateFitnessValuesAndGetBestIndividual(configuration, input, gameData, generation, defenderUnitForces);

				Console.WriteLine($"Generation: {generationCounter++}\t Best fitness: {theBestIndividual.FitnessValue}");

				this.ResetRandomFitnessRollingTable(configuration, generation);

				generation = this.CreateNewGeneration(configuration, input, generation, theBestIndividual);

				GC.Collect();
			}
			while (!this.IsTheStopCaseMet(configuration, generation, theBestIndividual));

			return this.GetOutputData(theBestIndividual);
		}

		private Individual CalculateFitnessValuesAndGetBestIndividual(IConfigurationData configuration, IInputData input, IGameData gameData, List<Individual> generation, IUnitForces defenderUnitForces)
		{
			Individual bestIndividual = generation[0];

			foreach (var individual in generation)
			{
				if (individual.FitnessValue != 0)
				{
					continue;
				}

				var individualUnitForces = this.unitForcesFactory.CreateAttacker(input, individual.Fleet, gameData);

				var simulationResult = this.combatSimulator.RunSimulation(input, individualUnitForces, defenderUnitForces);

				individual.FitnessValue = this.GetFitnessValue(configuration, simulationResult);
				individual.SimulationResult = simulationResult;

				if (individual.FitnessValue > bestIndividual.FitnessValue)
				{
					bestIndividual = individual;
				}
			}

			return bestIndividual;
		}

		private int GetFitnessValue(IConfigurationData configuration, ISimulationResult simulationResult)
		{
			var total = 0;

			total += this.GetWinFitnessValue(configuration, simulationResult);
			total += this.GetProfitFitnessValue(configuration, simulationResult);
			total += this.GetFleetSpeedFitnessValue(configuration, simulationResult);

			total = this.ApplyRatioPenalty(total, configuration, simulationResult);

			return total;
		}

		private int ApplyRatioPenalty(int fitnessValue, IConfigurationData configuration, ISimulationResult simulationResult)
		{
			if (simulationResult.AttackerFleetToDefenderRatio >= 3 && configuration.AttackerToDefenderMoreThan3FleetRatioPentaltyPercentage > 0)
			{
				return CalculationHelper.GetPercentageValue(fitnessValue, 100 - configuration.AttackerToDefenderMoreThan3FleetRatioPentaltyPercentage);
			}

			if (simulationResult.AttackerFleetToDefenderRatio >= 5 && configuration.AttackerToDefenderMoreThan5FleetRatioPentaltyPercentage > 0)
			{
				return CalculationHelper.GetPercentageValue(fitnessValue, 100 - configuration.AttackerToDefenderMoreThan5FleetRatioPentaltyPercentage);
			}

			return fitnessValue;
		}

		private int GetFleetSpeedFitnessValue(IConfigurationData configuration, ISimulationResult simulationResult)
		{
			return simulationResult.AttackerFlightSpeed * configuration.FlightSpeedFitnessMultiplier;
		}

		private int GetProfitFitnessValue(IConfigurationData configuration, ISimulationResult simulationResult)
		{
			var profitResources = simulationResult.AttackerAverageProfitResources - simulationResult.FuelConsumption;

			return profitResources.GetTotalWorth(configuration) * configuration.ProfitResourcesFitnessMultiplier;
		}

		private int GetWinFitnessValue(IConfigurationData configuration, ISimulationResult simulationResult)
		{
			return simulationResult.AttackerWinningChancePercentage * configuration.AttackerWinFitnessMultiplier
				+ simulationResult.DrawChancePercentage * configuration.DrawFitnessMultiplier
				+ simulationResult.DefenderWinningChancePercentage * configuration.DefenderWinFitnessMultiplier;
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
			for (int i = 0; i < configuration.GenerationSize; i++)
			{
				this.randomFitnessRollingTable[i + 1] = this.randomFitnessRollingTable[i] + generation[i].FitnessValue;
			}
		}

		private List<Individual> CreateNewGeneration(IConfigurationData configuration, IInputData input, List<Individual> generation, Individual theBestIndividual)
		{
			var newGeneration = this.PrepareNewGeneration(configuration, generation, theBestIndividual);

			while (newGeneration.Count < configuration.GenerationSize)
			{
				if (Randomizer.CheckIfHitTheChance(configuration.MutationChanceProbablityPercentage))
				{
					newGeneration.Add(this.PerformMutation(configuration, input, generation));
				}
				else
				{
					newGeneration.Add(this.PerformCrossover(configuration, input, generation));
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

		private Individual GetTheBestIndividual(IConfigurationData configuration, List<Individual> generation)
		{
			var bestIndividual = generation.First();

			for (int i = 1; i < generation.Count; i++)
			{
				if (bestIndividual.FitnessValue < generation[i].FitnessValue)
				{
					bestIndividual = generation[i];
				}
			}

			return bestIndividual;
		}

		private Individual PerformMutation(IConfigurationData configuration, IInputData input, List<Individual> generation)
		{
			var fleetToMutate = this.PickRandomIndividualBasedOnFitness(generation).Fleet.Copy();
			var unitTypeToMutate = (UnitType)Randomizer.RandomFromRange((int)UnitType.SmallCargo, (int)UnitType.Pathfinder);
			var units = fleetToMutate.FleetUnits[unitTypeToMutate];

			if (units == 0)
			{
				fleetToMutate.FleetUnits[unitTypeToMutate] = Randomizer.GetRandomPercentageValueOfNumber(this.maxFleet.FleetUnits[unitTypeToMutate]);
			}
			else
			{
				if (Randomizer.CheckIfHitTheChance(configuration.ChanceOfMutationToZeroPercentage))
				{
					fleetToMutate.FleetUnits[unitTypeToMutate] = 0;
				}
				else if (Randomizer.CheckIfHitTheChance(configuration.ChanceOfMutationToMaxPercentage))
				{
					fleetToMutate.FleetUnits[unitTypeToMutate] = this.maxFleet.FleetUnits[unitTypeToMutate];
				}
				else
				{
					int newUnitValue;
					do
					{
						newUnitValue = fleetToMutate.FleetUnits[unitTypeToMutate];
						var modificationPercentage = Randomizer.RandomFromRange(configuration.MinMutationModificationPercentage, configuration.MaxMutationModificationPercentage);
						var modificationValue = Math.Max(1, CalculationHelper.GetPercentageValue(units, modificationPercentage));

						if (Randomizer.RandomTrueFalse())
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

		private Individual PerformCrossover(IConfigurationData configuration, IInputData input, List<Individual> generation)
		{
			var leftParentFleet = this.PickRandomIndividualBasedOnFitness(generation).Fleet;
			var rightParentFleet = this.PickRandomIndividualBasedOnFitness(generation).Fleet;
			var childFleet = new Fleet();

			foreach (var unitType in this.maxFleet.FleetUnits.Keys)
			{
				var parentToGetUnitFrom = Randomizer.RandomTrueFalse() ? leftParentFleet : rightParentFleet;

				childFleet.FleetUnits[unitType] = parentToGetUnitFrom.FleetUnits[unitType];
			}

			return new Individual(childFleet, 0);
		}

		private List<Individual> PrepareNewGeneration(IConfigurationData configuration, List<Individual> generation, Individual theBestIndividual)
		{
			return new List<Individual>(configuration.GenerationSize)
			{
				theBestIndividual
			};
		}

		private bool IsTheStopCaseMet(IConfigurationData configuration, List<Individual> generation, Individual theBestIndividual)
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

		private List<Individual> InitializeFirstGeneration(IConfigurationData configuration, IInputData input)
		{
			var randomGeneration = new List<Individual>(configuration.GenerationSize);

			for (int i = 0; i < configuration.GenerationSize; i++)
			{
				var randomFleet = new Fleet();

				foreach (var unitType in this.maxFleet.FleetUnits.Keys)
				{
					randomFleet.FleetUnits[unitType] = Randomizer.GetRandomPercentageValueOfNumber(this.maxFleet.FleetUnits[unitType]);
				}

				randomGeneration.Add(new Individual(randomFleet, fitnessValue: 0));
			}

			return randomGeneration;
		}

		private Individual PickRandomIndividualBasedOnFitness(List<Individual> generation)
		{
			var randomFitnessChance = Randomizer.RandomFromRange(0, this.randomFitnessRollingTable.Last());

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
	}
}
