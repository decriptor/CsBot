using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CsBot.Models;
using Newtonsoft.Json;

namespace CsBot
{
	class SettingsManager
	{
		bool UseLocalSettings { get; }
		public Settings Settings { get; private set; }

		public SettingsManager (bool useLocalSettings)
		{
			UseLocalSettings = useLocalSettings;
		}

		void LoadLocalSettings (string file)
		{

		}

		public void LoadRemoteSettings (string url)
		{
			Console.WriteLine ("Trying to pull config.");
			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
			//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

			var remoteSettings = new Settings ();
			using (var client = new HttpClient ())
			using (HttpResponseMessage response = client.GetAsync (url).Result)
			using (HttpContent content = response.Content) {
				var json = content.ReadAsStringAsync ().Result;
				remoteSettings = JsonConvert.DeserializeObject<Settings> (json);
				Console.WriteLine ("Config File:{0}", remoteSettings);
			}

			Settings = remoteSettings;
		}

		public bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			// TODO take this out for actual validation.
			return true;
			foreach (X509ChainStatus status in chain.ChainStatus) {
				if (certificate.Subject == certificate.Issuer &&
						status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot &&
						Settings != null && Settings.server_validate == false) {
					continue;

				} else if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError &&
						  ((Settings != null && Settings.server_validate != false) || status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot)) {
					Console.WriteLine ("Certificate not valid: {0}, {1}", sslPolicyErrors, status.StatusInformation);
					return false;
				}
			}

			return true;
		}

	}
}
