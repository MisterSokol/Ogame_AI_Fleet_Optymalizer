using OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class OutputData : IOutputData
	{
		public Individual WinnerIndividual { get; set; }
	}
}
