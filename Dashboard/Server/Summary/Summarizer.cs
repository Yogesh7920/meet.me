using System.Collections.Generic;
using System.Diagnostics;
using Content;
using Dashboard.Server.Persistence;

namespace Dashboard.Server.Summary
{
	/// <summary>
	/// Summarizer which implements the ISummarizer interface
	/// </summary>
	class Summarizer : ISummarizer
	{
		/// <summary>
		/// The Constructor for the summarizer 
		/// for the summary logic module to get
		/// the summary for the chats and save 
		/// this summary in the database
		/// </summary>
		public Summarizer()
		{
			_processor = new ChatProcessor();
			_persister = PersistenceFactory.GetSummaryPersistenceInstance();
		}

		/// <summary>
		/// Function to get the Summary of the chat and
		/// discussion to present in the Dashboard
		/// </summary>
		/// <param name="chats">Array of ChatContext each of 
		/// which would contain an array of chat messages
		/// which would be used for the summarizer to 
		/// generate the summary
		/// <returns>
		/// String which is the summary of the 
		/// chat in the particular discusiion
		/// </returns>>
		public string GetSummary(ChatContext[] chats)
		{
			if (chats == null || chats.Length == 0)
			{
				Trace.WriteLine("Empty chat context obtained.");
				return "";
			}
			List<(string, bool)> discussionChat = new();
			foreach (ChatContext chat in chats)
			{
				foreach (ReceiveMessageData msg in chat.MsgList)
				{
					if (msg.Type == MessageType.Chat)
						discussionChat.Add((msg.Message, msg.Starred));
				}
			}
			return _processor.Summarize(discussionChat);
		}

		/// <summary>
		/// Function to save the summary of the dashboard
		/// discussion that took place into the database
		/// using the persistence module.
		/// </summary>
		/// <param name="chats">Array of ChatContext each of 
		/// which would contain an array of chat messages
		/// which would be used for the summarizer to 
		/// save the summary in the database
		/// </param>
		/// <returns>
		/// Returns true if summary was succesfully stored 
		/// and false otherwise
		/// </returns>
		public bool SaveSummary(ChatContext[] chats)
		{
			return _persister.SaveSummary(GetSummary(chats));
		}

		private readonly ChatProcessor _processor;
		private readonly ISummaryPersistence _persister;
	}
}
