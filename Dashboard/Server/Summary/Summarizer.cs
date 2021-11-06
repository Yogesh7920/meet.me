using System;
using Content;

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
		}

		/// <summary>
		/// Function to get the Summary of the chat and
		/// discussion to present in the Dashboard
		/// </summary>
		/// <param name="chats">
		/// ChatContext array
		/// </param>
		/// <returns>
		/// String which is the summary of the discussion
		/// </returns>
		public string GetSummary(ChatContext[] chats, double fraction = 0.2)
		{
			string discussionChat = "";
			int numChats = 0;
			foreach (ChatContext chat in chats)
			{
				foreach (ReceiveMessageData msg in chat.MsgList)
				{
					discussionChat += msg.Message;
					numChats++;
				}
			}
			return _processor.Summarize(discussionChat, fraction);
		}

		/// <summary>
		/// Function to save the Summary of the chat and
		/// discussion to present in the Dashboard into 
		/// the database
		/// </summary>
		/// <param name="chats">
		/// ChatContext array
		/// </param>
		/// <returns>
		/// Error code denoting the success or failure 
		/// of the operation
		/// </returns>
		public bool SaveSummary(ChatContext[] chats, double fraction = 0.2)
		{
			string summary = GetSummary(chats, fraction);
			throw new NotImplementedException();
		}

		private readonly ChatProcessor _processor;
	}
}
