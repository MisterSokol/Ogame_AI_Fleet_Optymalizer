namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IInputData
	{
		// Attacker details
		IInputPlayerData AttackerData { get; set; }
		IInputPlayerData DefenderData { get; set; }
		//int AttackerClass { get; set; }
		//int AttackerWeaponLevel { get; set; }
		//int AttackerShieldLevel { get; set; }
		//int AttackerArmourLevel { get; set; }
		//int AttackerCombustionLevel { get; set; }
		//int AttackerImpulseLevel { get; set; }
		//int AttackerHyperspaceLevel { get; set; }
		//int AttackerHyperspaceTechnologyLevel { get; set; }
		//int AttackerGalaxyNumber { get; set; }
		//int AttackerSystemNumber { get; set; }
		//int AttackerPositionNumber { get; set; }

		//int AttackerSmallCargo { get; set; }
		//int AttackerLargeCargo { get; set; }
		//int AttackerLightFighter { get; set; }
		//int AttackerHeavyFighter { get; set; }
		//int AttackerCruiser { get; set; }
		//int AttackerBattleship { get; set; }
		//int AttackerColonyShip { get; set; }
		//int AttackerRecycler { get; set; }
		//int AttackerProbe { get; set; }
		//int AttackerBomber { get; set; }
		//int AttackerDestroyer { get; set; }
		//int AttackerDeathstar { get; set; }
		//int AttackerBattlecruiser { get; set; }
		//int AttackerReaper { get; set; }
		//int AttackerPathfinder { get; set; }
		//string AttackerCoordinates { get; set; }
		//int AttackerFlightSpeedPercentage { get; set; }

		// Defender details
		//int DefenderClass { get; set; }
		//int DefenderWeaponLevel { get; set; }
		//int DefenderShieldLevel { get; set; }
		//int DefenderArmourLevel { get; set; }
		//int DefenderHyperspaceTechnologyLevel { get; set; }
		//int DefenderGalaxyNumber { get; set; }
		//int DefenderSystemNumber { get; set; }
		//int DefenderPositionNumber { get; set; }

		//int DefenderSmallCargo { get; set; }
		//int DefenderLargeCargo { get; set; }
		//int DefenderLightFighter { get; set; }
		//int DefenderHeavyFighter { get; set; }
		//int DefenderCruiser { get; set; }
		//int DefenderBattleship { get; set; }
		//int DefenderColonyShip { get; set; }
		//int DefenderRecycler { get; set; }
		//int DefenderProbe { get; set; }
		//int DefenderBomber { get; set; }
		//int DefenderDestroyer { get; set; }
		//int DefenderDeathstar { get; set; }
		//int DefenderBattlecruiser { get; set; }
		//int DefenderReaper { get; set; }
		//int DefenderPathfinder { get; set; }
		//string DefenderCoordinates { get; set; }
		//int DefenderRocketLauncher { get; set; }
		//int DefenderLightLaser { get; set; }
		//int DefenderHeavyLaser { get; set; }
		//int DefenderGaussCannon { get; set; }
		//int DefenderIonCannon { get; set; }
		//int DefenderPlasmaTurret { get; set; }
		//bool DefenderHasSmallShield { get; set; }
		//bool DefenderHasBigShield { get; set; }
		//int DefenderSolarSatelite { get; set; }
		//int DefenderCrawler { get; set; }
		//bool DefenderHasEngineer { get; set; }
		//int DefenderMetalResources { get; set; }
		//int DefenderCrystalResources { get; set; }
		//int DefenderDeuteriumResources { get; set; }

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
