using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class UnitGameData
	{
		public UnitType UnitType { get; set; }
		public string Name { get; set; }
		public int BaseHP { get; set; }
		public int BaseShieldValue { get; set; }
		public int BaseDamage { get; set; }
		public Resources ResourcesCost { get; set; }
		public Dictionary<UnitType, int> FastGuns { get; set; }
		public bool IsFleet { get; set; }
		public int Capacity { get; set; }
		public bool IsCivilUnit { get; set; }
		public List<DriveTechnology> DriveTechnologies { get; set; }

		public DriveTechnology GetActiveDriveTechnology(Dictionary<DriveTechnologyType, int> driveLevels)
		{
			if (!this.DriveTechnologies.Any())
			{
				return new DriveTechnology
				{
					BaseSpeed = 0,
					FuelConsumption = 0,
					MinLevelOfTechnologyTypeRequired = 0,
					TechnologyType = DriveTechnologyType.Combustion
				};
			}

			DriveTechnology bestAvailableDrive = this.DriveTechnologies[0];

			for (int i = 1; i < this.DriveTechnologies.Count; i++)
			{
				var bestCandidate = this.DriveTechnologies[i];

				if (bestCandidate.MinLevelOfTechnologyTypeRequired <= driveLevels[bestCandidate.TechnologyType])
				{
					bestAvailableDrive = bestCandidate;
				}
			}

			return bestAvailableDrive;
		}
	}
}
