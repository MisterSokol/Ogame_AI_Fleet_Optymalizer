using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	internal class CombatResult : ICombatResult
	{
		public WinnerType Winner { get;  }
		public Resources AttackerProfitResources { get; }

		public CombatResult(WinnerType winner, Resources attackerProfitResources)
		{
			this.Winner = winner;
			this.AttackerProfitResources = attackerProfitResources;
		}
	}
}