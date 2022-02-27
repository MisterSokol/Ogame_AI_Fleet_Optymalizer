using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnitForcesFactory
	{
		IUnitForces Create(IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData);
		IUnitForces CreateDefender(IInputData inputData, IGameData gameData);
		IUnitForces CreateAttacker(IInputData inputData, Fleet fleet, IGameData gameData);
	}
}
