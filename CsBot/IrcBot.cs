using System;
using System.Configuration;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

using CsBot.Interfaces;
using CsBot.Models;

namespace CsBot
{
	class IrcBot
	{
		//private static string SETTINGS_FILE = "settings.json";
		readonly string SETTINGS_FILE = ConfigurationManager.AppSettings["Site"];


		object stream;
		// StreamWriter is declared here so that PingSender can access it
		public StreamWriter Writer { get; private set; }
		public StreamReader Reader { get; private set; }

		public PingSender Ping { get; private set; }

		//public Users Users { get; }

		CommandHandler ch;
		TcpClient ircClient;
		bool isUnderscoreNick;

		readonly SettingsManager settingsManager;
		public Settings Settings => settingsManager.Settings;

		public ILogger Logger { get; }

		public IrcBot (ILogger logger, bool useLocalSettings)
		{
			settingsManager = new SettingsManager (useLocalSettings);
			Logger = logger ?? new ConsoleLogger ();
		}

		void OpenConnection ()
		{
			Logger.Log (LogLevel.Debug, $"Connecting to {Settings.server}:{Settings.port}");
			try {
				ircClient = new TcpClient ();
				ircClient.Connect (Settings.server, Settings.port);
				if (Settings.secure == "1") {
					stream = new SslStream (ircClient.GetStream (), true, new RemoteCertificateValidationCallback (settingsManager.ValidateServerCertificate));
					var sslStream = (SslStream)stream;
					sslStream.AuthenticateAsClient (Settings.server);
				} else
					stream = ircClient.GetStream ();
			} catch (Exception ex) {
				Logger.Log (LogLevel.Error, $"Connection Failed to {Settings.server}:{Settings.port}");
				throw ex;
			}

			Reader = new StreamReader ((Stream)stream);
			Writer = new StreamWriter ((Stream)stream);
		}

		void CloseConnection ()
		{
			Reader?.Close ();
			Writer?.Close ();

			stream = null;

			ircClient.Close ();
			ircClient = null;
		}

		public int Run ()
		{
			settingsManager.LoadRemoteSettings ("SETTINGS_FILE");

			try {
				//FileStream setting_file = new FileStream(SETTINGS_FILE, FileMode.Open);

				OpenConnection ();

				Logger.Log (LogLevel.Info, Settings.user);

				// Start PingSender thread
				Ping = new PingSender (this);
				Ping.Start ();

				Writer.WriteLine (Settings.user);
				Writer.Flush ();
				Writer.WriteLine ($"NICK {Settings.nick}");
				Writer.Flush ();
				Writer.WriteLine ($"PASS {Settings.password}");
				Writer.Flush ();
				Logger.Log (LogLevel.Info, $"NICK {Settings.nick}");

				ch = new CommandHandler (this);
				//writer.WriteLine("JOIN " + settings.channels[0].name + " " + KEY);
				//writer.WriteLine("JOIN " + settings.channels[0].name);
				//writer.Flush();
				//writer.WriteLine("JOIN " + settings.channels[0].name2);
				//writer.Flush();

				var fromChannel = Settings.channels[0].name;
				EventLoop (fromChannel, joined1: false, joined2: false, identified: true);
			} catch (Exception ex) {
				foreach (Channel channel in Settings.channels)
					ch.HandleMessage ($":{Settings.command_start}say Awe, Crap!", channel.name, "self");

				// Show the exception, sleep for a while and try to establish a new connection to irc server
				Logger.Log (ex);
				Task.Delay (5000);

				Run ();
			} finally {
				CloseConnection ();
			}

			return 0;
		}

		void CloseProgram ()
		{
			CloseConnection ();
			Environment.Exit (0);
		}

