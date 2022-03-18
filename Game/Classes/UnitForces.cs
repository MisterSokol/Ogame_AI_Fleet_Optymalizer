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
		private readonly int[,] rapidFireTable;
		private readonly Randomizer randomizer;

		private List<Unit> allUnits;
		private List<Unit> aliveUnits;
		private List<Unit> explodedUnits;

		public int AliveUnitsCount => this.aliveUnits.Count;
		public int FleetSpeed { get; private set; }
		public List<Unit> UnitTypesRepresentatives { get; }

		public UnitForces(IGameData gameData, List<Unit> units, List<Unit> unitTypesRepresentatives)
		{
			this.gameData = gameData;
			this.FleetSpeed = this.GetFleetSpeed(unitTypesRepresentatives);

			this.allUnits = units;
			this.aliveUnits = this.allUnits;
			this.explodedUnits = new List<Unit>();
			this.UnitTypesRepresentatives = unitTypesRepresentatives;

			this.randomizer = new Randomizer();

			this.rapidFireTable = this.gameData.RapidFire;
		}

		public IUnitForces Copy()
		{
			return new UnitForces(this.gameData, this.allUnits.Select(x => (Unit)x.Clone()).ToList(), this.UnitTypesRepresentatives.Select(x => (Unit)x.Clone()).ToList());
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
			var aliveAttackersUnitsCount = this.aliveUnits.Count;
			var aliveDefenderUnitAvailableIndexes = defenderUnit.AliveUnitsCount - 1;

			for (int i = 0; i < aliveAttackersUnitsCount; i++)
			{
				var attackerUnit = this.aliveUnits[i];
				var attackerUnitTypeValue = (int)attackerUnit.UnitType;
				Unit defenderTargetedUnit;

				do
				{
					defenderTargetedUnit = defenderUnit.GetRandomAliveUnit(aliveDefenderUnitAvailableIndexes);
					defenderTargetedUnit.TakeHit(this.randomizer, attackerUnit.Damage);
				} 
				while (ShouldAttackAgain(this.rapidFireTable, attackerUnitTypeValue, this.randomizer, defenderTargetedUnit));
			}
		}

		private static bool ShouldAttackAgain(int [,] rapidFireTable, int attackerUnitTypeValue, Randomizer randomizer, Unit defenderTargetedUnit)
		{
			var rapidFire = rapidFireTable[attackerUnitTypeValue, (int)defenderTargetedUnit.UnitType];

			return rapidFire != 0
				? randomizer.RandomFromRange(0, rapidFire - 1) < (rapidFire - 1)
				: false;
		}

		public void EndBattle()
		{
			this.explodedUnits = this.allUnits.Where(x => !x.IsAlive).ToList();
		}

		public Unit GetRandomAliveUnit(int aliveUnitsAvailableIndexes)
		{
			return this.aliveUnits[this.randomizer.RandomFromRange(0, aliveUnitsAvailableIndexes)];
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

		private int GetFleetSpeed(List<Unit> unitTypesRepresentatives)
		{
			var unitsExceptProbe = unitTypesRepresentatives.Where(x => x.UnitType != UnitType.Probe).ToList();

			return unitsExceptProbe.Count > 0
				? unitsExceptProbe.Min(x => x.Speed)
				: 0;
		}
	}
}
