using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class UnitForcesFactory : IUnitForcesFactory
	{
		private readonly IUnitFactory unitFactory;

		public UnitForcesFactory()
		{
			this.unitFactory = new UnitFactory();
		}

		public UnitForces Create(IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData)
		{
			var units = new List<Unit>();
			var unitTypesRepresentatives = new List<Unit>();

			foreach (var unitType in Enum.GetValues(typeof(UnitType)).Cast<UnitType>())
			{
				if (unitType == UnitType.SmallDome || unitType == UnitType.LargeDome)
				{
					continue;
				}

				var unitCount = this.GetUnitCount(inputPlayerData, unitType.ToString());

				if (unitCount > 0)
				{
					var unitsOfType = this.unitFactory.CreateUnits(unitType, inputData, inputPlayerData, gameData, unitCount);

					units.AddRange(unitsOfType);

					unitTypesRepresentatives.Add(unitsOfType[0]);
				}
			}

			this.AddDomesToUnits(units, unitTypesRepresentatives, inputData, inputPlayerData, gameData);

			return new UnitForces(gameData, units, unitTypesRepresentatives);
		}

		public UnitForces CreateDefender(IInputData inputData, IGameData gameData)
		{
			var units = new List<Unit>();
			var unitTypesRepresentatives = new List<Unit>();

			foreach (var unitType in Enum.GetValues(typeof(UnitType)))
			{
				if ((UnitType)unitType == UnitType.SmallDome || (UnitType)unitType == UnitType.LargeDome)
				{
					continue;
				}

				var unitCount = this.GetUnitCount(inputData.DefenderData, unitType.ToString());

				if (unitCount > 0)
				{
					//Console.WriteLine($"Start creating {unitType}, amount: {unitCount}");
					var unitsOfType = this.unitFactory.CreateUnits((UnitType)unitType, inputData, inputData.DefenderData, gameData, unitCount);
					//Console.WriteLine($"End creating {unitType}, amount: {unitCount}");
					units.AddRange(unitsOfType);

					unitTypesRepresentatives.Add(unitsOfType[0]);
				}
			}

			return new UnitForces(gameData, units, unitTypesRepresentatives);
		}

		public UnitForces CreateAttacker(IInputData inputData, Fleet fleet, IGameData gameData)
		{
			var units = new List<Unit>();
			var unitTypesRepresentatives = new List<Unit>();

			foreach (var fleetUnitType in fleet.FleetUnits)
			{
				if (fleetUnitType.Value > 0)
				{
					var unitsOfType = this.unitFactory.CreateUnits(fleetUnitType.Key, inputData, inputData.AttackerData, gameData, fleetUnitType.Value);

					units.AddRange(unitsOfType);

					unitTypesRepresentatives.Add(unitsOfType[0]);
				}
			}

			return new UnitForces(gameData, units, unitTypesRepresentatives);
		}

		private int GetUnitCount(IInputPlayerData inputPlayerData, string unitTypeName)
		{
			return (int)inputPlayerData.GetType().GetProperty(unitTypeName).GetValue(inputPlayerData, null);
		}

		private void AddDomesToUnits(List<Unit> units, List<Unit> unitTypesRepresentatives, IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData)
		{
			if (inputPlayerData.HasSmallShield)
			{
				var smallDome = this.unitFactory.CreateUnit(UnitType.SmallDome, inputData, inputPlayerData, gameData);

				units.Add(smallDome);
				unitTypesRepresentatives.Add(smallDome);
			}

			if (inputPlayerData.HasBigShield)
			{
				var largeDome = this.unitFactory.CreateUnit(UnitType.LargeDome, inputData, inputPlayerData, gameData);

				units.Add(largeDome);
				unitTypesRepresentatives.Add(largeDome);
			}
		}
	}
}
