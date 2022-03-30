using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnitForcesFactory
	{
		UnitForces Create(IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData);
		UnitForces CreateDefender(IInputData inputData, IGameData gameData);
		UnitForces CreateAttacker(IInputData inputData, Fleet fleet, IGameData gameData);
	}
}
