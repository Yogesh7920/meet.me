using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;


namespace Dashboard
{
    using Client.SessionManagement;
    using Server.SessionManagement;
    public static class SessionManagerFactory
    {
        /// <summary>
        /// Constructor to create client and server session manager objects.
        /// </summary>
        static SessionManagerFactory()
        {
            // the objects are initialized only once for the program
            if(s_clientSessionManager == null)
            {
                s_clientSessionManager = new ClientSessionManager();
            }

            if(s_serverSessionManager == null)
            {
               s_serverSessionManager = new ServerSessionManager();
            }
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
        /// Constructor for testing the module
        /// </summary>
        /// <param name="communicator"> Test communicator to test functionality</param>
        /// <returns></returns>
        public static IUXClientSessionManager GetClientSessionManager(ICommunicator communicator)
        {
            return new ClientSessionManager(communicator);
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

        /// <summary>
        /// Constructor for testing the module
        /// </summary>
        /// <param name="communicator"> Test communicator to test functionality</param>
        /// <returns></returns>
        public static ServerSessionManager GetServerSessionManager(ICommunicator communicator)
        {
            return new ServerSessionManager(communicator);
        }

        private static IUXClientSessionManager s_clientSessionManager;
        private static ITelemetrySessionManager s_serverSessionManager;
    }
}
