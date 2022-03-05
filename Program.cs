using Newtonsoft.Json;
using OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Classes;
using OGame_FleetOptymalizer_AI_ConsoleApp.Game.Enums;
using System;
using System.Collections.Generic;

namespace OGame_FleetOptymalizer_AI_ConsoleApp
{
	class Program
	{
		static void Main()
		{
			//CombatSimulationTest();

			var dataIOHandler = new DataIOHandler();

			var input = dataIOHandler.GetInput();
			var configuration = dataIOHandler.GetConfiguration();
			var gameData = dataIOHandler.GetGameData();

			var evolutionaryAlgorithm = new EvolutionaryAlgorithm();

			//var configuration = GetConfiguration();
			//var inputData = GetInputData();
			//var gameData = GetGameData();

			var output = evolutionaryAlgorithm.Process(configuration, input, gameData);

			dataIOHandler.SaveOutput(output);

			Console.WriteLine(JsonConvert.SerializeObject(output));
			Console.ReadLine();
		}

		private static IConfigurationData GetConfiguration()
		{
			return new ConfigurationData
			{
				GenerationSize = 10,
				MaxNumberOfGenerationsWithoutFitnessImprovement = 200,
				MutationChanceProbablityPercentage = 80,
				ChanceOfMutationToZeroPercentage = 5,
				ChanceOfMutationToMaxPercentage = 5,
				MinMutationModificationPercentage = 1,
				MaxMutationModificationPercentage = 20,

				AttackerWinFitnessMultiplier = 6,
				DrawFitnessMultiplier = 3,
				DefenderWinFitnessMultiplier = 1,
				ProfitResourcesFitnessMultiplier = 2,
				FlightSpeedFitnessMultiplier = 1,
				AttackerToDefenderMoreThan3FleetRatioPentaltyPercentage = 0,
				AttackerToDefenderMoreThan5FleetRatioPentaltyPercentage = 0
			};
		}

		public static void CombatSimulationTest()
		{
			var inputData = GetInputData();
			var gameData = GetGameData();
			var unitForcesFactory = new UnitForcesFactory();

			var attackerFleet = unitForcesFactory.Create(inputData, inputData.AttackerData, gameData);
			var defenderUnits = unitForcesFactory.Create(inputData, inputData.DefenderData, gameData);
			var combatSim = new CombatSimulator();

			var result = combatSim.RunSimulation(inputData, attackerFleet, defenderUnits);

			Console.WriteLine(JsonConvert.SerializeObject(result));
			Console.ReadLine();
		}

		private static InputData GetInputData()
		{
			return new InputData()
			{
				AttackerData = GetAttackerData(),
				DefenderData = GetDefenderData(),

				NumberOfSimulations = 50,
				PlunderPercentage = 50,
				FleetToDebrisPercentage = 30,
				CargoHyperspaceMultiplier = 5,
				CollectorIncreasedTradingShipCargoPercentage = 25,
			};
		}

		private static InputPlayerData GetDefenderData()
		{
			return new Communication.Interfaces.InputPlayerData()
			{
				Class = Game.Enums.PlayerClassType.Collector,
				WeaponLevel = 1,
				ShieldLevel = 1,
				ArmourLevel = 1,
				HyperspaceTechnologyLevel = 5,

				//SmallCargo = 5,
				//LargeCargo = 2,
				//LightFighter = 10,
				//HeavyFighter = 1,
				Cruiser = 1,

				MetalResources = 20000,
				CrystalResources = 10000,
				DeuteriumResources = 5000
			};
		}

		private static InputPlayerData GetAttackerData()
		{
			return new Communication.Interfaces.InputPlayerData()
			{
				Class = Game.Enums.PlayerClassType.General,
				WeaponLevel = 5,
				ShieldLevel = 4,
				ArmourLevel = 3,
				HyperspaceTechnologyLevel = 3,
				CombustionLevel = 10,
				ImpulseLevel = 9,
				HyperspaceLevel = 8,

				SmallCargo = 5,
				LargeCargo = 0,
				LightFighter = 0,
				HeavyFighter = 0,
				Cruiser = 2
			};
		}

