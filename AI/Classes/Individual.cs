using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes
{
	public class Individual
	{
		public Fleet Fleet { get; set; }
		public int FitnessValue { get; set; }
		public ISimulationResult SimulationResult { get; set; }

		public Individual(Fleet fleet, int fitnessValue)
		{
			this.Fleet = fleet;
			this.FitnessValue = fitnessValue;
		}

		public Individual()
		{
		}
	}
}
