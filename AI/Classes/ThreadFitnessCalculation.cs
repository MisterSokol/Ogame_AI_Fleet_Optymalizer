using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes
{
	public class ThreadFitnessCalculation
	{
		private readonly IConfigurationData configuration;
		private readonly IInputData input;
		private readonly IGameData gameData;
		private readonly IUnitForcesFactory unitForcesFactory;
		private readonly ICombatSimulator combatSimulator;

		private List<Individual> generation;
		private IUnitForces defenderUnitForces;
		private object locker;
		private int nextIndividualIndex;

		public ThreadFitnessCalculation(
			IConfigurationData configuration,
			IInputData input,
			IGameData gameData,
			IUnitForcesFactory unitForcesFactory,
			ICombatSimulator combatSimulator,
			List<Individual> generation,
			IUnitForces defenderUnitForces,
			object locker,
			ref int nextIndividualIndex
			)
		{
			this.configuration = configuration;
			this.input = input;
			this.gameData = gameData;
			this.unitForcesFactory = unitForcesFactory;
			this.combatSimulator = combatSimulator;
			this.generation = generation;
			this.defenderUnitForces = defenderUnitForces;
			this.locker = locker;
			this.nextIndividualIndex = nextIndividualIndex;
		}

		public void Calculate()
		{
			//Individual bestThreadIndividual = null;
			Individual nextIndividual;

			while (true)
			{
				lock (this.locker)
				{
					if (this.nextIndividualIndex < this.generation.Count)
					{
						//Console.WriteLine($"Calculating Fitness for indivisual number: {this.nextIndividualIndex}");

						nextIndividual = this.generation[this.nextIndividualIndex];
						this.nextIndividualIndex++;
					}
					else
					{
						break;
					}
				}

				this.CalculateFitness(nextIndividual);

				//if (bestThreadIndividual == null || nextIndividual.FitnessValue > bestThreadIndividual.FitnessValue)
				//{
				//	bestThreadIndividual = nextIndividual;
				//}
			}

			//return bestThreadIndividual;
		}

		private void CalculateFitness(Individual individual)
		{
			if (individual.FitnessValue != 0)
			{
				return;
			}

			var individualUnitForces = this.unitForcesFactory.CreateAttacker(this.input, individual.Fleet, this.gameData);

			var simulationResult = this.combatSimulator.RunSimulation(this.input, individualUnitForces, this.defenderUnitForces.Copy());

			individual.FitnessValue = this.GetFitnessValue(simulationResult);
			individual.SimulationResult = simulationResult;
		}

		private int GetFitnessValue(ISimulationResult simulationResult)
		{
			var total = 0;

			total += this.GetWinFitnessValue(simulationResult);
			total += this.GetProfitFitnessValue(simulationResult);
			total += this.GetFleetSpeedFitnessValue(simulationResult);

			total = this.ApplyRatioPenalty(total, simulationResult);

			return total;
		}

		private int GetFleetSpeedFitnessValue(ISimulationResult simulationResult)
		{
			return simulationResult.AttackerFlightSpeed * configuration.FlightSpeedFitnessMultiplier;
		}

		private int GetProfitFitnessValue(ISimulationResult simulationResult)
		{
			var profitResources = simulationResult.AttackerAverageProfitResources - simulationResult.FuelConsumption;

			return profitResources.GetTotalWorth(configuration) * configuration.ProfitResourcesFitnessMultiplier;
		}

		private int GetWinFitnessValue(ISimulationResult simulationResult)
		{
			return simulationResult.AttackerWinningChancePercentage * configuration.AttackerWinFitnessMultiplier
				+ simulationResult.DrawChancePercentage * configuration.DrawFitnessMultiplier
				+ simulationResult.DefenderWinningChancePercentage * configuration.DefenderWinFitnessMultiplier;
		}

		private int ApplyRatioPenalty(int fitnessValue, ISimulationResult simulationResult)
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
	}
}
