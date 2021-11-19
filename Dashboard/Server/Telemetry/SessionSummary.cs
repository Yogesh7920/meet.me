using System;

namespace Dashboard.Server.Telemetry
{
    public class SessionSummary
    {
        /// The starting time of the session
        public DateTime SessionStartTime;

        /// The  score of the session
        public int Score;

        /// Total User count in the session
        public int UserCount;

        /// Total chats in the session
        public int ChatCount;
    }
}