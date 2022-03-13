using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnit : ICloneable
	{
		UnitType UnitType { get; }
		Resources Debris { get; }
		int HP { get; }
		int MaxHpPercentage { get; }
		int ShieldValue { get; }
		int MinApplicableDamage { get; }
		int Damage { get; }
		bool IsAlive { get; }
		int Speed { get; }
		int FuelConsumption { get; }

		void TakeHit(Randomizer randomizer, int damage);
		void RestoreShield();
		Resources GetDebrisResources();
		long GetUnitResourcesCapacity();
		Resources GetUnitResourcesCost();
		void Reset();
	}
}
