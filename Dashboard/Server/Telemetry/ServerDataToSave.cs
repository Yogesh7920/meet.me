using System.Collections.Generic;

namespace Dashboard.Server.Telemetry
{
    public class ServerDataToSave
    {
        ///<summary>
        ///     the total number of sessions
        ///</summary>
        public int SessionCount;

        /// <summary>
        ///     List of SessionSummary that is summary of each session
        /// </summary>
        public List<SessionSummary> AllSessionsSummary;
    }
}
