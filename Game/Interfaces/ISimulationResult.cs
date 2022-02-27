using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface ISimulationResult
	{
		int AttackerWinningChancePercentage { get; }
		int DrawChancePercentage { get; }
		int DefenderWinningChancePercentage { get; }

		Resources AttackerAverageProfitResources { get; }
		Resources FuelConsumption { get; set; }

		int AttackerFlightSpeed { get; set; }
		double AttackerFleetToDefenderRatio { get; set; }

		void AddCombatResultToSummary(ICombatResult combatResult);
	}
}
