using System;
using CsBot.Interfaces;

namespace CsBot.Commands
{
	public class StringReplaceCommand : IIrcCommand
	{
		public StringReplaceCommand ()
		{
		}

		public void Replace ()
		{
			if (command.Length == endCommand + 1) {
				Say (m_addresser + ": What did you want " + settings.nick + " to replace?");
			} else if (command.IndexOf ("/") == -1) {
				Say (m_addresser + ": Usage is ~s/<wrong phrase>/<corrected phrase>/");
			} else {
				string toReplace = command.Substring (command.IndexOf ("/") + 1, command.Substring (command.IndexOf ("/") + 1).IndexOf ("/"));
				string withString = command.Substring (command.IndexOf ("/", command.IndexOf (toReplace))).Replace ("/", "");
				string lastSaid = m_users.getUserMessage (m_addresser);
				lastSaid = lastSaid.Replace (toReplace, withString);
				Say (m_addresser + " meant: " + lastSaid);
			}

		}
	}
}
