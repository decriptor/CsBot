using System;
using CsBot.Interfaces;

namespace CsBot.Commands
{
    public class EmoteCommand : IIrcCommand
    {
        public void Emote()
        {
            if (command.Length == endCommand + 1)
            {
                Say(m_addresser + ": What did you want " + settings.nick + " to emote?");
            }
            else
            {
                string toEmote = command.Substring(endCommand + 2).Trim();
                if (toEmote.StartsWith("in"))
                {
                    string channel = GetChannel(toEmote);
                    if (channel != null)
                    {
                        string toEmoteIn = toEmote.Substring(toEmote.IndexOf(channel) + channel.Length + 1);
                        Say("/me " + toEmoteIn, channel);
                    }
                }
                else
                {
                    Say("/me " + toEmote);
                }
            }
        }
    }
}
