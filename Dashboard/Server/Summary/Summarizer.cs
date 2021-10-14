namespace Dashboard.Server.Summary
{
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
			return "Not Implemented";
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
			return -1;
		}
	}
}
