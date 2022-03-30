using Newtonsoft.Json;
using OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes;
using System;

namespace OGame_FleetOptymalizer_AI_ConsoleApp
{
	class Program
	{
		static void Main()
		{
			var dataIOHandler = new DataIOHandler();

			var input = dataIOHandler.GetInput();
			var configuration = dataIOHandler.GetConfiguration();
			var gameData = dataIOHandler.GetGameData();

			var evolutionaryAlgorithm = new EvolutionaryAlgorithm();

			var output = evolutionaryAlgorithm.Process(configuration, input, gameData);

			dataIOHandler.SaveOutput(output);

			Console.WriteLine(JsonConvert.SerializeObject(output));
			Console.ReadLine();
		}
	}
}
