using System;
using CsBot.Interfaces;

namespace CsBot.Commands
{
    public class ApbCommand : IIrcCommand
    {
        public void Apb()
        {
            if (command.Length == endCommand + 1)
            {
                Say(m_addresser + ": Who do you want " + settings.nick + " to find?");
            }
            else
            {
                string toFind = command.Substring(endCommand + 2).Trim();
                if (!m_users.HasUser(toFind))
                {
                    Say(m_addresser + ": That person doesn't exist.");
                }
                else
                {
                    Console.WriteLine(m_addresser + " put out apb for " + toFind);
                    Say("/me sends out the blood hounds to find " + toFind + ".");
                }
            }
        }
    }
}
