using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;

namespace Dashboard
{
    public static class SessionManagerFactory
    {
        private static readonly IUXClientSessionManager s_clientSessionManager;
        private static readonly ITelemetrySessionManager s_serverSessionManager;

        /// <summary>
        ///     Constructor to create client and server session manager objects.
        /// </summary>
        static SessionManagerFactory()
        {
            // the objects are initialized only once for the program
            if (s_clientSessionManager == null) s_clientSessionManager = new ClientSessionManager();

            if (s_serverSessionManager == null) s_serverSessionManager = new ServerSessionManager();
        }

        /// <summary>
        ///     This method will create a Client sided server
        ///     manager that will live till the end of the program
        /// </summary>
        /// <returns>
        ///     Returns a ClientSessionManager object which
        ///     implements the interface IUXClientSM
        /// </returns>
        public static IUXClientSessionManager GetClientSessionManager()
        {
            return s_clientSessionManager;
        }

        /// <summary>
        ///     This method will server a Client sided server
        ///     manager that will live till the end of the program
        /// </summary>
        /// <returns>
        ///     Returns a ServerSessionManager object which
        ///     implements the interface ITelemetrySessionManager
        /// </returns>
        public static ITelemetrySessionManager GetServerSessionManager()
        {
            return s_serverSessionManager;
        }
    }
}