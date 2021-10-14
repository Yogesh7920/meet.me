using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.SessionManagement
{
    public class ServerSessionManager : ITelemetrySessionManager
    {
        public class NotImplementedErr : Exception
        {
            public NotImplementedErr() : base(String.Format("Not Implemented")) { }
        }

        /// <summary>
        /// Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The listener of the subscriber </param>
        public void Subcribe(ITelemetryNotifications listener, string identifier)
        {
            throw new NotImplementedErr();
        }
    }
}
