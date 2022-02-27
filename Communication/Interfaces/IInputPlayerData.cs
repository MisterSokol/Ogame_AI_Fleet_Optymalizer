using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IInputPlayerData
	{
		PlayerClassType Class { get; set; }
		int WeaponLevel { get; set; }
		int ShieldLevel { get; set; }
		int ArmourLevel { get; set; }
		int CombustionLevel { get; set; }
		int ImpulseLevel { get; set; }
		int HyperspaceLevel { get; set; }
		int HyperspaceTechnologyLevel { get; set; }
		int GalaxyNumber { get; set; }
		int SystemNumber { get; set; }
		int PositionNumber { get; set; }
		bool HasEngineer { get; set; }

		int SmallCargo { get; set; }
		int LargeCargo { get; set; }
		int LightFighter { get; set; }
		int HeavyFighter { get; set; }
		int Cruiser { get; set; }
		int Battleship { get; set; }
		int ColonyShip { get; set; }
		int Recycler { get; set; }
		int Probe { get; set; }
		int Bomber { get; set; }
		int Destroyer { get; set; }
		int Deathstar { get; set; }
		int Battlecruiser { get; set; }
		int Reaper { get; set; }
		int Pathfinder { get; set; }

		int RocketLauncher { get; set; }
		int LightLaser { get; set; }
		int HeavyLaser { get; set; }
		int GaussCannon { get; set; }
		int IonCannon { get; set; }
		int PlasmaTurret { get; set; }
		bool HasSmallShield { get; set; }
		bool HasBigShield { get; set; }
		int SolarSatelite { get; set; }
		int Crawler { get; set; }

		int MetalResources { get; set; }
		int CrystalResources { get; set; }
		int DeuteriumResources { get; set; }

		Dictionary<DriveTechnologyType, int> GetDriveTechnologies();
	}
}
