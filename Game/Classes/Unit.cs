using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Unit : ICloneable
	{
		private int hp;
		private int shieldValue;
		private int minApplicableDamage;
		private int maxHpPercentage;

		private readonly int maxHP;
		private readonly int maxShieldValue;
		private readonly int maxShieldMinApplicableDamage;

		public UnitType UnitType;
		public Resources Debris;
		public int ResourcesCapacity;
		public Resources UnitResourcesCost;
		public int Index;

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
		public int Damage;
		public bool IsAlive;
		public int Speed;
		public int FuelConsumption;

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
			this.maxShieldMinApplicableDamage = this.minApplicableDamage;
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
			this.RestoreShield();
			this.IsAlive = true;
		}

		public void RestoreShield()
		{
			this.shieldValue = this.maxShieldValue;
			this.minApplicableDamage = this.maxShieldMinApplicableDamage;
		}

		public void TakeHit(Randomizer randomizer, int damage, IUnitForces unitForces)
		{
			if (!this.IsAlive || damage < this.minApplicableDamage)
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
				this.IsAlive = false;
				unitForces.MarkAsExplodedNextRound(this.Index);
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
