using System.Diagnostics.CodeAnalysis;

namespace Networking
{
    public interface INotificationHandler
    {
        /// <summary>
        ///     Handler for messages received over the network. This
        ///     will be signalled only to the concerned modules.
        /// </summary>
        /// <param name="data">The received message.</param>
        void OnDataReceived(string data);

        /// <summary>
        ///     Event to indicate that the client has joined the room. This
        ///     will be signalled only on the server side to all modules.
        /// </summary>
        /// <param name="socketObject">The socket object of the incoming client.</param>
        /// <typeparam name="T">Type of the socket object (TcpClient)</typeparam>
        [ExcludeFromCodeCoverage]
        void OnClientJoined<T>(T socketObject)
        {
        }

        /// <summary>
        ///     Event to indicate that the client has left the room. This will be
        ///     signalled only on the server side to all modules.
        /// </summary>
        /// <param name="clientId"></param>
        [ExcludeFromCodeCoverage]
        void OnClientLeft(string clientId)
        {
        }
    }
}