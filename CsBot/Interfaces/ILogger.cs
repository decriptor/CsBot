using System;

namespace CsBot.Interfaces
{
	public interface ILogger
	{
		void Log (LogLevel level, string message);
		void Log (Exception ex);
	}
}
