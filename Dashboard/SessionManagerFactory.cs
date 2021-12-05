/// <author> Rajeev Goyal </author>
/// <created> 22/10/2021 </created>
/// <summary>
/// This file contains the SessionManager Factory that produces Client and Server Session 
/// Managers for both testing and implementing.
/// </summary>

using System;
using Content;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;
using Networking;
using Whiteboard;

namespace Dashboard
{
    public static class SessionManagerFactory
    {
        private static readonly Lazy<ClientSessionManager> s_clientSessionManager =
            new(() => new ClientSessionManager());

        private static readonly Lazy<ServerSessionManager> s_serverSessionManager =
            new(() => new ServerSessionManager());

        /// <summary>
        ///     This method will create a Client sided server
        ///     manager that will live till the end of the program
        /// </summary>
        /// <returns>
        ///     Returns a ClientSessionManager object which
        ///     implements the interface IUXClientSM
        /// </returns>
        public static ClientSessionManager GetClientSessionManager()
        {
            return s_clientSessionManager.Value;
        }

        /// <summary>
        ///     Constructor for testing the module
        /// </summary>
        /// <param name="communicator"> Test communicator to test functionality</param>
        /// <returns></returns>
        public static ClientSessionManager GetClientSessionManager(ICommunicator communicator,
            IClientBoardStateManager whiteBoardInstance = null)
        {
            return new ClientSessionManager(communicator, whiteBoardInstance);
        }

        /// <summary>
        ///     This method will server a Client sided server
        ///     manager that will live till the end of the program
        /// </summary>
        /// <returns>
        ///     Returns a ServerSessionManager object which
        ///     implements the interface ITelemetrySessionManager
        /// </returns>
        public static ServerSessionManager GetServerSessionManager()
        {
            return s_serverSessionManager.Value;
        }

        /// <summary>
        ///     Constructor for testing the module
        /// </summary>
        /// <param name="communicator"> Test communicator to test functionality</param>
        /// <returns></returns>
        public static ServerSessionManager GetServerSessionManager(ICommunicator communicator,
            IContentServer contentServer = null)
        {
            return new ServerSessionManager(communicator, contentServer);
        }
    }
}