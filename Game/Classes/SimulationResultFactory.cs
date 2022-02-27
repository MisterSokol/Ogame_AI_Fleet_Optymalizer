using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class SimulationResultFactory : ISimulationResultFactory
	{
		public ISimulationResult CreateEmpty()
		{
			return new SimulationResult();
		}
	}
}
