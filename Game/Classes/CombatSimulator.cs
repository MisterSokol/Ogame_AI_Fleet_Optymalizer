using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class CombatSimulator : ICombatSimulator
	{
		private readonly ICombatResultFactory combatResultFactory;
		private readonly ISimulationResultFactory simulationResultFactory;

		public CombatSimulator()
		{
			this.combatResultFactory = new CombatResultFactory();
			this.simulationResultFactory = new SimulationResultFactory();
		}

		public ISimulationResult RunSimulation(IInputData inputData, UnitForces attackerFleet, UnitForces defenderUnits)
		{
			var combatResults = new List<ICombatResult>();

			for (int i = 0; i < inputData.NumberOfSimulations; i++)
			{
				//Console.WriteLine($"\tSimulation Number: {i}");
				combatResults.Add(this.RunSingleSimulation(inputData, attackerFleet, defenderUnits));

				attackerFleet.ResetToFullForces();
				defenderUnits.ResetToFullForces();
			}

			return this.GetSimulationResult(inputData, combatResults, attackerFleet, defenderUnits);
		}

		private ISimulationResult GetSimulationResult(
			IInputData inputData,
			List<ICombatResult> combatResults,
			UnitForces attackerFleet,
			UnitForces defenderUnits
			)
		{
			var simulationResult = this.simulationResultFactory.CreateEmpty();

			combatResults.ForEach(x => simulationResult.AddCombatResultToSummary(x));

			simulationResult.AttackerFlightSpeed = this.GetFlightTime(attackerFleet, inputData);
			simulationResult.AttackerFleetToDefenderRatio = this.GetAttackerFleetToDefenderRatio(attackerFleet, defenderUnits);
			simulationResult.FuelConsumption = attackerFleet.GetFlightCost(inputData);

			return simulationResult;
		}

		private double GetAttackerFleetToDefenderRatio(UnitForces attackerFleet, UnitForces defenderUnits)
		{
			return attackerFleet.TacticalPower() / defenderUnits.TacticalPower();
		}

		private ICombatResult RunSingleSimulation(IInputData inputData, UnitForces attackerFleet, UnitForces defenderUnits)
		{
			var round = 0;

			while (round < 6
				&& attackerFleet.HasUnitsLeft()
				&& defenderUnits.HasUnitsLeft()
				)
			{
				//Console.WriteLine($"Round {round}");
				defenderUnits.TakeHitButDoNotUpdate(attackerFleet);
				attackerFleet.TakeHitButDoNotUpdate(defenderUnits);

				attackerFleet.EndRound();
				defenderUnits.EndRound();

				round++;
			}

			var winnerType = this.ChooseWinner(round, attackerFleet, defenderUnits);

			return this.combatResultFactory.Create(
				winnerType,
				this.CalculateAttackerProfit(inputData, winnerType, attackerFleet, defenderUnits)
				);
		}

		private int GetFlightTime(UnitForces attackerFleet, IInputData inputData)
		{
			//return (int)(10 + 3500 * Math.Sqrt((double)10 * CalculationHelper.GetDistanceBetweenPlayers(inputData) / attackerFleet.FleetSpeed));
			// For now slimple slowest unit value will be enough
			return attackerFleet.FleetSpeed;
		}

		private Resources CalculateAttackerProfit(IInputData inputData, WinnerType winnerType, UnitForces attackerFleet, UnitForces defenderUnits)
		{
			var stolenResources = winnerType == WinnerType.Attacker
				? this.GetStolenResources(inputData, attackerFleet)
				: Resources.Empty();

			return stolenResources 
				- attackerFleet.GetLostResources()
				+ attackerFleet.GetDebrisResources()
				+ defenderUnits.GetDebrisResources()
				;
		}

		private Resources GetStolenResources(IInputData inputData, UnitForces attackerFleet)
		{
			var attackerResourcesCapacity = attackerFleet.GetFleetResourcesCapacity();

			var availableResources = new Resources()
			{
				Metal = CalculationHelper.GetPercentageValue(inputData.DefenderData.MetalResources, inputData.PlunderPercentage),
				Crystal = CalculationHelper.GetPercentageValue(inputData.DefenderData.CrystalResources, inputData.PlunderPercentage),
				Deuterium = CalculationHelper.GetPercentageValue(inputData.DefenderData.DeuteriumResources, inputData.PlunderPercentage)
			};

			return availableResources.GetResourcesBasedOnCapacity(attackerResourcesCapacity);
		}

		private WinnerType ChooseWinner(int round, UnitForces attackerFleet, UnitForces defenderUnits)
		{
			var didBothSurvieBattle = round == 6 && attackerFleet.HasUnitsLeft() && defenderUnits.HasUnitsLeft();
			var didBothLostBattle = !attackerFleet.HasUnitsLeft() && !defenderUnits.HasUnitsLeft();

			if (didBothSurvieBattle || didBothLostBattle)
			{
				return WinnerType.Draw;
			}
			else if (attackerFleet.HasUnitsLeft())
			{
				return WinnerType.Attacker;
			}
			else
			{
				return WinnerType.Defender;
			}
		}
	}
}
