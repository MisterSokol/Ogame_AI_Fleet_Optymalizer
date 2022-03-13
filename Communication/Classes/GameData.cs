using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class GameData : IGameData
	{
		public Dictionary<UnitType, UnitGameData> UnitsData { get; set; }
	}
}
