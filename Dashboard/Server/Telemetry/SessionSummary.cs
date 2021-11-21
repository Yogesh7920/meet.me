using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    ///<summary>
    /// Summary of each session after it is finished, like total user count, chat count and score.
    ///</summary>
    public class SessionSummary
    {
        /// The  score of the session
        public int score;

        /// Total User count in the session
        public int userCount;

        /// Total chats in the session
        public int chatCount;
    }
}
