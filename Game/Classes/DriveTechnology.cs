using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class DriveTechnology
	{
		public DriveTechnologyType TechnologyType { get; set; }
		public int MinLevelOfTechnologyTypeRequired { get; set; }
		public int BaseSpeed { get; set; }
		public int FuelConsumption { get; set; }
	}
}