		void EventLoop(string fromChannel, bool joined1, bool joined2, bool identified)
        {
			var addresser = string.Empty;

			while (true) {
				var inputLine = Reader.ReadLine ();
				string parsedLine = null;

				foreach (var channel in Settings.channels) {
					if (inputLine.Contains (channel.name))
						fromChannel = inputLine.Substring (inputLine.IndexOf ("#")).Split (' ')[0];

					if (inputLine.Contains (Settings.nick + " = " + channel.name))
						ch.ParseUsers (inputLine);

					if (joined1 && !inputLine.EndsWith (fromChannel)) {
						//parsedLine = inputLine.Substring(inputLine.IndexOf(m_fromChannel) + m_fromChannel.Length + 1);
						if (!inputLine.EndsWith (channel.name) && (parsedLine == null || !parsedLine.StartsWith (":" + Settings.command_start)))
							parsedLine = inputLine.Substring (inputLine.IndexOf (fromChannel) + channel.name.Length + 1).Trim ();
					}
				}

				if (!joined1 || !joined2)
					Logger.Log (LogLevel.Info, inputLine);

				if (inputLine.Contains ("NICK :")) {
					string origUser = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
					string newUser = inputLine.Substring (inputLine.IndexOf ("NICK :") + 6);
					ch.UpdateUserName (origUser, newUser);
				}

				if (inputLine.EndsWith ($"JOIN {fromChannel}")) {
					// Parse nickname of person who joined the channel
					var nickname = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
					if (nickname == Settings.nick) {
						if (fromChannel == Settings.channels[0].name)
							joined1 = true;
						else if (fromChannel == Settings.channels[1].name)
							joined2 = true;
						ch.HandleMessage ($":{Settings.command_start}say I'm back baby!", fromChannel, addresser);
						continue;
					}

					// Welcome the nickname to channel by sending a notice
					Writer.WriteLine ($"NOTICE {nickname}: Hi {nickname} and welcome to {fromChannel} channel!");
					ch.HandleMessage (":" + Settings.command_start + "say " + nickname + ": Hi and welcome to " + fromChannel + " channel!", fromChannel, addresser);
					ch.AddUser (nickname);
					Writer.Flush ();
					// Sleep to prevent excess flood
					Task.Delay (2000);
				} else if (inputLine.StartsWith (":NickServ") && inputLine.Contains ("You are now identified")) {
					identified = true;
					Console.WriteLine (inputLine);
				} else if (inputLine.Contains ("!") && inputLine.Contains (" :" + Settings.command_start + "quit")) {
					addresser = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
					bool useChannel = false;
					if (inputLine.IndexOf ("#") >= 0) {
						useChannel = true;
						fromChannel = inputLine.Substring (inputLine.IndexOf ("#")).Split (' ')[0];
					}

					if (Settings.admins != null && Array.IndexOf (Settings.admins, addresser) >= 0) {
						foreach (Channel channel in Settings.channels) {
							ch.HandleMessage (":" + Settings.command_start + "say Awe, Crap!", channel.name, addresser);
						}
						Ping.Stop ();
						CloseProgram ();
					} else {
						ch.Say ("You don't have permissions.", useChannel ? fromChannel : addresser);
					}
				} else if (inputLine.Contains ("!") && inputLine.Contains (" :" + Settings.command_start + "reload")) {
					addresser = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
					bool useChannel = false;
					if (inputLine.IndexOf ("#") >= 0) {
						useChannel = true;
						fromChannel = inputLine.Substring (inputLine.IndexOf ("#")).Split (' ')[0];
					}

					if (Settings.admins != null && Array.IndexOf (Settings.admins, addresser) >= 0) {
						settingsManager.LoadRemoteSettings (SETTINGS_FILE);
						//setting_file = new FileStream(SETTINGS_FILE, FileMode.Open);
						//settings = (Settings)js.ReadObject(setting_file);
						//setting_file.Close();
						//setting_file = null;
						ch.Say ("Reloaded settings from web service.", useChannel ? fromChannel : addresser);
					} else {
						ch.Say ("You don't have permissions.", useChannel ? fromChannel : addresser);
					}
				} else if (inputLine.Contains (Settings.command_start) && parsedLine != null && parsedLine.StartsWith (":" + Settings.command_start)) {
					addresser = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
					fromChannel = inputLine.Substring (inputLine.IndexOf ("#")).Split (' ')[0];
					ch.HandleMessage (parsedLine, fromChannel, addresser);
				} else if (inputLine.StartsWith ("PING :")) {
					Writer.WriteLine ("PONG :" + inputLine.Substring (inputLine.IndexOf (":") + 1));
					Writer.Flush ();
				} else if (inputLine.Contains ("PONG") && (!joined1 || !joined2)) {
					if (isUnderscoreNick) {
						Writer.WriteLine ("PRIVMSG NickServ :ghost " + Settings.nick + " " + Settings.password);
						Writer.WriteLine ("NICK " + Settings.nick);
						Logger.Log (LogLevel.Info, $"NICK {Settings.nick}");
						ch.HandleMessage (":" + Settings.command_start + "say identify " + Settings.password, "NickServ", Settings.nick);
						isUnderscoreNick = false;
					} else {
						for (int i = 0; i < Settings.channels.Count; i++) {
							if (Settings.channels[i].key != "")
								Writer.WriteLine ("JOIN " + Settings.channels[i].name + " " + Settings.channels[i].key);
							else {
								Writer.WriteLine ("JOIN " + Settings.channels[i].name);
							}
							Writer.Flush ();
						}
					}
				} else if (inputLine.Contains ("PONG") && (joined1 || joined2) && !identified) {
					ch.HandleMessage (":" + Settings.command_start + "say identify " + Settings.password, "NickServ", addresser);
				} else if (inputLine.Contains (":Nickname is already in use.")) {
					Logger.Log (LogLevel.Info, "Reopening with _ nick.");
					CloseConnection ();
					OpenConnection ();
					Logger.Log (LogLevel.Info, Settings.user);
					// Start PingSender thread
					Ping = new PingSender (this);
					Ping.Start ();
					Writer.WriteLine (Settings.user);
					Writer.Flush ();
					Writer.WriteLine ("NICK _" + Settings.nick);
					Logger.Log (LogLevel.Info, $"NICK _{Settings.nick}");
					ch = new CommandHandler (this);
					isUnderscoreNick = true;
				} else if (inputLine.Contains (Settings.nick) && inputLine.Contains ("PRIVMSG") && (inputLine.Contains ("rock") || inputLine.Contains ("paper") || inputLine.Contains ("scissors"))) {
					Logger.Log (LogLevel.Info, inputLine);
					addresser = inputLine.Substring (inputLine.IndexOf (":") + 1, inputLine.IndexOf ("!") - inputLine.IndexOf (":") - 1);
					var choice = inputLine.Substring (inputLine.LastIndexOf (":") + 1);
					ch.DirectRoShamBo (choice);

				} else if (inputLine.Contains (Settings.nick) && inputLine.Contains ("PRIVMSG") && inputLine.Contains (":" + Settings.command_start)) {
					Logger.Log (LogLevel.Info, $"PrivateMessage: {inputLine}");
					addresser = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
					var command = inputLine.Substring (inputLine.LastIndexOf (":" + Settings.command_start));
					ch.HandleMessage (command, addresser, addresser);
				} else {
					if (inputLine.Contains ("PRIVMSG") && inputLine.Contains ("!")) {
						var userName = inputLine.Substring (1, inputLine.IndexOf ("!") - 1);
						ch.LastMessage (userName, inputLine, fromChannel);
					}
				}
			}
		}
	}
}

