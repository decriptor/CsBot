using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CsBot.Models
{
	[DataContract]
	class Settings
	{
		[DataMember (Name = "server", IsRequired = true)]
		public string server;

		[DataMember (Name = "password", IsRequired = true)]
		public string password;

		[DataMember (Name = "port", IsRequired = true)]
		public int port;

		[DataMember (Name = "secure", IsRequired = false)]
		public string secure;

		[DataMember (Name = "server_validate", IsRequired = false)]
		public bool server_validate;

		[DataMember (Name = "user", IsRequired = true)]
		public string user;

		[DataMember (Name = "nick", IsRequired = true)]
		public string nick;

		[DataMember (Name = "channels", IsRequired = true)]
		public List<Channel> channels;

		[DataMember (Name = "admins", IsRequired = true)]
		public string[] admins;

		[DataMember (Name = "command_start", IsRequired = true)]
		public string command_start;

		[DataMember (Name = "insults", IsRequired = false)]
		public string[] insults;

		[DataMember (Name = "quotes", IsRequired = false)]
		public string[] quotes;

		[DataMember (Name = "praises", IsRequired = false)]
		public string[] praises;
	}

	[DataContract]
	class Channel
	{
		[DataMember (Name = "id", IsRequired = true)]
		public int id;

		[DataMember (Name = "name", IsRequired = true)]
		public string name;

		[DataMember (Name = "key", IsRequired = false)]
		public string key;
	}
}
