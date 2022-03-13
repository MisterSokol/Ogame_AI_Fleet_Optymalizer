using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Unit : IUnit
	{
		private int hp;
		private int shieldValue;
		private bool isAlive;
		private int minApplicableDamage;
		private int maxHpPercentage;

		private readonly int maxHP;
		private readonly int maxShieldValue;

		public UnitType UnitType { get; }
		public Resources Debris { get; }
		public int ResourcesCapacity { get; }
		public Resources UnitResourcesCost { get; }
		public int HP 
		{
			get => hp;
			private set
			{
				this.hp = value;
				this.maxHpPercentage = this.hp / this.maxHP * 100;
			}
		}
		public int ShieldValue 
		{
			get => this.shieldValue;
			private set
			{
				this.shieldValue = value;
				this.minApplicableDamage = this.shieldValue / 100;
			}
		}
		public int Damage { get; }
		public bool IsAlive => this.isAlive;
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
			this.maxHP = maxHP;
			this.HP = maxHP;
			this.ShieldValue = maxShieldValue;
			this.maxShieldValue = maxShieldValue;
			this.Damage = damage;
			this.isAlive = true;
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
			this.isAlive = true;
		}

		public void RestoreShield()
		{
			this.ShieldValue = this.maxShieldValue;
		}

		public void TakeHit(Randomizer randomizer, int damage)
		{
			if (!this.isAlive || damage < this.minApplicableDamage)
			{
				return;
			}

			if (damage > this.shieldValue)
			{
				this.HP = this.hp - (damage - this.shieldValue);
				this.ShieldValue = 0;
			}
			else
			{
				this.ShieldValue =  this.shieldValue - damage;
			}

			if (this.hp > 0 && this.maxHpPercentage <= 70 && randomizer.CheckIfHitTheChance(100 - this.maxHpPercentage))
			{
				this.HP = 0;
			}

			if (this.hp <= 0)
			{
				this.isAlive = false;
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
