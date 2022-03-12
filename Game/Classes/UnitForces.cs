using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class UnitForces : IUnitForces
	{
		private readonly IGameData gameData;
		private readonly Randomizer randomizer;

		private List<IUnit> allUnits;
		private List<IUnit> aliveUnits;
		private List<IUnit> explodedUnits;

		public int FleetSpeed { get; private set; }
		public List<IUnit> UnitTypesRepresentatives { get; }

		public UnitForces(IGameData gameData, List<IUnit> units, List<IUnit> unitTypesRepresentatives)
		{
			this.gameData = gameData;
			this.FleetSpeed = this.GetFleetSpeed(unitTypesRepresentatives);

			this.allUnits = units;
			this.aliveUnits = this.allUnits;
			this.explodedUnits = new List<IUnit>();
			this.UnitTypesRepresentatives = unitTypesRepresentatives;

			this.randomizer = new Randomizer();
		}

		public IUnitForces Copy()
		{
			return new UnitForces(this.gameData, this.allUnits.Select(x => (IUnit)x.Clone()).ToList(), this.UnitTypesRepresentatives.Select(x => (IUnit)x.Clone()).ToList());
		}

		public Resources GetDebrisResources(bool includeAliveUnits = false)
		{
			var units = includeAliveUnits
				? this.allUnits
				: this.explodedUnits;

			if (!units.Any())
			{
				return new Resources();
			}

			return units
				.Select(x => x.GetDebrisResources())
				.Aggregate((x, y) => x + y);
		}

		public long GetFleetResourcesCapacity()
		{
			return this.aliveUnits.Sum(x => x.GetUnitResourcesCapacity());
		}

		public Resources GetLostResources(bool includeAliveUnits = false)
		{
			var units = includeAliveUnits
				? this.allUnits
				: this.explodedUnits;

			if (!units.Any())
			{
				return new Resources();
			}

			return units
				.Select(x => x.GetUnitResourcesCost())
				.Aggregate((x, y) => x + y);
		}

		public bool HasUnitsLeft()
		{
			return this.aliveUnits.Any();
		}

		public void ResetToFullForces()
		{
			this.allUnits.ForEach(x => x.Reset());
			this.aliveUnits = this.allUnits;
		}

		public void EndRound()
		{
			this.aliveUnits = this.allUnits.Where(x => x.IsAlive).ToList();
			this.aliveUnits.ForEach(x => x.RestoreShield());
		}

		public void HitButDoNotUpdate(IUnitForces defenderUnit)
		{
			for (int i = 0; i < this.aliveUnits.Count; )
			{
				//System.Console.WriteLine($"\t\tUnit index {i}");
				var attackerUnit = this.aliveUnits[i];
				var defenderTargetedUnit = defenderUnit.GetRandomAliveUnit();

				defenderTargetedUnit.TakeHit(this.randomizer, attackerUnit);

				if (!ShouldAttackAgain(this.gameData, this.randomizer, attackerUnit, defenderTargetedUnit))
				{
					i++;
				}
			}
		}

		private static bool ShouldAttackAgain(IGameData gameData, Randomizer randomizer, IUnit attackerUnit, IUnit defenderTargetedUnit)
		{
			var fastGun = gameData.UnitsData[attackerUnit.UnitType].FastGuns[defenderTargetedUnit.UnitType];

			return fastGun > 0
				? randomizer.CheckIfHitTheChance((fastGun - 1) * 100 / fastGun)
				: false;
		}

		public void EndBattle()
		{
			this.explodedUnits = this.allUnits.Where(x => !x.IsAlive).ToList();
		}

		public IUnit GetRandomAliveUnit()
		{
			return this.aliveUnits[this.randomizer.RandomFromRange(0, this.aliveUnits.Count - 1)];
		}

		public double TacticalPower()
		{
			var fleetUnits = this.allUnits
				.Where(x => gameData.UnitsData[x.UnitType].IsFleet)
				.ToList();

			var fleetPoints = fleetUnits.Where(x => !gameData.UnitsData[x.UnitType].IsCivilUnit).Sum(x => x.GetUnitResourcesCost().GetTotal());
			var civilPoints = fleetUnits.Where(x => gameData.UnitsData[x.UnitType].IsCivilUnit).Sum(x => x.GetUnitResourcesCost().GetTotal());

			return (double)fleetPoints + (double)civilPoints / 4;
		}

		public Resources GetFlightCost(IInputData inputData, bool forceEveryFleetMaxSpeed = false)
		{
			// TODO: It's not perfect but it's accurate enought to leave it for now
			var distance = CalculationHelper.GetDistanceBetweenPlayers(inputData);
			var fleetFuelUsage = this.allUnits.Sum(x => 
				1 + 
				Math.Round(
					x.FuelConsumption *
					distance / (double)35000 * 
					(forceEveryFleetMaxSpeed ? 4 : Math.Pow(1+this.FleetSpeed/x.Speed, 2))
				)
			);

			return new Resources(0, 0, (int)fleetFuelUsage);
		}

		public void PerformTacticalRetreat()
		{
			this.allUnits = this.allUnits
				.Where(x => !this.gameData.UnitsData[x.UnitType].IsFleet || !this.gameData.UnitsData[x.UnitType].CanDoTacticalRetreat)
				.ToList();

			this.aliveUnits = this.allUnits;
		}

		private int GetFleetSpeed(List<IUnit> unitTypesRepresentatives)
		{
			var unitsExceptProbe = unitTypesRepresentatives.Where(x => x.UnitType != UnitType.Probe).ToList();

			return unitsExceptProbe.Count > 0
				? unitsExceptProbe.Min(x => x.Speed)
				: 0;
		}
	}
}
