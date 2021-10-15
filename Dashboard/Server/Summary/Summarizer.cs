using System;

namespace Dashboard.Server.Summary
{
	/// <summary>
	/// A not error for not implementing the required function
	/// which would inherit from the Eception class
	/// </summary>
	public class NotImplementedErr : Exception
	{
		/// <summary>
		/// Constructor for the NorImplementdErr class
		/// which throws the not implemented error <see cref="string"/>
		/// </summary>
		public NotImplementedErr() : base(String.Format("Not Implemented")) { }
	}
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
			throw new NotImplementedErr();
		}

		/// <summary>
		/// Function to get the Summary of the chat and
		/// discussion to present in the Dashboard
		/// </summary>
		/// <param name="chats">
		/// Thread array
		/// </param>
		/// <returns>
		/// String which is the summary of the discussion
		/// </returns>
		public string GetSummary(Thread[] chats)
		{
			throw new NotImplementedErr();
		}

		/// <summary>
		/// Function to save the Summary of the chat and
		/// discussion to present in the Dashboard into 
		/// the database
		/// </summary>
		/// <param name="chats">
		/// Thread array
		/// </param>
		/// <returns>
		/// Error code denoting the success or failure 
		/// of the operation
		/// </returns>
		public int SaveSummary(Thread[] chats)
		{
			throw new NotImplementedErr();
		}
	}
}
