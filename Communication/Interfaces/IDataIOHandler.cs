using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGame_FleetOptymalizer_AI_ConsoleApp.Communication.Interfaces
{
	public interface IDataIOHandler
	{
		IInputData GetInput();
		IConfigurationData GetConfiguration();
		void SaveOutput(IOutputData outputData);
	}
}
