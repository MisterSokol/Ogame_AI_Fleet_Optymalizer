using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class GameData : IGameData
	{
		public Dictionary<UnitType, UnitGameData> UnitsData { get; set; }
		public int[,] RapidFire { get; private set; }

		public void LoadRapidFire()
		{
			var tableSize = Enum.GetValues(typeof(UnitType)).Length;

			this.RapidFire = new int[tableSize, tableSize];

			for (var i = 0; i < tableSize; i++)
			{
				for (var j = 0; j < tableSize; j++)
				{
					var hasRapidFire = this.UnitsData[(UnitType)i].FastGuns.TryGetValue((UnitType)j, out var rapidFireValue);

					this.RapidFire[i, j] = hasRapidFire
						? rapidFireValue
						: 0;
				}
			}
		}
	}
}
