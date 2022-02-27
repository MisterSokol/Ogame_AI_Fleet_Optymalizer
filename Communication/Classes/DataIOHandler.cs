using Newtonsoft.Json;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using System.IO;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes
{
	public class DataIOHandler : IDataIOHandler
	{
		private readonly string configrationDataPath = @".\app_configuration.json";
		private readonly string inputDataPath = @".\app_configuration.json";
		private readonly string outputDataPath = @".\app_configuration.json";

		public IConfigurationData GetConfiguration()
		{
			return JsonConvert.DeserializeObject<ConfigurationData>(File.ReadAllText(configrationDataPath));
		}

		public IInputData GetInput()
		{
			return JsonConvert.DeserializeObject<InputData>(File.ReadAllText(inputDataPath));
		}

		public void SaveOutput(IOutputData outputData)
		{
			File.WriteAllText(outputDataPath, JsonConvert.SerializeObject(outputData));
		}
	}
}
