using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IGameData
	{
		Dictionary<UnitType, UnitGameData> UnitsData { get; set; }

		void FillEmptyDataWithZeros();
	}
}
