using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.SessionManagement
{
    interface IUXServerSessionManager
    {
        public interface IUXServerSessionManager
        {
            /// <summary>
            /// Returns the credentials required to 
            /// Join or start the meeting
            /// </summary>
            /// <returns> A MeetingCredentials Object </returns>
            MeetingCredentials GetPortsAndIPAddress();
        }
    }
}
