using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Randomizer
	{
		private Random random = new Random();

		public int RandomFromRange(int min, int max)
		{
			return random.Next(min, max + 1);
		}

		public long RandomFromRange(long min, long max)
		{
			byte[] buf = new byte[8];
			random.NextBytes(buf);
			long longRand = BitConverter.ToInt64(buf, 0);

			return Math.Abs(longRand % (max + 1 - min)) + min;
		}

		public bool CheckIfHitTheChance(int percentage)
		{
			return RandomFromRange(0, 100) < percentage;
		}

		public bool RandomTrueFalse()
		{
			return RandomFromRange(0, 1) == 1;
		}

		public int GetRandomPercentageValueOfNumber(int number)
		{
			return (int)Math.Round((double)RandomFromRange(0, 100) * number / 100 );
		}
	}
}
