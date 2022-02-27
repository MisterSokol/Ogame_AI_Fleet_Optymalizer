using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnitFactory
	{
		List<IUnit> CreateUnits(UnitType unitType, IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData, int count);
	}
}
