using OGame_FleetOptymalizer_AI_ConsoleApp.AI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IOutputData
	{
		Individual WinnerIndividual { get; set; }
	}
}
