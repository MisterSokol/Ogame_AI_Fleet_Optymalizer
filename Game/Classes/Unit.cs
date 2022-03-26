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
		public int hp;
		public int shieldValue;
		public int minApplicableDamage;
		public int maxHpPercentage;

		public readonly int maxHP;
		public readonly int maxShieldValue;
		public readonly int maxShieldMinApplicableDamage;

		public UnitType UnitType;
		public Resources Debris;
		public int ResourcesCapacity;
		public Resources UnitResourcesCost;
		public int Index;

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
				this.minApplicableDamage = (int)((double)this.shieldValue / 100);
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

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
