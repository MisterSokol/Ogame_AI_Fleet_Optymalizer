using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnitFactory
	{
		List<Unit> CreateUnits(UnitType unitType, IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData, int count);
		Unit CreateUnit(UnitType unitType, IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData);
	}
}