		private static GameData GetGameData()
		{
			var gameData = new GameData();

			gameData.UnitsData = new Dictionary<UnitType, UnitGameData>();

			gameData.UnitsData[UnitType.SmallCargo] = new UnitGameData()
			{
				UnitType = UnitType.SmallCargo,
				IsFleet = true,
				IsCivilUnit = true,
				ResourcesCost = new Resources(2000, 2000, 0),
				BaseHP = 400,
				BaseShieldValue = 10,
				BaseDamage = 5,
				Capacity = 5000,
				FastGuns = new Dictionary<UnitType, int> { { UnitType.Probe, 5 }, { UnitType.SolarSatelite, 5 }, { UnitType.Crawler, 5 } },
				DriveTechnologies = new List<DriveTechnology>
				{
					new DriveTechnology(){ TechnologyType = DriveTechnologyType.Combustion, BaseSpeed = 5000, FuelConsumption = 10, MinLevelOfTechnologyTypeRequired = 0 },
					new DriveTechnology(){ TechnologyType = DriveTechnologyType.Impulse, BaseSpeed = 10000, FuelConsumption = 20, MinLevelOfTechnologyTypeRequired = 5 }
				}
			};

			gameData.UnitsData[UnitType.LargeCargo] = new UnitGameData()
			{
				UnitType = UnitType.LargeCargo,
				IsFleet = true,
				IsCivilUnit = true,
				ResourcesCost = new Resources(6000, 6000, 0),
				BaseHP = 1200,
				BaseShieldValue = 25,
				BaseDamage = 5,
				Capacity = 25000,
				FastGuns = new Dictionary<UnitType, int> { { UnitType.Probe, 5 }, { UnitType.SolarSatelite, 5 }, { UnitType.Crawler, 5 } },
				DriveTechnologies = new List<DriveTechnology>
				{
					new DriveTechnology(){ TechnologyType = DriveTechnologyType.Combustion, BaseSpeed = 7500, FuelConsumption = 50, MinLevelOfTechnologyTypeRequired = 0 },
				}
			};

			gameData.UnitsData[UnitType.LightFighter] = new UnitGameData()
			{
				UnitType = UnitType.LightFighter,
				IsFleet = true,
				IsCivilUnit = false,
				ResourcesCost = new Resources(3000, 1000, 0),
				BaseHP = 400,
				BaseShieldValue = 10,
				BaseDamage = 50,
				Capacity = 50,
				FastGuns = new Dictionary<UnitType, int> { { UnitType.Probe, 5 }, { UnitType.SolarSatelite, 5 }, { UnitType.Crawler, 5 } },
				DriveTechnologies = new List<DriveTechnology>
				{
					new DriveTechnology(){ TechnologyType = DriveTechnologyType.Combustion, BaseSpeed = 12500, FuelConsumption = 20, MinLevelOfTechnologyTypeRequired = 0 },
				}
			};

			gameData.UnitsData[UnitType.HeavyFighter] = new UnitGameData()
			{
				UnitType = UnitType.HeavyFighter,
				IsFleet = true,
				IsCivilUnit = false,
				ResourcesCost = new Resources(6000, 4000, 0),
				BaseHP = 1000,
				BaseShieldValue = 25,
				BaseDamage = 150,
				Capacity = 100,
				FastGuns = new Dictionary<UnitType, int> { { UnitType.SmallCargo, 3 }, { UnitType.Probe, 5 }, { UnitType.SolarSatelite, 5 }, { UnitType.Crawler, 5 } },
				DriveTechnologies = new List<DriveTechnology>
				{
					new DriveTechnology(){ TechnologyType = DriveTechnologyType.Impulse, BaseSpeed = 10000, FuelConsumption = 75, MinLevelOfTechnologyTypeRequired = 0 },
				}
			};

			gameData.UnitsData[UnitType.Cruiser] = new UnitGameData()
			{
				UnitType = UnitType.Cruiser,
				IsFleet = true,
				IsCivilUnit = false,
				ResourcesCost = new Resources(20000, 7000, 2000),
				BaseHP = 2700,
				BaseShieldValue = 50,
				BaseDamage = 400,
				Capacity = 800,
				FastGuns = new Dictionary<UnitType, int> { { UnitType.LightFighter, 6 }, { UnitType.Probe, 5 }, { UnitType.SolarSatelite, 5 }, { UnitType.Crawler, 5 }, { UnitType.RocketLauncher, 10 } },
				DriveTechnologies = new List<DriveTechnology>
				{
					new DriveTechnology(){ TechnologyType = DriveTechnologyType.Impulse, BaseSpeed = 15000, FuelConsumption = 300, MinLevelOfTechnologyTypeRequired = 0 },
				}
			};

			return gameData;
		}
	}
}
