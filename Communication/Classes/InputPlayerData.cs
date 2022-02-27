using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public class InputPlayerData : IInputPlayerData
	{
		public PlayerClassType Class { get; set; }
		public int WeaponLevel { get; set; }
		public int ShieldLevel { get; set; }
		public int ArmourLevel { get; set; }
		public int CombustionLevel { get; set; }
		public int ImpulseLevel { get; set; }
		public int HyperspaceLevel { get; set; }
		public int HyperspaceTechnologyLevel { get; set; }
		public int GalaxyNumber { get; set; }
		public int SystemNumber { get; set; }
		public int PositionNumber { get; set; }
		public bool HasEngineer { get; set; }

		public int SmallCargo { get; set; }
		public int LargeCargo { get; set; }
		public int LightFighter { get; set; }
		public int HeavyFighter { get; set; }
		public int Cruiser { get; set; }
		public int Battleship { get; set; }
		public int ColonyShip { get; set; }
		public int Recycler { get; set; }
		public int Probe { get; set; }
		public int Bomber { get; set; }
		public int Destroyer { get; set; }
		public int Deathstar { get; set; }
		public int Battlecruiser { get; set; }
		public int Reaper { get; set; }
		public int Pathfinder { get; set; }

		public int RocketLauncher { get; set; }
		public int LightLaser { get; set; }
		public int HeavyLaser { get; set; }
		public int GaussCannon { get; set; }
		public int IonCannon { get; set; }
		public int PlasmaTurret { get; set; }
		public bool HasSmallShield { get; set; }
		public bool HasBigShield { get; set; }
		public int SolarSatelite { get; set; }
		public int Crawler { get; set; }

		public int MetalResources { get; set; }
		public int CrystalResources { get; set; }
		public int DeuteriumResources { get; set; }

		public Dictionary<DriveTechnologyType, int> GetDriveTechnologies()
		{
			return new Dictionary<DriveTechnologyType, int>
			{
				{ DriveTechnologyType.Combustion, this.CombustionLevel },
				{ DriveTechnologyType.Impulse, this.ImpulseLevel },
				{ DriveTechnologyType.Hyperspace, this.HyperspaceLevel }
			};
		}
	}
}
