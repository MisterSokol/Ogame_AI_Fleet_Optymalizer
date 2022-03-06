using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class ConfigurationData : IConfigurationData
	{
		public int GenerationSize { get; set; }
		public int MaxNumberOfGenerationsWithoutFitnessImprovement { get; set; }
		public int MutationChanceProbablityPercentage { get; set; }
		public int ChanceOfMutationToZeroPercentage { get; set; }
		public int ChanceOfMutationToMaxPercentage { get; set; }
		public int MinMutationModificationPercentage { get; set; }
		public int MaxMutationModificationPercentage { get; set; }
		public int ThreadNumber { get; set; }

		public int AttackerWinFitnessMultiplier { get; set; }
		public int DrawFitnessMultiplier { get; set; }
		public int DefenderWinFitnessMultiplier { get; set; }
		public int ProfitResourcesFitnessMultiplier { get; set; }
		public int FlightSpeedFitnessMultiplier { get; set; }
		public int AttackerToDefenderMoreThan3FleetRatioPentaltyPercentage { get; set; }
		public int AttackerToDefenderMoreThan5FleetRatioPentaltyPercentage { get; set; }
		public int DeuteriumPriceMultiplier { get; set; }
		public int CrystalPriceMultiplier { get; set; }
	}
}
