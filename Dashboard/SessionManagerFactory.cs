using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dashboard
{
    using Client.SessionManagement;
    using Server.SessionManagement;
    public static class SessionManagerFactory
    {
        
        static SessionManagerFactory()
        {
            s_clientSessionManager = new ClientSessionManager();
            s_serverSessionManager = new ServerSessionManager();
        }
        /// <summary>
        /// This method will create a Client sided server 
        /// manager that will live till the end of the program
        /// </summary>
        /// <returns>
        /// Returns a ClientSessionManager object which 
        /// implements the interface IUXClientSM
        /// </returns>
        public static IUXClientSessionManager GetClientSessionManager()
        {
            return s_clientSessionManager;
        }

        /// <summary>
        /// This method will server a Client sided server
        /// manager that will live till the end of the program
        /// </summary>
        /// <returns>
        /// Returns a ServerSessionManager object which 
        /// implements the interface ITelemetrySessionManager
        /// </returns>
        public static ITelemetrySessionManager  GetServerSessionManager()
        {
            return s_serverSessionManager;
        }

        private static IUXClientSessionManager s_clientSessionManager;
        private static ITelemetrySessionManager s_serverSessionManager;
    }
}
