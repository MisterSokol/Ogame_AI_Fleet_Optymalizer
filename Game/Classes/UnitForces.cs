using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class UnitForces : IUnitForces
	{
		private readonly IGameData gameData;
		private readonly int[,] rapidFireTable;
		private readonly FastRandom randomizer;

		private Unit[] allUnits;
		private int[] aliveUnitsCurrentRoundIndexes;
		private bool[] aliveUnitStatusesNextRound;
		private int aliveUnitsNextRoundCount;
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
			var unitsCount = this.allUnits.Length;

			for (var i = 0; i < unitsCount; i++)
			{
				this.allUnits[i].Index = i;
			}

			this.ResetArrays();

			this.UnitTypesRepresentatives = unitTypesRepresentatives;

			this.randomizer = new FastRandom();

			this.rapidFireTable = this.gameData.RapidFire;
		}

		private void ResetArrays()
		{
			var unitsNumber = this.allUnits.Length;
			this.aliveUnitsCurrentRoundIndexes = new int[unitsNumber];
			this.aliveUnitStatusesNextRound = new bool[unitsNumber];

			for (var i = 0; i < unitsNumber; i++)
			{
				this.aliveUnitsCurrentRoundIndexes[i] = i;
				this.aliveUnitStatusesNextRound[i] = true;
			}

			this.explodedUnitIndexes = new int[unitsNumber];
			this.explodedUnitIndexesNextIndex = 0;
			this.aliveUnitsNextRoundCount = unitsNumber;
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
					.Select(x => x.Debris)
					.Aggregate((x, y) => x + y);
			}

			var explodedUnitsLength = this.explodedUnitIndexes.Length;
			var sum = new Resources();

			for (int i = 0; i < explodedUnitsLength; i++)
			{
				sum += this.allUnits[this.explodedUnitIndexes[i]].Debris;
			}

			return sum;
		}

		public long GetFleetResourcesCapacity()
		{
			var aliveUnitsIndexes = this.aliveUnitsCurrentRoundIndexes.Length;
			var sum = 0L;

			for (int i = 0; i < aliveUnitsIndexes; i++)
			{
				sum += this.allUnits[this.aliveUnitsCurrentRoundIndexes[i]].ResourcesCapacity;
			}

			return sum;
		}

		public Resources GetLostResources(bool includeAliveUnits = false)
		{
			if (includeAliveUnits)
			{
				return this.allUnits
					.Select(x => x.Debris)
					.Aggregate((x, y) => x + y);
			}

			var explodedUnitsLength = this.explodedUnitIndexes.Length;
			var sum = new Resources();

			for (int i = 0; i < explodedUnitsLength; i++)
			{
				sum += this.allUnits[this.explodedUnitIndexes[i]].UnitResourcesCost;
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
			var aliveUnitsNextRound = new int[this.aliveUnitsNextRoundCount];
			var aliveUnitsCurrentRoundCount = this.aliveUnitsCurrentRoundIndexes.Length;

			var j = 0;
			for (var i = 0; i < aliveUnitsCurrentRoundCount; i++)
			{
				var unitIndex = this.aliveUnitsCurrentRoundIndexes[i];

				if (this.aliveUnitStatusesNextRound[unitIndex])
				{
					aliveUnitsNextRound[j++] = unitIndex;
					this.allUnits[unitIndex].RestoreShield();
				}
			}

			this.aliveUnitsCurrentRoundIndexes = aliveUnitsNextRound;
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

				do
				{
					defenderTargetedUnit = defenderUnit.GetRandomAliveUnit(aliveDefenderUnitAvailableIndexes);
					defenderTargetedUnit.TakeHit(this.randomizer, attackerUnit.Damage, defenderUnit);
				} 
				while (ShouldAttackAgain(this.rapidFireTable, attackerUnitTypeValue, this.randomizer, defenderTargetedUnit));
			}
		}

		public void MarkAsExplodedNextRound(int unitIndex)
		{
			this.aliveUnitStatusesNextRound[unitIndex] = false;
			this.aliveUnitsNextRoundCount--;
			this.explodedUnitIndexes[this.explodedUnitIndexesNextIndex++] = unitIndex;
		}

		private static bool ShouldAttackAgain(int [,] rapidFireTable, int attackerUnitTypeValue, FastRandom randomizer, Unit defenderTargetedUnit)
		{
			var rapidFire = rapidFireTable[attackerUnitTypeValue, (int)defenderTargetedUnit.UnitType];

			return rapidFire != 0 && randomizer.Next(rapidFire) < (rapidFire - 1);
		}

		public Unit GetRandomAliveUnit(int aliveUnitsAvailableIndexes)
		{
			var randomIndex = this.aliveUnitsCurrentRoundIndexes[this.randomizer.Next(aliveUnitsAvailableIndexes + 1)];

			return this.allUnits[randomIndex];
		}

		public double TacticalPower()
		{
			var fleetUnits = this.allUnits
				.Where(x => gameData.UnitsData[x.UnitType].IsFleet)
				.ToList();

			var fleetPoints = fleetUnits.Where(x => !gameData.UnitsData[x.UnitType].IsCivilUnit).Sum(x => x.UnitResourcesCost.GetTotal());
			var civilPoints = fleetUnits.Where(x => gameData.UnitsData[x.UnitType].IsCivilUnit).Sum(x => x.UnitResourcesCost.GetTotal());

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

			var unitsCount = this.allUnits.Length;

			for (var i = 0; i < unitsCount; i++)
			{
				this.allUnits[i].Index = i;
			}

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
