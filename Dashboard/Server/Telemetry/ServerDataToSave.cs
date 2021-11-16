
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class ServerDataToSave
    {
        ///<summary>
        ///     the total number of sessions
        ///</summary>
        int SessionCount;

        /// <summary>
        ///     List of SessionSummary that is summary of each session
        /// </summary>
        List<SessionSummary> AllSessionsSummary;
    }
}

