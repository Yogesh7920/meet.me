using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class UXTelemetry{
        /// list of UserCountVsTimestamp object
        List<UserCountVsTimestamp> UserTime;
        /// list of users-chatcount 
        List<UserIDVsChatCount> UserChat;
        /// returns the users who were present in the session for less than the threshold time 
        InsincereMembers InsincereMemberList;
    }
}
