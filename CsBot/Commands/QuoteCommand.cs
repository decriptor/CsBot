using CsBot.Interfaces;

namespace CsBot.Commands
{
    public class QuoteCommand : IIrcCommand
    {
        public void Quote()
        {
            if (settings.quotes != null && settings.quotes.Length > 0)
            {
                Say(settings.quotes[random.Next(0, 10000) % settings.quotes.Length]);
            }
            else
            {
                Say("Hey, the blues. The tragic sound of other people's suffering. Thant's kind of a pick-me-up.");
            }
        }
    }
}
