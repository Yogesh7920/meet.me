/// <author>Harsh Parihar</author>
/// <created> 17/11/2021 </created>
/// <summary>
/// Contains the summary of session
/// </summary>

namespace Dashboard
{
    /// <summary>
    ///     Summary of each session after it is finished, like total user count, chat count and score.
    /// </summary>
    public class SessionSummary
    {
        /// Total chats in the session
        public int chatCount;

        /// The  score of the session
        public int score;

        /// Total User count in the session
        public int userCount;
    }
}