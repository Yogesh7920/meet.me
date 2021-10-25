//Author: Parmanand Kumar
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.Telemetry;



//Persistence
namespace Dashboard.Server.Persistence
{

    public interface ITelemetryPersistence
    {
        /// <summary>
                /// save the UserCountVsTimeStamp, UserIdVsChatCount, InsincereMember data as png after each session.
        /// </summary>
        /// <params> takes IntraSessionData from Telemetry.
	    bool save(IntraSessionData intraSessionData);

        /// <summary>
                 /// append the ServerData into a file after each session end
        /// </summary>
        /// <params> takes ServerData from Telemetry
        bool saveServerData(ServerData serverData);

        /// <summary>
                 /// retrives the ServerData after end of all of the sessions.
        /// </summary>
        List<ServerData> retriveAllSeverData();

    }

    public class TelemetryPersistence : ITelemetryPersistence
    {
        public override bool save(IntraSessionData intraSessionData)
        {

        }

        public override bool saveServerData(ServerData serverData)
        {

        }

        public override List<ServerData> retriveAllSeverData()
        {

        }


    }

}
