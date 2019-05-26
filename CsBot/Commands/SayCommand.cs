using System;
using System.Collections.Generic;
using CsBot.Interfaces;

namespace CsBot.Commands
{
	public class SayCommand : IIrcCommand
	{
		public List<string> ValidCommands
			=> new List<string> { "Say" };

		public void Say()
        {
            if (command.Length == endCommand + 1)
            {
                Say(m_addresser + ": What did you want " + settings.nick + " to say?");
            }
            else
            {
                string toSay = command.Substring(endCommand + 2).Trim();
                if (toSay.StartsWith("in"))
                {
                    string channel = GetChannel(toSay);
                    if (channel != null)
                    {
                        string toSayIn = toSay.Substring(toSay.IndexOf(channel) + channel.Length + 1);
                        Say(toSayIn, channel);
                    }
                }
                else
                {
                    Say(toSay, m_fromChannel);
                }
            }
        }
    }
}
