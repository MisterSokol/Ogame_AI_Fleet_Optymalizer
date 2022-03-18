using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnitForces
	{
		int AliveUnitsCount { get; }
		int FleetSpeed { get; }
		List<Unit> UnitTypesRepresentatives { get; }

		void ResetToFullForces();
		bool HasUnitsLeft();
		void HitButDoNotUpdate(IUnitForces defenderUnit);
		Resources GetLostResources(bool includeAliveUnits = false);
		Resources GetDebrisResources(bool includeAliveUnits = false);
		long GetFleetResourcesCapacity();
		void EndBattle();
		void EndRound();
		Unit GetRandomAliveUnit(int aliveUnitsAvailableIndexes);
		double TacticalPower();
		Resources GetFlightCost(IInputData inputData, bool forceEveryFleetMaxSpeed = false);

		IUnitForces Copy();
		void PerformTacticalRetreat();
	}
}
