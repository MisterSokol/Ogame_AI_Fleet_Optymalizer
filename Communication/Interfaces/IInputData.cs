namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IInputData
	{
		InputPlayerData AttackerData { get; set; }
		InputPlayerData DefenderData { get; set; }

		// Simulation details
		int NumberOfSimulations { get; set; }
		int PlunderPercentage { get; set; }
		int FleetSpeedMultiplier { get; set; }
		int FleetToDebrisPercentage { get; set; }
		int DefenceToDebrisPercentage { get; set; }
		int NumberOfGalaxies { get; set; }
		int DeuteriumSaveFactor { get; set; }
		int CargoHyperspaceMultiplier { get; set; }
		int CollectorFasterTradingShipsPercentage { get; set; }
		int CollectorIncreasedTradingShipCargoPercentage { get; set; }
		int GeneralFasterCombatShipsPercentage { get; set; }
		int GeneralFasterRecyclersPercentage { get; set; }
		int GeneralReducedRecyclerConsumptionPercentage { get; set; }
		int ReaperDebrisHarvestLimitPercentage { get; set; }
	}
}
