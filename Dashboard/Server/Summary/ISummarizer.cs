using Content;

namespace Dashboard.Server.Summary
{
	/// <summary>
	/// Interface for Summary Logic / Summarizer module
	/// </summary>
	public interface ISummarizer
	{
		/// <summary>
		/// Function to get the Summary of the chat and
		/// discussion to present in the Dashboard
		/// </summary>
		/// <param name="chats">Array of ChatContext each of 
		/// which would contain an array of chat messages
		/// which would be used for the summarizer to 
		/// generate the summary
		/// </param>
		/// <param name="fraction"> The fraction of the chat 
		/// length which determines the length of summary
		/// </param>
		/// <returns>
		/// String which is the summary of the 
		/// chat in the particular discusiion
		/// </returns>
		string GetSummary(ChatContext[] chats, double fraction);

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
		/// <param name="fraction"> The fraction of the chat 
		/// length which determines the length of summary
		/// </param>
		/// <returns>
		/// Returns true if summary was succesfully stored 
		/// and false otherwise
		/// </returns>
		bool SaveSummary(ChatContext[] chats, double fraction);
	}
}
