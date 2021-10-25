using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.Telemetry;

namespace Dashboard.Server.Persistence
{
    public class SessionIdVsChatCountList
    {
        public List<Tuple<int, int>> sessionIdVsChatCountList;
    }

    public class SessionIdVsUserCountList
    {
	        public List<Tuple<int, int>> sessionIdVsUserCountList;
    }

    public class SessionIdVsScoreList
    {
	        public List<Tuple<int, int>> sessionIdVsScoreList;
    }

    public class InsincereMemberList
    {
	       public List<int> insincereMemberList;
    }

    public interface ITelemetryPersistence
    {
	    bool save(UserCountVsTimeStamp userCountVsTime);
	    bool save(UserIdVsChatCount userIdVsChatCount);
        bool saveIrreleventUserId(InsincereMember insincereList);
        bool saveUXTelemetry(UXTelemetry uxTelemetry);
	    SessionIdVsChatCountList retrieveAllSessionIdVsChatCount();
	    SessionIdVsUserCountList retrieveAllSessionIdVsUserCount();
	    SessionIdVsScoreList retrieveAllSessionIdVsScore();
	    InsincereMemberList retrieveAllInsincereMember();
        UXTelemetry retrieveUXTelemetry();

    }

    public class TelemetryPersistence : ITelemetryPersistence
    {
        public override bool save(UserCountVsTimeStamp userCountVsTime)
        {

        }

        public override bool save(UserIdVsChatCount userIdVsChatCount)
        {

        }

        public override bool saveIrreleventUserId(InsincereMember insincereList)
        {

        }

        public override bool saveUXTelemetry(UXTelemetry uxTelemetry)
        {

        }

        public override SessionIdVsChatCountList retrieveAllSessionIdVsChatCount()
        {

        }

        public override SessionIdVsUserCountList retrieveAllSessionIdVsUserCount()
        {

        }

        public override SessionIdVsScoreList retrieveAllSessionIdVsScore()
        {

        }

        public override InsincereMemberList retrieveAllInsincereMember()
        {

        }

        public override UXTelemetry retrieveUXTelemetry()
        {

        }


    }

}
