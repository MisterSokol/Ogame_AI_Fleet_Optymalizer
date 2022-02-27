using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class SimulationResult : ISimulationResult
	{
		private Dictionary<WinnerType, int> winningTable;
		private List<Resources> attackerProfitResources;

		public int AttackerWinningChancePercentage => this.GetWinningChancePercentage(WinnerType.Attacker);
		public int DrawChancePercentage => this.GetWinningChancePercentage(WinnerType.Draw);
		public int DefenderWinningChancePercentage => this.GetWinningChancePercentage(WinnerType.Defender);
		public Resources AttackerAverageProfitResources => this.attackerProfitResources.Aggregate((x, y) => x + y) / this.attackerProfitResources.Count;
		public int AttackerFlightSpeed { get; set; }
		public double AttackerFleetToDefenderRatio { get; set; }
		public Resources FuelConsumption { get; set; }

		public SimulationResult()
		{
			this.winningTable = new Dictionary<WinnerType, int>
			{
				{ WinnerType.Attacker, 0 },
				{ WinnerType.Draw, 0 },
				{ WinnerType.Defender, 0 },
			};

			this.attackerProfitResources = new List<Resources>();
		}

		public void AddCombatResultToSummary(ICombatResult combatResult)
		{
			this.winningTable[combatResult.Winner]++;
			this.attackerProfitResources.Add(combatResult.AttackerProfitResources);
		}

		private int GetWinningChancePercentage(WinnerType winnerType)
		{
			return this.winningTable[winnerType] * 100 / this.winningTable.Sum(x => x.Value);
		}
	}
}
