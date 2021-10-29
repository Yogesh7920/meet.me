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
        /// <param name="chats">Array of thread each of 
        /// which would contain an array of chat messages
        /// which would be used for the summarizer to 
        /// generate the summary
        /// </param>
        /// <returns>
        /// String which is the summary of the 
        /// chat in the particular discusiion
        /// </returns>
        string GetSummary(Thread[] chats);

        /// <summary>
        /// Function to save the summary of the dashboard
        /// discussion that took place into the database
        /// using the persistence module.
        /// </summary>
        /// <param name="chats">Array of thread each of 
        /// which would contain an array of chat messages
        /// which would be used for the summarizer to 
        /// save the summary in the database
        /// </param>
        /// <returns>
        /// An error code where 0 represents that the summary 
        /// has been successfullly stored and -1 to denote 
        /// there has been an error in the saving of the 
        /// summary in the database
        /// </returns>
        int SaveSummary(Thread[] chats);
    }
}
