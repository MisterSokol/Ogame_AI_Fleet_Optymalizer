using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class CombatResultFactory : ICombatResultFactory
	{
		public ICombatResult Create(WinnerType winner, Resources attackerProfitResources)
		{
			return new CombatResult(winner, attackerProfitResources);
		}
	}
}
