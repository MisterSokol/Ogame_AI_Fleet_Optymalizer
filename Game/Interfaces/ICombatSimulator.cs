using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface ICombatSimulator
	{
		ISimulationResult RunSimulation(IInputData inputData, UnitForces attackerFleet, UnitForces defenderUnits);
	}
}
