using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Fleet
	{
		//public static int MaxUnitIndex = 14;

		//public int[] FleetUnits { get; set; }

		//public int SmallCargo { get => this.FleetUnits[0]; set => this.FleetUnits[0] = value; }
		//public int LargeCargo { get => this.FleetUnits[1]; set => this.FleetUnits[1] = value; }
		//public int LightFighter { get => this.FleetUnits[2]; set => this.FleetUnits[2] = value; }
		//public int HeavyFighter { get => this.FleetUnits[3]; set => this.FleetUnits[3] = value; }
		//public int Cruiser { get => this.FleetUnits[4]; set => this.FleetUnits[4] = value; }
		//public int Battleship { get => this.FleetUnits[5]; set => this.FleetUnits[5] = value; }
		//public int ColonyShip { get => this.FleetUnits[6]; set => this.FleetUnits[6] = value; }
		//public int Recycler { get => this.FleetUnits[7]; set => this.FleetUnits[7] = value; }
		//public int Probe { get => this.FleetUnits[8]; set => this.FleetUnits[8] = value; }
		//public int Bomber { get => this.FleetUnits[9]; set => this.FleetUnits[9] = value; }
		//public int Destroyer { get => this.FleetUnits[10]; set => this.FleetUnits[10] = value; }
		//public int Deathstar { get => this.FleetUnits[11]; set => this.FleetUnits[11] = value; }
		//public int Battlecruiser { get => this.FleetUnits[12]; set => this.FleetUnits[12] = value; }
		//public int Reaper { get => this.FleetUnits[13]; set => this.FleetUnits[13] = value; }
		//public int Pathfinder { get => this.FleetUnits[14]; set => this.FleetUnits[14] = value; }

		public Dictionary<UnitType, int> FleetUnits { get; set; }

		public Fleet()
		{
			this.FleetUnits = new Dictionary<UnitType, int>();

			this.FleetUnits[UnitType.SmallCargo] = 0;
			this.FleetUnits[UnitType.LargeCargo] = 0;
			this.FleetUnits[UnitType.LightFighter] = 0;
			this.FleetUnits[UnitType.HeavyFighter] = 0;
			this.FleetUnits[UnitType.Cruiser] = 0;
			this.FleetUnits[UnitType.Battleship] = 0;
			this.FleetUnits[UnitType.ColonyShip] = 0;
			this.FleetUnits[UnitType.Recycler] = 0;
			this.FleetUnits[UnitType.Probe] = 0;
			this.FleetUnits[UnitType.Bomber] = 0;
			this.FleetUnits[UnitType.Destroyer] = 0;
			this.FleetUnits[UnitType.Deathstar] = 0;
			this.FleetUnits[UnitType.Battlecruiser] = 0;
			this.FleetUnits[UnitType.Reaper] = 0;
			this.FleetUnits[UnitType.Pathfinder] = 0;
		}

		public Fleet Copy()
		{
			var copy = new Fleet();

			copy.FleetUnits = this.FleetUnits.ToDictionary(x => x.Key, x => x.Value);

			return copy;
		}
	}
}
