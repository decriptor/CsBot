using System;
using CsBot.Interfaces;

namespace CsBot.Commands
{
    public class CaffeineCommand : IIrcCommand
    {
        public void Caffeine()
        {
            if (command.Length == endCommand + 1)
            {
                Say("/me walks over to " + m_addresser + " and gives them a shot of caffeine straight into the blood stream.");
            }
            else
            {
                int shots;
                if (!int.TryParse(command.Substring(endCommand + 2).Trim(), out shots))
                {
                    Say(m_addresser + ": I didn't understand, how many shots of caffeine did you want?");
                }
                else if (shots == 1)
                {
                    Say("/me walks over to " + m_addresser + " and gives them a shot of caffeine straight into the blood stream.");
                }
                else
                {
                    Say("/me walks over to " + m_addresser + " and gives them " + shots + " shots of caffeine straight into the blood stream.");
                }
            }

        }
    }
}
