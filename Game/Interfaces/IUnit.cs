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
		int ShieldValue { get; }
		int Damage { get; }
		bool IsAlive { get; }
		int Speed { get; }
		int FuelConsumption { get; }

		void TakeHit(Randomizer randomizer, IUnit enemyUnit);
		void RestoreShield();
		Resources GetDebrisResources();
		long GetUnitResourcesCapacity();
		Resources GetUnitResourcesCost();
		void Reset();
	}
}
