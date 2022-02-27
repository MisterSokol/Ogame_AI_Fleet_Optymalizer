using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public static class Randomizer
	{
		private static Random random = new Random();

		public static int RandomFromRange(int min, int max)
		{
			return random.Next(min, max + 1);
		}

		public static long RandomFromRange(long min, long max)
		{
			byte[] buf = new byte[8];
			random.NextBytes(buf);
			long longRand = BitConverter.ToInt64(buf, 0);

			return Math.Abs(longRand % (max + 1 - min)) + min;
		}

		public static bool CheckIfHitTheChance(int percentage)
		{
			if (percentage > 100 || percentage < 0)
			{
				throw new Exception("Percentage value of CheckIfHitTheChance must be from <0, 100> range");
			}

			return RandomFromRange(0, 100) < percentage;
		}

		public static bool RandomTrueFalse()
		{
			return RandomFromRange(0, 1) == 1;
		}

		public static int GetRandomPercentageValueOfNumber(int number)
		{
			return (int)((double)RandomFromRange(0, 100) * number / 100 );
		}
	}
}
