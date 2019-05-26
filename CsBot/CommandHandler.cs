using System;
using System.IO;
using System.Linq;

using CsBot.Models;

namespace CsBot
{
	class CommandHandler
	{
		IrcBot Bot { get; }
		Settings settings => Bot.Settings;

		/*private static string settings.nick = "Be|\\|der";
        private const string settings.command_start = "~";
        private static string settings.channels[0].name = "#pyrous";
        private static string settings.channels[0].name2 = "#CsBot";
        private static string m_fromChannel = settings.channels[0].name;
        */
		static string m_fromChannel;
		public StreamWriter writer => Bot.Writer;
		public StreamReader reader => Bot.Reader;
		static string m_addresser = "";
		static Users m_users = new Users ();
		Random random = new Random ();


		public CommandHandler (IrcBot bot)
		{
			Bot = bot;
		}

		/// <summary>
		/// Make the Bot say something in a specific channel.
		/// </summary>
		/// <param name="message">String to Say.</param>
		/// <param name="Channel">Channel to talk in</param>
		public void Say (string message, string Channel)
		{
			if (message.StartsWith ("/me"))
				message = "\x1" + message.Replace ("/me", "ACTION") + "\x1";

			if (message.EndsWith ("me"))
				message = message.Replace (" me", " " + m_addresser);

			if (message.Contains (" {nick} "))
				message = message.Replace (" {nick} ", " me ");

			if (message.Contains (" me "))
				message = message.Replace (" me ", " " + m_addresser + " ");

			if (message.Contains (" " + settings.nick + " "))
				message = message.Replace (" " + settings.nick + " ", " me ");

			Console.WriteLine ($"PRIVMSG {Channel} :{message}");
			writer.WriteLine ($"PRIVMSG {Channel} :{message}");
			writer.Flush ();
		}

		/// <summary>
		/// Make the Bot say something in a specific channel.
		/// </summary>
		/// <param name="message">String to Say.</param>
		void Say (string message)
			=> Say (message, m_fromChannel);

		string GetChannel (string input)
		{
			var parts = input.Split (' ');
			foreach (var part in parts) {
				if (part.StartsWith ("#"))
					return part;
			}

			return null;
		}

		public void HandleMessage (string command, string fromChannel, string addresser)
		{
			Bot.Logger.Log (LogLevel.Info, $"Handling message: {command} : {fromChannel} : {addresser}");

			m_fromChannel = fromChannel;
			m_addresser = addresser;

			int endCommand = command.IndexOf (" ") - 1;
			if (endCommand < 0)
				endCommand = command.Length - 1;

			string fixedCommand = command.Substring (1, endCommand);

			if (fixedCommand.StartsWith (settings.command_start)) { //If present remove leading command, otherwise log it.
				fixedCommand = fixedCommand.TrimStart (settings.command_start.ToCharArray ());
				command = command.TrimStart (settings.command_start.ToCharArray ());
			} else {
				fixedCommand = "";
			}

			if (command.Length == endCommand + 1) {
				if (fixedCommand.StartsWith ("s")) {
					fixedCommand = fixedCommand.Substring (1);
					command = "s " + fixedCommand;
					endCommand = 1;
				}
			}

			if (fixedCommand.StartsWith ("1")) {
				fixedCommand = "say";
				command = command.Replace ("1", "say in " + settings.channels[0].name);
				endCommand = fixedCommand.Length;
			}

			if (fixedCommand.StartsWith ("2")) {
				fixedCommand = "emote";
				command = command.Replace ("2", "emote in " + settings.channels[0].name);
				endCommand = fixedCommand.Length;
			}

			switch (fixedCommand) {
			//case "insult":
			//	InsultCommand.Insult ();
			//	break;
			//case "quote":
			//	QuoteCommand.Quote ();
			//	break;
			//case "praise":
			//	PraiseCommand.Praise ();
			//	break;
			//case "apb":
			//	ApbCommand.Apb ();
			//	break;
			//case "caffeine":
			//	CaffeineCommand.Caffeine ();
			//	break;
			case "say":
				SayCommand.Say ();
				break;
			//case "emote":
			//	EmoteCommand.Emote ();
			//	break;
			//case "roll":
			//	Farkle.Roll ();
			//	break;
			//case "rps":
			//	RoShamBoGame.PRS (command);
			//	break;
			//case "farklehelp":
			//	Farkle.Help ();
			//	break;
			//case "farkleforfeit":
			//	Farkle.Forfeit ();
			//	break;
			//case "farklescore":
			//	Farkle.Score ();
			//	break;
			//case "farkle":
			//	Farkle.FarkleFarkle ();
			//	break;
			//case "joinfarkle":
			//	Farkle.Join ();
			//	break;
			//case "s":
			//	StringReplaceCommand.Replace ();
			//	break;
			//case "getnext":
			//	RssFeedCommand.Next ();
			//	break;
			//case "getmore":
			//	RssFeedCommand.More ();
			//	break;
			//case "getfeeds":
			//	RssFeedCommand.Feeds ();
			//	break;
			default:
				Bot.Logger.Log (LogLevel.Info, $"\n{fixedCommand}");
				break;
			}
		}

		public void UpdateUserName (string origUser, string newUser)
		{
			if (m_users.HasUser (origUser)) {
				User tempUser = m_users[origUser];
				m_users.RemoveUser (origUser);
				m_users.AddUser (newUser, tempUser);
				Console.WriteLine ($"Updated username from {origUser} to {newUser}");
			} else if (newUser != settings.nick) {
				m_users.AddUser (newUser);
			}
		}

		public void ParseUsers (string usersInput)
		{
			Console.WriteLine ("Users Input " + usersInput);
			var users = usersInput.Substring (usersInput.LastIndexOf (":") + 1).Split (" ".ToCharArray ());

			for (int i = 0; i < users.Length; i++) {
				if (users[i] == "") continue;

				if (users[i] != settings.nick && !m_users.HasUser (users[i])) {
					if (users[i].StartsWith ("@")) {
						m_users.AddUser (users[i].Substring (1));
						Console.WriteLine ($"Found user {users[i].Substring (1)}");
					} else {
						m_users.AddUser (users[i]);
						Console.WriteLine ($"Found user {users[i]}");
					}
				}
			}

			foreach (var user in m_users)
				Console.WriteLine ($"Users  {user}");
		}

		public void AddUser (string userName)
		{
			if (!m_users.HasUser (userName))
				m_users.AddUser (userName);
		}

		public void LastMessage (string user, string inputLine, string fromChannel)
		{
			if (m_users.HasUser (user)) {
				var message = inputLine.Substring (inputLine.LastIndexOf (fromChannel + " :") + fromChannel.Length + 2);
				m_users.AddUserLastMessage (user, message);
			}
		}
	}
}
