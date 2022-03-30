using Newtonsoft.Json;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using System.IO;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class DataIOHandler : IDataIOHandler
	{
		private readonly string configrationDataPath = @"..\..\app_configuration.json";
		private readonly string inputDataPath = @"..\..\app_input.json";
		private readonly string outputDataPath = @"..\..\app_output.json";
		private readonly string gameDataPath = @"..\..\app_gameData.json";

		public IConfigurationData GetConfiguration()
		{
			return JsonConvert.DeserializeObject<ConfigurationData>(File.ReadAllText(configrationDataPath));
		}

		public IInputData GetInput()
		{
			return JsonConvert.DeserializeObject<InputData>(File.ReadAllText(inputDataPath));
		}

		public IGameData GetGameData()
		{
			var gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(gameDataPath)); ;

			gameData.LoadRapidFire();

			return gameData;
		}

		public void SaveOutput(IOutputData outputData, string path = null)
		{
			File.WriteAllText(path ?? outputDataPath, JsonConvert.SerializeObject(outputData));
		}
	}
}
