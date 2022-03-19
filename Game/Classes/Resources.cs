using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Helpers;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes
{
	public struct Resources
	{
		public int Metal;
		public int Crystal;
		public int Deuterium;

		public Resources(int metal, int crystal, int deuterium)
		{
			this.Metal = metal;
			this.Crystal = crystal;
			this.Deuterium = deuterium;
		}

		public static Resources operator +(Resources a, Resources b)
		{
			return new Resources
			{
				Metal = a.Metal + b.Metal,
				Crystal = a.Crystal + b.Crystal,
				Deuterium = a.Deuterium + b.Deuterium
			};
		}

		public static Resources operator -(Resources a, Resources b)
		{
			return new Resources
			{
				Metal = a.Metal - b.Metal,
				Crystal = a.Crystal - b.Crystal,
				Deuterium = a.Deuterium - b.Deuterium
			};
		}

		public static Resources operator /(Resources a, int denominator)
		{
			return new Resources
			{
				Metal = a.Metal/denominator,
				Crystal = a.Crystal/denominator,
				Deuterium = a.Deuterium/denominator
			};
		}

		public static Resources Empty()
		{
			return new Resources()
			{
				Metal = 0,
				Crystal = 0,
				Deuterium = 0
			};
		}

		public Resources GetPercentage(int percentage)
		{
			return new Resources()
			{
				Metal = CalculationHelper.GetPercentageValue(this.Metal, percentage),
				Crystal = CalculationHelper.GetPercentageValue(this.Crystal, percentage),
				Deuterium = CalculationHelper.GetPercentageValue(this.Deuterium, percentage),
			};
		}

		public int GetTotal()
		{
			return this.Metal + this.Crystal + this.Deuterium;
		}

		public int GetTotalWorth(IConfigurationData configuration)
		{
			return this.Metal + this.Crystal * configuration.CrystalPriceMultiplier + this.Deuterium * configuration.DeuteriumPriceMultiplier;
		}

		public Resources GetResourcesBasedOnCapacity(long attackerResourcesCapacity)
		{
			if (attackerResourcesCapacity >= this.GetTotal())
			{
				return this;
			}

			var resourcesTaken = new Resources();

			for (int i = 3; i > 0; i--)
			{
				var amountToFairDistributePerResources = (int)Math.Round((double)(attackerResourcesCapacity - resourcesTaken.GetTotal()) / i);

				resourcesTaken.Metal = Math.Min(this.Metal, resourcesTaken.Metal  + amountToFairDistributePerResources);
				resourcesTaken.Crystal = Math.Min(this.Crystal, resourcesTaken.Crystal + amountToFairDistributePerResources);
				resourcesTaken.Deuterium = Math.Min(this.Deuterium, resourcesTaken.Deuterium + amountToFairDistributePerResources);
			}

			return resourcesTaken;
		}
	}
}
