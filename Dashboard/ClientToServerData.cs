/// <author> Rajeev Goyal </author>
/// <created> 24/10/2021 </created>
/// <summary>
/// This file contains the ClientToServerData class whose instance are used to send data from client 
/// to server side.
/// </summary>

namespace Dashboard
{
    public class ClientToServerData
    {
        public string eventType;
        public int userID;
        public string username;

        /// <summary>
        ///     Parametric constructor to initialise the fields
        /// </summary>
        /// <param name="eventName"> The name of the event </param>
        /// <param name="clientName"> Name of the user </param>
        /// <param name="clientID"> The ID of the user (-1, if not known/assigned) </param>
        public ClientToServerData(string eventName, string clientName, int clientID = -1)
        {
            eventType = eventName;
            username = clientName;
            userID = clientID;
        }

        /// <summary>
        ///     Default constructor for serialization
        /// </summary>
        public ClientToServerData()
        {
        }
    }
}