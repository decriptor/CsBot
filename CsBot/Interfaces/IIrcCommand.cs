
using System.Collections.Generic;

namespace CsBot.Interfaces
{
	interface IIrcCommand
	{
		string Name { get; }
		string Description { get; }
		List<string> ValidCommands { get; }
	}
}
