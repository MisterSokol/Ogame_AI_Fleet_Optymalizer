using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class InputData : IInputData
	{
		public InputPlayerData AttackerData { get; set; }
		public InputPlayerData DefenderData { get; set; }

		public int NumberOfSimulations { get; set; }
		public int PlunderPercentage { get; set; }
		public int FleetSpeedMultiplier { get; set; }
		public int FleetToDebrisPercentage { get; set; }
		public int DefenceToDebrisPercentage { get; set; }
		public int NumberOfGalaxies { get; set; }
		public int DeuteriumSaveFactor { get; set; }
		public int CargoHyperspaceMultiplier { get; set; }
		public int CollectorFasterTradingShipsPercentage { get; set; }
		public int CollectorIncreasedTradingShipCargoPercentage { get; set; }
		public int GeneralFasterCombatShipsPercentage { get; set; }
		public int GeneralFasterRecyclersPercentage { get; set; }
		public int GeneralReducedRecyclerConsumptionPercentage { get; set; }
		public int ReaperDebrisHarvestLimitPercentage { get; set; }
	}
}
