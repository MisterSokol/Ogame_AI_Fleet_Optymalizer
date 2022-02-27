using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface ICombatSimulator
	{
		ISimulationResult RunSimulation(IInputData inputData, IUnitForces attackerFleet, IUnitForces defenderUnits);
	}
}
