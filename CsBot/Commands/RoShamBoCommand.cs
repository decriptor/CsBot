
using CsBot.Interfaces;

namespace CsBot.Commands
{
	class RoShamBoCommand : IIrcCommand
	{
		private enum RoShamBo { Rock, Paper, Scissors };

		public static void DirectRoShamBo (string choice)
		{
			switch (choice) {
			case "rock":
				m_users.RPSValue (m_addresser, RoShamBo.Rock);
				break;
			case "paper":
				m_users.RPSValue (m_addresser, RoShamBo.Paper);
				break;
			case "scissors":
				m_users.RPSValue (m_addresser, RoShamBo.Scissors);
				break;
			default:
				Say ("/me whispers something to " + m_addresser + ".");
				Say ("Valid options are either rock, paper, or scissors.", m_addresser);
				break;
			}

			HandleMessage (":" + settings.command_start + "rps", m_fromChannel, m_addresser);
		}

		public void RPS (string command)
		{
			if (command.Length == endCommand + 1 && m_users.RPSValue (m_addresser) == -2) {
				Say ("/me whispers something to " + m_addresser + ".");
				Say ("Would you like to throw rock, paper, or scissors?", m_addresser);
			} else if (m_users.RPSValue (m_addresser) == -2) {
				Say ("Please just use " + settings.command_start + "rps as a single command. Thanks!");
			}

			string opponent;
			bool isPlaying = m_users.IsOpponentPlayingRPS (m_addresser, out opponent); //TODO: This needs to look for a player otherthan myself. Not the first person in the list.
			Console.WriteLine ("isPlaying: " + isPlaying + " opponent: " + opponent);
			if (isPlaying && (!opponent.Equals (m_addresser)) && m_users.RPSValue (m_addresser) != -2 && m_users.RPSValue (opponent) != -2) {
				int opponent_throw = m_users.RPSValue (opponent);
				int my_throw = m_users.RPSValue (m_addresser);
				m_users.StopRPS (opponent);
				m_users.StopRPS (m_addresser);
				if (opponent_throw == my_throw)
					Say ("The Rock, Paper, Scissors game between " + opponent + " and " + m_addresser + " ended in a tie.");
				else if (opponent_throw == (int)RoShamBo.Rock && my_throw == (int)RoShamBo.Scissors)
					Say (opponent + " has beaten " + m_addresser + " at a game of Rock, Paper, Scissors.");
				else if (opponent_throw == (int)RoShamBo.Scissors && my_throw == (int)RoShamBo.Rock)
					Say (m_addresser + " has beaten " + opponent + " at a game of Rock, Paper, Scissors.");
				else if (opponent_throw == (int)RoShamBo.Paper && my_throw == (int)RoShamBo.Scissors)
					Say (m_addresser + " has beaten " + opponent + " at a game of Rock, Paper, Scissors.");
				else if (opponent_throw == (int)RoShamBo.Scissors && my_throw == (int)RoShamBo.Paper)
					Say (opponent + " has beaten " + m_addresser + " at a game of Rock, Paper, Scissors.");
				else if (opponent_throw == (int)RoShamBo.Paper && my_throw == (int)RoShamBo.Rock)
					Say (opponent + " has beaten " + m_addresser + " at a game of Rock, Paper, Scissors.");
				else if (opponent_throw == (int)RoShamBo.Rock && my_throw == (int)RoShamBo.Paper)
					Say (m_addresser + " has beaten " + opponent + " at a game of Rock, Paper, Scissors.");
			} else if (opponent.Equals (string.Empty) && !m_users.IsPlayingRPS (m_addresser)) {
				Say (m_addresser + " is looking for an opponent in Rock, Paper, Scissors.");
			}

		}
	}
}
