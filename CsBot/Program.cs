using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using CsBot.Interfaces;

namespace CsBot
{
	/// <summary>
	/// This program establishes a connection to irc server, joins a channel and greets every nickname that
	/// joins the channel
	/// </summary>
	class Program
	{
		static int Main (string[] args)
		{
			// Add them to the root command
			var rootCommand = new RootCommand { Description = "C# IrcBot" };

			// Create some options and a parser
			var useLocalSettings = new Option (
				aliases: new string[] { "--local-settings" },
				"Use local settings.json file",
				new Argument<bool> (defaultValue: false));

			rootCommand.AddOption (useLocalSettings);

			rootCommand.Handler = System.CommandLine.Invocation.CommandHandler.Create<bool> (StartIrcBot);

			// Parse the incoming args and invoke the handler
			return rootCommand.InvokeAsync (args).Result;
		}

		static int StartIrcBot (bool useLocalSettings)
			=> new IrcBot (new ConsoleLogger (), useLocalSettings).Run ();

		static void LoadCommands ()
		{
			var results = from t in Assembly.GetExecutingAssembly ().GetTypes ()
						  where t.GetInterfaces ().Contains (typeof (IIrcCommand))
						  select Activator.CreateInstance (t) as IIrcCommand;

			foreach (var obj in results)
				Shell.RegisterShellCommand (obj);

		}
	}
}
