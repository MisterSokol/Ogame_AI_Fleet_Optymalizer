using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers
{
	public static class CalculationHelper
	{
		public static int GetPercentageValue(int value, int percentage)
		{
			return (int)Math.Round(value * (double)percentage / 100);
		}

		public static int GetDistanceBetweenPlayers(IInputData inputData)
		{
			if (inputData.AttackerData.GalaxyNumber != inputData.DefenderData.GalaxyNumber)
			{
				return 20000 * Math.Abs(inputData.AttackerData.GalaxyNumber - inputData.DefenderData.GalaxyNumber);
			}

			if (inputData.AttackerData.SystemNumber != inputData.DefenderData.SystemNumber)
			{
				return 2700 + 95 * Math.Abs(inputData.AttackerData.SystemNumber - inputData.DefenderData.SystemNumber);
			}

			if (inputData.AttackerData.PositionNumber != inputData.DefenderData.PositionNumber)
			{
				return 1000 + 5 * Math.Abs(inputData.AttackerData.PositionNumber - inputData.DefenderData.PositionNumber);
			}

			return 5;
		}
	}
}
