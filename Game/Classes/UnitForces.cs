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

		private Unit[] allUnits;
		private int[] aliveUnitsCurrentRoundIndexes;
		private HashSet<int> aliveUnitsNextRoundIndexes;
		private int[] explodedUnitIndexes;
		private int explodedUnitIndexesNextIndex;

		public int AliveUnitsCount => this.aliveUnitsCurrentRoundIndexes.Length;
		public int FleetSpeed { get; private set; }
		public List<Unit> UnitTypesRepresentatives { get; }

		public UnitForces(IGameData gameData, List<Unit> units, List<Unit> unitTypesRepresentatives)
		{
			this.gameData = gameData;
			this.FleetSpeed = this.GetFleetSpeed(unitTypesRepresentatives);

			this.allUnits = units.ToArray();

			this.ResetArrays();

			this.UnitTypesRepresentatives = unitTypesRepresentatives;

			this.randomizer = new Randomizer();

			this.rapidFireTable = this.gameData.RapidFire;
		}

		private void ResetArrays()
		{
			var unitsNumber = this.allUnits.Length;
			this.aliveUnitsCurrentRoundIndexes = new int[unitsNumber];
			this.aliveUnitsNextRoundIndexes = new HashSet<int>(unitsNumber);

			for (var i = 0; i < unitsNumber; i++)
			{
				this.aliveUnitsCurrentRoundIndexes[i] = i;
				this.aliveUnitsNextRoundIndexes.Add(i);
			}

			this.explodedUnitIndexes = new int[unitsNumber];
			this.explodedUnitIndexesNextIndex = 0;
		}

		public IUnitForces Copy()
		{
			return new UnitForces(this.gameData, this.allUnits.Select(x => (Unit)x.Clone()).ToList(), this.UnitTypesRepresentatives.Select(x => (Unit)x.Clone()).ToList());
		}

		public Resources GetDebrisResources(bool includeAliveUnits = false)
		{
			if (includeAliveUnits)
			{
				return this.allUnits
					.Select(x => x.GetDebrisResources())
					.Aggregate((x, y) => x + y);
			}

			var explodedUnitsLength = this.explodedUnitIndexes.Length;
			var sum = new Resources();

			for (int i = 0; i < explodedUnitsLength; i++)
			{
				sum += this.allUnits[this.explodedUnitIndexes[i]].GetDebrisResources();
			}

			return sum;
		}

		public long GetFleetResourcesCapacity()
		{
			var aliveUnitsIndexes = this.aliveUnitsCurrentRoundIndexes.Length;
			var sum = 0L;

			for (int i = 0; i < aliveUnitsIndexes; i++)
			{
				sum += this.allUnits[this.aliveUnitsCurrentRoundIndexes[i]].GetUnitResourcesCapacity();
			}

			return sum;
		}

		public Resources GetLostResources(bool includeAliveUnits = false)
		{
			if (includeAliveUnits)
			{
				return this.allUnits
					.Select(x => x.GetDebrisResources())
					.Aggregate((x, y) => x + y);
			}

			var explodedUnitsLength = this.explodedUnitIndexes.Length;
			var sum = new Resources();

			for (int i = 0; i < explodedUnitsLength; i++)
			{
				sum += this.allUnits[this.explodedUnitIndexes[i]].GetUnitResourcesCost();
			}

			return sum;
		}

		public bool HasUnitsLeft()
		{
			return this.aliveUnitsCurrentRoundIndexes.Length > 0;
		}

		public void ResetToFullForces()
		{
			var unitsNumber = this.allUnits.Length;

			this.ResetArrays();

			for (var i = 0; i < unitsNumber; i++)
			{
				this.allUnits[i].Reset();
			}
		}

		public void EndRound()
		{
			var aliveUnitsCount = this.aliveUnitsNextRoundIndexes.Count;
			this.aliveUnitsCurrentRoundIndexes = new int[aliveUnitsCount];
			var i = 0;

			foreach (var unitIndex in this.aliveUnitsNextRoundIndexes)
			{
				this.aliveUnitsCurrentRoundIndexes[i++] = unitIndex;
				this.allUnits[unitIndex].RestoreShield();
			}
		}

		public void HitButDoNotUpdate(IUnitForces defenderUnit)
		{
			var aliveAttackersUnitsCount = this.aliveUnitsCurrentRoundIndexes.Length;
			var aliveDefenderUnitAvailableIndexes = defenderUnit.AliveUnitsCount - 1;

			for (int i = 0; i < aliveAttackersUnitsCount; i++)
			{
				var attackerUnit = this.allUnits[this.aliveUnitsCurrentRoundIndexes[i]];
				var attackerUnitTypeValue = (int)attackerUnit.UnitType;
				Unit defenderTargetedUnit;
				int defenderUnitIndex;

				do
				{
					(defenderTargetedUnit, defenderUnitIndex) = defenderUnit.GetRandomAliveUnit(aliveDefenderUnitAvailableIndexes);
					defenderTargetedUnit.TakeHit(this.randomizer, attackerUnit.Damage, defenderUnit, defenderUnitIndex);
				} 
				while (ShouldAttackAgain(this.rapidFireTable, attackerUnitTypeValue, this.randomizer, defenderTargetedUnit));
			}
		}

		public void MarkAsExplodedNextRound(int unitIndex)
		{
			this.aliveUnitsNextRoundIndexes.Remove(unitIndex);
			this.explodedUnitIndexes[this.explodedUnitIndexesNextIndex++] = unitIndex;
		}

		private static bool ShouldAttackAgain(int [,] rapidFireTable, int attackerUnitTypeValue, Randomizer randomizer, Unit defenderTargetedUnit)
		{
			var rapidFire = rapidFireTable[attackerUnitTypeValue, (int)defenderTargetedUnit.UnitType];

			return rapidFire != 0
				? randomizer.RandomFromRange(0, rapidFire - 1) < (rapidFire - 1)
				: false;
		}

		public (Unit, int) GetRandomAliveUnit(int aliveUnitsAvailableIndexes)
		{
			var randomIndex = this.aliveUnitsCurrentRoundIndexes[this.randomizer.RandomFromRange(0, aliveUnitsAvailableIndexes)];

			return (this.allUnits[randomIndex], randomIndex);
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
				.ToArray();

			this.ResetArrays();
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
