using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Interfaces
{
	public interface IUnitForces
	{
		int FleetSpeed { get; }
		List<IUnit> UnitTypesRepresentatives { get; }

		void ResetToFullForces();
		bool HasUnitsLeft();
		void HitButDoNotUpdate(IUnitForces defenderUnit);
		Resources GetLostResources(bool includeAliveUnits = false);
		Resources GetDebrisResources(bool includeAliveUnits = false);
		long GetFleetResourcesCapacity();
		void EndBattle();
		void EndRound();
		IUnit GetRandomAliveUnit();
		double TacticalPower();
		Resources GetFlightCost(Communication.Interfaces.IInputData inputData);

		IUnitForces Copy();
	}
}
