using System;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using CsBot.Interfaces;

namespace CsBot.Commands
{
	public class RssFeedCommand : IIrcCommand
	{
		public void Next ()
		{
			int count = 0;
			SyndicationFeed feed = m_users[m_addresser].Feed;
			int feedCount = m_users[m_addresser].FeedCount;

			if (feed == null) {
				Say ("You must use " + settings.command_start + "getfeeds before trying to list them.");
				return;
			}

			if (feedCount >= feed.Items.Count () || feedCount < 0) {
				feedCount = 0;
				Say ("Reached max, starting over.");
			}

			for (; feedCount < feed.Items.Count (); feedCount++) {
				SyndicationItem item = feed.Items.ElementAt (feedCount);
				Say ("Item " + (feedCount + 1) + " of " + feed.Items.Count () + ": " + item.Title.Text.Trim ());
				count++;
				if (count == 4) break;
			}

			m_users[m_addresser].FeedCount = ++feedCount;
		}

		public void More ()
		{
			int feedNumber;
			var feed = m_users[m_addresser].Feed;
			if (feed == null) {
				Say ("You must use " + settings.command_start + "getfeeds before trying to get more information.");
				return;
			}

			if (command.Length == endCommand + 1 || !int.TryParse (command.Substring (endCommand + 2).Trim ().ToLower (), out feedNumber)) {
				Say ("Usage: " + settings.command_start + "getmore # (Where # is an item from the rss feed)");
			} else {
				feedNumber--;
				if (feedNumber < 0 || feedNumber > feed.Items.Count ()) {
					Say ("Number is not in the right range. Must be from 1 to " + feed.Items.Count () + ".");
				} else {
					count = 0;
					Say ("Links for " + feed.Items.ElementAt (feedNumber).Title.Text + "(Max 4):");
					foreach (SyndicationLink link in feed.Items.ElementAt (feedNumber).Links) {
						Say (link.Uri.ToString ().Trim ());
						count++;
						if (count == 4) return;
					}
				}
			}
		}

		public void Feeds ()
		{
			XmlReader rssReader;
			if (command.Length == endCommand + 1) {
				rssReader = XmlReader.Create ("http://rss.news.yahoo.com/rss/topstories#");
			} else {
				try {
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create (command.Substring (endCommand + 2).Trim ().ToLower ());
					request.AllowAutoRedirect = true;
					HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
					rssReader = XmlReader.Create (response.GetResponseStream ());
				} catch (Exception e) {
					Say ("Usage: " + settings.command_start + "getfeeds http://<rsssite>/rss/<rssfeed#>");
					Console.WriteLine (e.Message);
					return;
				}
			}
			try {
				var feed = SyndicationFeed.Load (rssReader);
				m_users[m_addresser].Feed = feed;
				rssReader.Close ();
				count = 0;
				m_users[m_addresser].FeedCount = 4;

				Say ("Items 1 through 4 of " + feed.Items.Count () + " items.");
				foreach (SyndicationItem item in feed.Items) {
					Say (item.Title.Text.Trim ());
					count++;
					if (count == 4) return;
				}
			} catch (Exception e) {
				Say ("Received invalid feed data.");
				Console.WriteLine (e.Message);
			}
		}
	}
}
