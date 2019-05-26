using System;
using CsBot.Interfaces;

namespace CsBot.Commands
{
    public class PraiseCommand : IIrcCommand
    {
        public void Praise()
        {
            if (command.Length == endCommand + 1)
            {
                Say(m_addresser + ": Who do you want " + settings.nick + " to praise?");
            }
            else
            {
                string toPraise = command.Substring(endCommand + 2).Trim();
                if (!m_users.HasUser(toPraise))
                {
                    Say(m_addresser + ": That person doesn't exist.");
                }
                else
                {
                    Console.WriteLine(m_addresser + " praised " + toPraise + ".");
                    if (settings.praises != null && settings.praises.Length > 0)
                    {
                        Say("/me " + string.Format(settings.praises[random.Next(0, 10000) % settings.praises.Length], toPraise));
                    }
                    else
                    {
                        Say("/me thinks " + toPraise + " is very smart.");
                    }
                }
            }
        }
    }
}
