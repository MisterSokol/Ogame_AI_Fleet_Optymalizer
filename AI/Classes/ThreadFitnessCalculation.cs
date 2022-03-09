using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes
{
	public class ThreadFitnessCalculation
	{
		private readonly IConfigurationData configuration;
		private readonly IInputData input;
		private readonly IGameData gameData;
		private readonly IUnitForcesFactory unitForcesFactory;
		private readonly ICombatSimulator combatSimulator;

		private readonly IUnitForces attackerMaxForces;
		private readonly int winMinFitnessValue;
		private readonly int winMaxFitnessValue;
		private readonly int profitMinFitnessValue;
		private readonly int profitMaxFitnessValue;
		private readonly int fleetSpeedMinFitnessValue;
		private readonly int fleetSpeedMaxFitnessValue;
		//private readonly int fuelMinFitnessValue;
		//private readonly int fuelMaxFitnessValue;

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

			this.attackerMaxForces = this.unitForcesFactory.Create(this.input, this.input.AttackerData, this.gameData);

			this.winMinFitnessValue = this.GetWinMinFitnessValue();
			this.winMaxFitnessValue = this.GetWinMaxFitnessValue();
			this.profitMinFitnessValue = this.GetProfitMinFitnessValue();
			this.profitMaxFitnessValue = this.GetProfitMaxFitnessValue();
			this.fleetSpeedMinFitnessValue = this.GetFleetSpeedMinFitnessValue();
			this.fleetSpeedMaxFitnessValue = this.GetFleetSpeedMaxFitnessValue();
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
			var defenderUnitForcesForSimulation = this.defenderUnitForces.Copy();

			var attackerFleetToDefenderRatio = this.GetAttackerFleetToDefenderRatio(individualUnitForces, defenderUnitForcesForSimulation);

			if (this.CanDoTacticalRetreat(attackerFleetToDefenderRatio, individualUnitForces, defenderUnitForces))
			{
				defenderUnitForcesForSimulation.PerformTacticalRetreat();
			}

			var simulationResult = this.combatSimulator.RunSimulation(this.input, individualUnitForces, defenderUnitForcesForSimulation);

			individual.FitnessValue = this.GetFitnessValue(simulationResult);
			simulationResult.AttackerFleetToDefenderRatio = attackerFleetToDefenderRatio;
			individual.SimulationResult = simulationResult;
		}

		private bool CanDoTacticalRetreat(double attackerFleetToDefenderRatio, IUnitForces attackerFleet, IUnitForces defenderUnits)
		{
			var retreatRatio = this.configuration.TacticalRetreatAt3Ratio
				? 3
				: this.configuration.TacticalRetreatAt5Ratio
					? 5
					: int.MaxValue;

			return attackerFleetToDefenderRatio >= retreatRatio;
		}

		private double GetAttackerFleetToDefenderRatio(IUnitForces attackerFleet, IUnitForces defenderUnits)
		{
			return attackerFleet.TacticalPower() / defenderUnits.TacticalPower();
		}

		private int GetFitnessValue(ISimulationResult simulationResult)
		{
			var total = 0;

			total += this.GetWinFitnessValue(simulationResult) * this.configuration.WinPriority;
			total += this.GetProfitFitnessValue(simulationResult) * this.configuration.ProfitPriority;
			total += this.GetFleetSpeedFitnessValue(simulationResult) * this.configuration.FleetSpeedPriority;
			//total += this.GetFuelFitnessValue(simulationResult) * this.configuration.FuelPriority;

			//total = this.ApplyRatioPenalty(total, simulationResult);

			return total;
		}

		private int GetFleetSpeedFitnessValue(ISimulationResult simulationResult)
		{
			return this.NormalizeValue(simulationResult.AttackerFlightSpeed, this.fleetSpeedMinFitnessValue, this.fleetSpeedMaxFitnessValue);
		}

		private int GetProfitFitnessValue(ISimulationResult simulationResult)
		{
			var profit = simulationResult.AttackerAverageProfitResources.GetTotalWorth(configuration);

			return this.NormalizeValue(profit, this.profitMinFitnessValue, this.profitMaxFitnessValue);
		}

		private int GetWinFitnessValue(ISimulationResult simulationResult)
		{
			var winValue = simulationResult.AttackerWinningChancePercentage * configuration.AttackerWinFitnessMultiplier
				+ simulationResult.DrawChancePercentage * configuration.DrawFitnessMultiplier
				+ simulationResult.DefenderWinningChancePercentage * configuration.DefenderWinFitnessMultiplier;

			return this.NormalizeValue(winValue, this.winMinFitnessValue, this.winMaxFitnessValue);
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


		private int NormalizeValue(int value, int minValue, int maxValue)
		{
			const int normalizedDataMin = 0;
			const int normalizedDataMax = 1000000;

			var valuePercentage = (double)(value - minValue) / (maxValue - minValue);

			return (int)((normalizedDataMax - normalizedDataMin) * valuePercentage + normalizedDataMin);
		}

		private int GetWinMinFitnessValue()
		{
			var multipliers = new List<int>() { this.configuration.DefenderWinFitnessMultiplier, this.configuration.DrawFitnessMultiplier, this.configuration.AttackerWinFitnessMultiplier };
			
			return 100 * multipliers.Min();
		}

		private int GetWinMaxFitnessValue()
		{
			var multipliers = new List<int>() { this.configuration.DefenderWinFitnessMultiplier, this.configuration.DrawFitnessMultiplier, this.configuration.AttackerWinFitnessMultiplier };
			return 100 * multipliers.Max();
		}

		private int GetProfitMinFitnessValue()
		{
			var lostResources = this.attackerMaxForces.GetDebrisResources(true) - this.attackerMaxForces.GetLostResources(true);

			return lostResources.GetTotalWorth(this.configuration);
		}

		private int GetProfitMaxFitnessValue()
		{
			var availableResources = new Resources()
			{
				Metal = CalculationHelper.GetPercentageValue(this.input.DefenderData.MetalResources, this.input.PlunderPercentage),
				Crystal = CalculationHelper.GetPercentageValue(this.input.DefenderData.CrystalResources, this.input.PlunderPercentage),
				Deuterium = CalculationHelper.GetPercentageValue(this.input.DefenderData.DeuteriumResources, this.input.PlunderPercentage)
			};

			var gainedResources = availableResources + this.defenderUnitForces.GetDebrisResources(true);

			return gainedResources.GetTotalWorth(this.configuration);
		}

		private int GetFleetSpeedMinFitnessValue()
		{
			return this.attackerMaxForces.UnitTypesRepresentatives.Min(x => x.Speed);
		}

		private int GetFleetSpeedMaxFitnessValue()
		{
			return this.attackerMaxForces.UnitTypesRepresentatives.Max(x => x.Speed);
		}
	}
}
