using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Client.SessionManagement
{
    public interface ISessionNotifications
    {
        /// <summary>
        /// Handles the changes in the SessionData object
        /// </summary>
        /// <param name="session"> The changed SessionData </param>
        void OnSessionChanged(SessionData session);
    }
}

