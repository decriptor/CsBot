using System.Threading;
using System.Threading.Tasks;

namespace CsBot
{
    /// <summary>
    /// Class that sends PING to irc server every 15 seconds
    /// </summary>
    class PingSender
    {
        const string PING = "PING :";
		const int PING_RATE = 15000;
        readonly Thread pingSender;

		IrcBot Bot { get; }

        // Empty constructor makes instance of Thread
        public PingSender(IrcBot bot)
        {
			Bot = bot;
            pingSender = new Thread(new ThreadStart(Run));
        }

		// Starts the thread
		public void Start () => pingSender.Start ();

		// Kills the thead
		public void Stop () => pingSender.Abort ();

		// Send PING to irc server every 15 seconds
		public void Run()
        {
            while (true)
            {
                Bot.Writer.WriteLine(PING + Bot.Settings.server);
                Bot.Writer.Flush();
				Task.Delay (PING_RATE);
            }
        }
    }
}
