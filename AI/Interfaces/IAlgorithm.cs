using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.AI.Interfaces
{
	public interface IAlgorithm
	{
		IOutputData Process(IConfigurationData configuration, IInputData input, IGameData gameData);
	}
}
