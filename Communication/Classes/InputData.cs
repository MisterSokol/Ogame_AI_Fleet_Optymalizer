using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class InputData : IInputData
	{
		//public int AttackerClass { get; set; }
		//public int AttackerWeaponLevel { get; set; }
		//public int AttackerShieldLevel { get; set; }
		//public int AttackerArmourLevel { get; set; }
		//public int AttackerCombustionLevel { get; set; }
		//public int AttackerImpulseLevel { get; set; }
		//public int AttackerHyperspaceLevel { get; set; }
		//public int AttackerHyperspaceTechnologyLevel { get; set; }
		//public int AttackerGalaxyNumber { get; set; }
		//public int AttackerSystemNumber { get; set; }
		//public int AttackerPositionNumber { get; set; }
		//public int AttackerSmallCargo { get; set; }
		//public int AttackerLargeCargo { get; set; }
		//public int AttackerLightFighter { get; set; }
		//public int AttackerHeavyFighter { get; set; }
		//public int AttackerCruiser { get; set; }
		//public int AttackerBattleship { get; set; }
		//public int AttackerColonyShip { get; set; }
		//public int AttackerRecycler { get; set; }
		//public int AttackerProbe { get; set; }
		//public int AttackerBomber { get; set; }
		//public int AttackerDestroyer { get; set; }
		//public int AttackerDeathstar { get; set; }
		//public int AttackerBattlecruiser { get; set; }
		//public int AttackerReaper { get; set; }
		//public int AttackerPathfinder { get; set; }
		//public string AttackerCoordinates { get; set; }
		//public int AttackerFlightSpeedPercentage { get; set; }
		//public int DefenderClass { get; set; }
		//public int DefenderWeaponLevel { get; set; }
		//public int DefenderShieldLevel { get; set; }
		//public int DefenderArmourLevel { get; set; }
		//public int DefenderHyperspaceTechnologyLevel { get; set; }
		//public int DefenderGalaxyNumber { get; set; }
		//public int DefenderSystemNumber { get; set; }
		//public int DefenderPositionNumber { get; set; }
		//public int DefenderSmallCargo { get; set; }
		//public int DefenderLargeCargo { get; set; }
		//public int DefenderLightFighter { get; set; }
		//public int DefenderHeavyFighter { get; set; }
		//public int DefenderCruiser { get; set; }
		//public int DefenderBattleship { get; set; }
		//public int DefenderColonyShip { get; set; }
		//public int DefenderRecycler { get; set; }
		//public int DefenderProbe { get; set; }
		//public int DefenderBomber { get; set; }
		//public int DefenderDestroyer { get; set; }
		//public int DefenderDeathstar { get; set; }
		//public int DefenderBattlecruiser { get; set; }
		//public int DefenderReaper { get; set; }
		//public int DefenderPathfinder { get; set; }
		//public string DefenderCoordinates { get; set; }
		//public int DefenderRocketLauncher { get; set; }
		//public int DefenderLightLaser { get; set; }
		//public int DefenderHeavyLaser { get; set; }
		//public int DefenderGaussCannon { get; set; }
		//public int DefenderIonCannon { get; set; }
		//public int DefenderPlasmaTurret { get; set; }
		//public bool DefenderHasSmallShield { get; set; }
		//public bool DefenderHasBigShield { get; set; }
		//public int DefenderSolarSatelite { get; set; }
		//public int DefenderCrawler { get; set; }
		//public bool DefenderHasEngineer { get; set; }
		//public int DefenderMetalResources { get; set; }
		//public int DefenderCrystalResources { get; set; }
		//public int DefenderDeuteriumResources { get; set; }
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
		public IInputPlayerData AttackerData { get; set; }
		public IInputPlayerData DefenderData { get; set; }
	}
}
