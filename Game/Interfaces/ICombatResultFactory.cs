using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface ICombatResultFactory
	{
		ICombatResult Create(WinnerType Winner, Resources AttackerProfitResources);
	}
}
