using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class UnitFactory : IUnitFactory
	{
		public List<Unit> CreateUnits(UnitType unitType, IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData, int count)
		{
			var units = new List<Unit>(count);
			var oryginal = this.CreateUnit(unitType, inputData, inputPlayerData, gameData);

			for (int i = 0; i < count; i++)
			{
				units.Add((Unit)oryginal.Clone());
			}

			return units;
		}

		public Unit CreateUnit(UnitType unitType, IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData)
		{
			var unitGameData = gameData.UnitsData[unitType];
			var driveTechnologies = inputPlayerData.GetDriveTechnologies();
			var driveInstalled = unitGameData.GetActiveDriveTechnology(driveTechnologies);

			return new Unit(
				unitType,
				debris: this.GetUnitDebris(inputData, unitGameData),
				resourcesCapacity: this.GetResourcesCapacity(inputData, inputPlayerData, unitGameData),
				unitResourcesCost: unitGameData.ResourcesCost,
				maxHP: this.GetMaxHp(inputPlayerData, unitGameData),
				maxShieldValue: this.GetMaxShieldValue(inputPlayerData, unitGameData),
				damage: this.GetDamage(inputPlayerData, unitGameData),
				speed: this.GetSpeed(driveTechnologies, driveInstalled),
				fuelConsumption: driveInstalled.FuelConsumption
				);
		}

		private int GetSpeed(Dictionary<DriveTechnologyType, int> driveTechnologies, DriveTechnology driveInstalled)
		{
			var speedModifierPercentage = driveInstalled.TechnologyType == DriveTechnologyType.Combustion
				? 10
				: driveInstalled.TechnologyType == DriveTechnologyType.Impulse
					? 20
					: 30;

			return CalculationHelper.GetPercentageValue(driveInstalled.BaseSpeed, 100 + driveTechnologies[driveInstalled.TechnologyType] * speedModifierPercentage);
		}

		private int GetDamage(IInputPlayerData inputPlayerData, UnitGameData unitGameData)
		{
			var classBonus = inputPlayerData.Class == PlayerClassType.General ? 2 : 0;
			var weaponLevel = inputPlayerData.WeaponLevel + classBonus;

			return CalculationHelper.GetPercentageValue(unitGameData.BaseDamage, 100 + 10 * weaponLevel);
		}

		private int GetMaxShieldValue(IInputPlayerData inputPlayerData, UnitGameData unitGameData)
		{
			var classBonus = inputPlayerData.Class == PlayerClassType.General ? 2 : 0;
			var shieldLevel = inputPlayerData.ShieldLevel + classBonus;

			return CalculationHelper.GetPercentageValue(unitGameData.BaseShieldValue, 100 + 10 * shieldLevel);
		}

		private int GetMaxHp(IInputPlayerData inputPlayerData, UnitGameData unitGameData)
		{
			var classBonus = inputPlayerData.Class == PlayerClassType.General ? 2 : 0;
			var armourLevel = inputPlayerData.ArmourLevel + classBonus;

			return CalculationHelper.GetPercentageValue(unitGameData.BaseHP, 100 + 10 * armourLevel);
		}

		private Resources GetUnitDebris(IInputData inputData, UnitGameData unitGameData)
		{
			var unitToDebrisPercentage = unitGameData.IsFleet
				? inputData.FleetToDebrisPercentage
				: inputData.DefenceToDebrisPercentage;

			var unitDebris = unitGameData.ResourcesCost.GetPercentage(unitToDebrisPercentage);
			unitDebris.Deuterium = 0;

			return unitDebris;
		}

		private int GetResourcesCapacity(IInputData inputData, IInputPlayerData inputPlayerData, UnitGameData unitGameData)
		{
			var hyperspaceFactor = inputData.CargoHyperspaceMultiplier * inputPlayerData.HyperspaceTechnologyLevel;
			var classFactor = inputPlayerData.Class == PlayerClassType.Collector
				? inputData.CollectorIncreasedTradingShipCargoPercentage
				: 0;

			return unitGameData.IsFleet
				? CalculationHelper.GetPercentageValue(unitGameData.Capacity, 100 + hyperspaceFactor + classFactor)
				: 0;
		}
	}
}
