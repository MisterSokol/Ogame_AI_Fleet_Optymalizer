using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces;
using SharpNeatLib.Maths;
using System;
using System.Runtime.CompilerServices;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Unit : ICloneable
	{
		private readonly int maxHP;
		private readonly int maxShieldValue;
		private readonly int maxShieldMinApplicableDamage;

		public UnitType UnitType;
		public Resources Debris;
		public int ResourcesCapacity;
		public Resources UnitResourcesCost;
		public int Index;
		public int MinApplicableDamage;
		public int hp;
		public int shieldValue;
		public int maxHpPercentage;

		public int HP 
		{
			get => hp;
			set
			{
				this.hp = value;
				this.maxHpPercentage = (int)((double)this.hp / this.maxHP * 100);
			}
		}
		public int ShieldValue 
		{
			get => this.shieldValue;
			set
			{
				this.shieldValue = value;
				this.MinApplicableDamage = (int)((double)this.shieldValue / 100);
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
			this.maxShieldMinApplicableDamage = this.MinApplicableDamage;
			this.maxShieldValue = maxShieldValue;
			this.Damage = damage;
			this.IsAlive = true;
			this.Speed = speed;
			this.FuelConsumption = fuelConsumption;
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
			this.MinApplicableDamage = this.maxShieldMinApplicableDamage;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
