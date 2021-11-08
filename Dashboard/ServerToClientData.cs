using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.Summary;

namespace Dashboard
{
    /// <summary>
    /// Class for sending data to the client side
    /// from the server side
    /// </summary>
    public class ServerToClientData
    {
        /// <summary>
        /// Parametric constructor to initialize the fields
        /// </summary>
        /// <param name="eventName"> The name of the event </param>
        /// <param name="objectToSend"> The object that is to be sent on the client side </param>
        public ServerToClientData(string eventName, IRecievedFromServer objectToSend, UserData user)
        {
            eventType = eventName;
            _receivedObject = objectToSend;
            _user = user;
        }

        /// <summary>
        /// Returns the object sent from the server to the client
        /// </summary>
        /// <returns>An object of type IReceivedFromServer</returns>
        public IRecievedFromServer GetObject()
        {
            return _receivedObject;
        }

        public UserData GetUser()
        {
            return _user;
        }

        public string eventType;
        private IRecievedFromServer _receivedObject;
        private UserData _user;
    }
}
