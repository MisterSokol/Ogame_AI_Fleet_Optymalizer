using SharpNeatLib.Maths;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public class Randomizer
	{
		private FastRandom random = new FastRandom();

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
			return random.Next0to100() < percentage;
		}

		public bool RandomTrueFalse()
		{
			return random.NextBool();
		}

		public int GetRandomPercentageValueOfNumber(int number)
		{
			return (int)Math.Round((double)random.Next0to100() * number / 100 );
		}
	}
}
