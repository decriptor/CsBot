using System.Collections.Generic;

namespace CsBot
{
	class Users
	{
		readonly Dictionary<string, User> m_users;

		public Users ()
		{
			m_users = new Dictionary<string, User> ();
		}

		public bool HasUser (string user)
		{
			return m_users.ContainsKey (user);
		}

		public void RemoveUser (string user)
		{
			m_users.Remove (user);
		}

		public void AddUser (string user, User tmpUser = null)
		{
			if (tmpUser == null)
				tmpUser = new User { Name = user };

			if (!m_users.ContainsKey (user))
				m_users.Add (user, tmpUser);
		}

		public bool IsPlayingRPS (out string playing_user)
		{
			foreach (var user in m_users) {
				if (user.Value.RPSFlag) {
					playing_user = user.Key;
					return true;
				}
			}

			playing_user = string.Empty;
			return false;
		}

		public bool IsOpponentPlayingRPS (string addresser, out string playing_user)
		{
			foreach (var user in m_users) {
				if (user.Value.RPSFlag && user.Key != addresser) {
					playing_user = user.Key;
					return true;
				}
			}

			playing_user = string.Empty;
			return false;
		}

		public bool IsPlayingRPS (string current_user)
		{
			return m_users[current_user].RPSFlag;
		}

		public bool IsPlayingFarkle (string current_user)
		{
			return m_users[current_user].FarkleFlag;
		}

		public void ClearFarkleScores ()
		{
			foreach (string m in m_users.Keys) {
				FarkleValue (m, -FarkleValue (m));
			}
		}

		public bool IsPlayingFarkle ()
		{
			bool ans = false;
			foreach (string m in m_users.Keys) {
				ans = m_users[m].FarkleFlag;
				if (ans == true)
					break;
			}
			return ans;
		}

		public bool SomeoneHasToken ()
		{
			bool ans = false;
			foreach (string m in m_users.Keys) {
				ans = m_users[m].HasFarkleToken;
				if (ans == true)
					break;
			}
			return ans;
		}

		public void SetFarkleFlag (string player, bool playing)
		{
			if (m_users.ContainsKey (player))
				m_users[player].FarkleFlag = playing;
		}

		public void FarkleValue (string player, int value)
		{
			if (m_users.ContainsKey (player))
				m_users[player].FarkleValue = value;
		}

		public int FarkleValue (string player)
		{
			if (m_users.ContainsKey (player))
				return m_users[player].FarkleValue;

			return 0;
		}

		public void SetFarkleToken (string player, bool value)
		{
			if (m_users.ContainsKey (player))
				m_users[player].HasFarkleToken = value;
		}

		public bool GetFarkleToken (string player)
		{
			if (m_users.ContainsKey (player))
				return m_users[player].HasFarkleToken;

			return false;
		}

		public void RPSValue (string player, int value)
		{
			if (m_users.ContainsKey (player)) {
				m_users[player].RPSFlag = true;
				m_users[player].RPS = value;
			}
		}

		public int RPSValue (string player)
		{
			if (m_users.ContainsKey (player))
				return m_users[player].RPS;

			return -1;
		}

		public void AddUserLastMessage (string uname, string message)
		{
			m_users[uname].Message = message;
		}

		public void StopRPS (string uname)
		{
			m_users[uname].RPS = -2;
			m_users[uname].RPSFlag = false;
		}

		public string getUserMessage (string uname)
		{
			return m_users[uname].Message;
		}

		public IEnumerator<string> GetEnumerator ()
		{
			Dictionary<string, User>.KeyCollection.Enumerator e = m_users.Keys.GetEnumerator ();
			return e;
		}

		public User this[string name] {
			get => m_users[name];
			set {
				return; //Don't allow
			}
		}
	}
}
