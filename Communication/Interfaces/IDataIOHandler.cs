namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IDataIOHandler
	{
		IInputData GetInput();
		IConfigurationData GetConfiguration();
		IGameData GetGameData();
		void SaveOutput(IOutputData outputData, string path = null);
	}
}
