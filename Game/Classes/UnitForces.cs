using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
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

		public int FleetSpeed { get; }
		public List<IUnit> UnitTypesRepresentatives { get; }

		public UnitForces(IGameData gameData, List<IUnit> units, int fleetSpeed, List<IUnit> unitTypesRepresentatives)
		{
			this.gameData = gameData;
			this.FleetSpeed = fleetSpeed;

			this.allUnits = units;
			this.aliveUnits = this.allUnits;
			this.explodedUnits = new List<IUnit>();
			this.UnitTypesRepresentatives = unitTypesRepresentatives;

			this.randomizer = new Randomizer();
		}

		public IUnitForces Copy()
		{
			return new UnitForces(this.gameData, this.allUnits.Select(x => (IUnit)x.Clone()).ToList(), this.FleetSpeed, this.UnitTypesRepresentatives.Select(x => (IUnit)x.Clone()).ToList());
		}

		public Resources GetDebrisResources()
		{
			if (!this.explodedUnits.Any())
			{
				return new Resources();
			}

			return this.explodedUnits
				.Select(x => x.GetDebrisResources())
				.Aggregate((x, y) => x + y);
		}

		public long GetFleetResourcesCapacity()
		{
			return this.aliveUnits.Sum(x => x.GetUnitResourcesCapacity());
		}

		public Resources GetLostResources()
		{
			if (!this.explodedUnits.Any())
			{
				return new Resources();
			}

			return this.explodedUnits
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

				if (!this.ShouldAttackAgain(attackerUnit, defenderTargetedUnit))
				{
					i++;
				}
			}
		}

		private bool ShouldAttackAgain(IUnit attackerUnit, IUnit defenderTargetedUnit)
		{
			if (this.gameData.UnitsData[attackerUnit.UnitType].FastGuns.ContainsKey(defenderTargetedUnit.UnitType))
			{
				var fastGunsValue = this.gameData.UnitsData[attackerUnit.UnitType].FastGuns[defenderTargetedUnit.UnitType];

				return fastGunsValue != 0
					&& this.randomizer.CheckIfHitTheChance((fastGunsValue - 1) * 100 / fastGunsValue);
			}

			return false;
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
			// TODO: It is now not working properly
			var fleetUnits = this.allUnits
				.Where(x => gameData.UnitsData[x.UnitType].IsFleet)
				.ToList();

			var fleetPoints = fleetUnits.Sum(x => x.Points);
			var civilPoints = fleetUnits.Where(x => gameData.UnitsData[x.UnitType].IsCivilUnit).Sum(x => x.Points);

			return (double)fleetPoints + (double)civilPoints / 4;
		}

		public Resources GetFlightCost(IInputData inputData)
		{
			//var fleetFuelUsage = this.allUnits.Sum(x => x.FuelConsumption) * CalculationHelper.GetDistanceBetweenPlayers(inputData)/(double)35000;
			// return new Resources(0, 0, 1 + (int)Math.Round(fleetFuelUsage));
			// For now simple summary of all fuel consumption will be enought
			var fleetFuelUsage = this.allUnits.Sum(x => x.FuelConsumption);

			return new Resources(0, 0, fleetFuelUsage);
		}
	}
}
