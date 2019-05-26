using System;

using CsBot.Interfaces;

namespace CsBot
{
	public class ConsoleLogger : ILogger
	{
		public void Log (LogLevel level, string message)
		{
			if (level == LogLevel.Error)
				Console.WriteLine ($"[Error] {message}");

			Console.WriteLine (message);
		}

		public void Log (Exception ex)
		{
			Console.WriteLine ($"{ex.Source}: {ex.Message}");
			Console.WriteLine ($"{ex.StackTrace}");
		}
	}
}
