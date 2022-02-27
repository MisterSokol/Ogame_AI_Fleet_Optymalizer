namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IConfigurationData
	{
		// Evolutionary Algorithm configuration
		int GenerationSize { get; set; }
		int MaxNumberOfGenerationsWithoutFitnessImprovement { get; set; }
		int MutationChanceProbablityPercentage { get; set; }
		int ChanceOfMutationToZeroPercentage { get; set; }
		int ChanceOfMutationToMaxPercentage { get; set; }
		int MinMutationModificationPercentage { get; set; }
		int MaxMutationModificationPercentage { get; set; }

		int AttackerWinFitnessMultiplier { get; set; }
		int DrawFitnessMultiplier { get; set; }
		int DefenderWinFitnessMultiplier { get; set; }
		int ProfitResourcesFitnessMultiplier { get; set; }
		int FlightSpeedFitnessMultiplier { get; set; }
		int AttackerToDefenderMoreThan3FleetRatioPentaltyPercentage { get; set; }
		int AttackerToDefenderMoreThan5FleetRatioPentaltyPercentage { get; set; }
	}
}
