using System.Collections.Generic;

namespace Dashboard.Server.Telemetry
{
    /// <summary>
    /// The server data that would be stored in Persistance
    /// </summary>
    public class ServerDataToSave
    {
        ///<summary>
        ///     the total number of sessions
        ///</summary>
        public int sessionCount;

        /// <summary>
        ///     List of SessionSummary that is summary of each session
        /// </summary>
        public List<SessionSummary> allSessionsSummary;
    }
}
