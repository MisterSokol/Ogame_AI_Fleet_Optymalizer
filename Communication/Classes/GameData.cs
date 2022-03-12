using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class GameData : IGameData
	{
		public Dictionary<UnitType, UnitGameData> UnitsData { get; set; }

		public void FillEmptyDataWithZeros()
		{
			var allUnitTypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>();

			foreach (var unitData in this.UnitsData.Values)
			{
				foreach (var unitType in allUnitTypes)
				{
					if (!unitData.FastGuns.ContainsKey(unitType))
					{
						unitData.FastGuns[unitType] = 0;
					}
				}
			}
		}
	}
}
