using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class UnitForcesFactory : IUnitForcesFactory
	{
		private readonly IUnitFactory unitFactory;

		public UnitForcesFactory()
		{
			this.unitFactory = new UnitFactory();
		}

		public IUnitForces Create(IInputData inputData, IInputPlayerData inputPlayerData, IGameData gameData)
		{
			var units = new List<IUnit>();
			var slowestSpeed = int.MaxValue;
			var unitTypesRepresentatives = new List<IUnit>();

			foreach (var unitType in Enum.GetValues(typeof(UnitType)))
			{
				var unitCount = this.GetUnitCount(inputPlayerData, unitType.ToString());

				if (unitCount > 0)
				{
					var unitsOfType = this.unitFactory.CreateUnits((UnitType)unitType, inputData, inputPlayerData, gameData, unitCount);

					units.AddRange(unitsOfType);

					slowestSpeed = Math.Min(slowestSpeed, unitsOfType[0].Speed);
					unitTypesRepresentatives.Add(unitsOfType[0]);
				}
			}

			return new UnitForces(gameData, units, slowestSpeed, unitTypesRepresentatives);
		}

		public IUnitForces CreateDefender(IInputData inputData, IGameData gameData)
		{
			var units = new List<IUnit>();
			var slowestSpeed = int.MaxValue;
			var unitTypesRepresentatives = new List<IUnit>();

			foreach (var unitType in Enum.GetValues(typeof(UnitType)))
			{
				var unitCount = this.GetUnitCount(inputData.DefenderData, unitType.ToString());

				if (unitCount > 0)
				{
					//Console.WriteLine($"Start creating {unitType}, amount: {unitCount}");
					var unitsOfType = this.unitFactory.CreateUnits((UnitType)unitType, inputData, inputData.DefenderData, gameData, unitCount);
					//Console.WriteLine($"End creating {unitType}, amount: {unitCount}");
					units.AddRange(unitsOfType);

					slowestSpeed = Math.Min(slowestSpeed, unitsOfType[0].Speed);
					unitTypesRepresentatives.Add(unitsOfType[0]);
				}
			}

			return new UnitForces(gameData, units, slowestSpeed, unitTypesRepresentatives);
		}

		public IUnitForces CreateAttacker(IInputData inputData, Fleet fleet, IGameData gameData)
		{
			var units = new List<IUnit>();
			var slowestSpeed = int.MaxValue;
			var unitTypesRepresentatives = new List<IUnit>();

			foreach (var fleetUnitType in fleet.FleetUnits)
			{
				if (fleetUnitType.Value > 0)
				{
					var unitsOfType = this.unitFactory.CreateUnits(fleetUnitType.Key, inputData, inputData.AttackerData, gameData, fleetUnitType.Value);

					units.AddRange(unitsOfType);

					slowestSpeed = Math.Min(slowestSpeed, unitsOfType[0].Speed);
					unitTypesRepresentatives.Add(unitsOfType[0]);
				}
			}

			return new UnitForces(gameData, units, slowestSpeed, unitTypesRepresentatives);
		}

		private int GetUnitCount(IInputPlayerData inputPlayerData, string unitTypeName)
		{
			return (int)inputPlayerData.GetType().GetProperty(unitTypeName).GetValue(inputPlayerData, null);
		}
	}
}
