using System;
using CsBot.Interfaces;

namespace CsBot.Commands
{
	public class InsultCommand : IIrcCommand
	{
		public InsultCommand ()
		{
		}

		public void Insult ()
		{
			if (command.Length == endCommand + 1) {
				Say (m_addresser + ": Who do you want " + settings.nick + " to insult?");
			} else {
				string toInsult = command.Substring (endCommand + 2).Trim ();
				if (!m_users.HasUser (toInsult)) {
					Say (m_addresser + ": That person doesn't exist.");
				} else {
					Bot.Logger.Log (LogLevel.Info, $"{m_addresser} insulted {toInsult}.");
					if (settings.insults != null && settings.insults.Length > 0) {
						Say ($"/me {string.Format (settings.insults[random.Next (0, 10000) % settings.insults.Length], toInsult)}");
					} else {
						Say ($"/me thinks {toInsult} is screwier than his Aunt Rita, and she's a screw.");
					}
				}
			}
		}
	}
}
