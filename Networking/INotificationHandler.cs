namespace Networking
{
    public interface INotificationHandler
    {
        /// <summary>
        /// Handles messages received over the network.
        /// </summary>
        /// <param name="data">received message.</param>
        void OnDataReceived(string data);

        void OnClientJoined<T>(T socketObject)
        {
        }

        void OnClientLeft(string clientId)
        {
        }
    }
}