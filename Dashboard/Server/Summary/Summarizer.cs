using Content;
using System;

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
        /// </returns>>
        public string GetSummary(ChatContext[] chats, double fraction = 0.2)
        {
            string discussionChat = "";
            foreach (ChatContext chat in chats)
            {
                foreach (ReceiveMessageData msg in chat.MsgList)
                    discussionChat += msg.Message;
            }
            return _processor.Summarize(discussionChat, fraction);
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
        /// <param name="fraction"> The fraction of the chat 
        /// length which determines the length of summary
        /// </param>
        /// <returns>
        /// Returns true if summary was succesfully stored 
        /// and false otherwise
        /// </returns>
        public bool SaveSummary(ChatContext[] chats, double fraction = 0.2)
        {
            string summary = GetSummary(chats, fraction);
            throw new NotImplementedException();
        }

        private readonly ChatProcessor _processor;
    }
}
