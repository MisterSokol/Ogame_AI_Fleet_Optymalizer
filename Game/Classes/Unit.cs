using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Unit : IUnit
	{
		private readonly int maxHP;
		private readonly int maxShieldValue;

		public UnitType UnitType { get; }
		public Resources Debris { get; }
		public int ResourcesCapacity { get; }
		public Resources UnitResourcesCost { get; }
		public int HP { get; private set; }
		public int ShieldValue { get; private set; }
		public int Damage { get; }
		public bool IsAlive { get; private set; }
		public int Speed { get; private set; }
		public int FuelConsumption { get; private set; }

		public Unit(
			UnitType unitType,
			Resources debris,
			int resourcesCapacity,
			Resources unitResourcesCost,
			int maxHP,
			int maxShieldValue,
			int damage,
			int speed,
			int fuelConsumption
			)
		{
			this.UnitType = unitType;
			this.Debris = debris;
			this.ResourcesCapacity = resourcesCapacity;
			this.UnitResourcesCost = unitResourcesCost;
			this.HP = maxHP;
			this.maxHP = maxHP;
			this.ShieldValue = maxShieldValue;
			this.maxShieldValue = maxShieldValue;
			this.Damage = damage;
			this.IsAlive = true;
			this.Speed = speed;
			this.FuelConsumption = fuelConsumption;
		}

		public Resources GetDebrisResources()
		{
			return this.Debris;
		}

		public long GetUnitResourcesCapacity()
		{
			return this.ResourcesCapacity;
		}

		public Resources GetUnitResourcesCost()
		{
			return this.UnitResourcesCost;
		}

		public void Reset()
		{
			this.HP = this.maxHP;
			this.ShieldValue = this.maxShieldValue;
			this.IsAlive = true;
		}

		public void RestoreShield()
		{
			this.ShieldValue = this.maxShieldValue;
		}

		public void TakeHit(Randomizer randomizer, IUnit enemyUnit)
		{
			if (!this.IsAlive || this.IsDamageLessOrEqualToOnePercentOfShieldValue(enemyUnit))
			{
				return;
			}

			if (enemyUnit.Damage > this.ShieldValue)
			{
				this.HP -= enemyUnit.Damage - this.ShieldValue;
				this.ShieldValue = 0;
			}
			else
			{
				this.ShieldValue -= enemyUnit.Damage;
			}

			if (this.HP > 0 && this.CheckIfExploded(randomizer))
			{
				this.HP = 0;
			}

			if (this.HP <= 0)
			{
				this.IsAlive = false;
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		private bool CheckIfExploded(Randomizer randomizer)
		{
			var percentOfMaxHP = (double)this.HP / this.maxHP * 100;

			return percentOfMaxHP < 70 && randomizer.CheckIfHitTheChance(100 - (int)percentOfMaxHP);
		}

		private bool IsDamageLessOrEqualToOnePercentOfShieldValue(IUnit enemyUnit)
		{
			return ((double)enemyUnit.Damage / this.ShieldValue * 100) <= 1.0;
		}
	}
}
