/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 27 Oct 2021
**/

using System;
using System.Collections.Generic;
using Networking;

namespace Whiteboard
{
    /// <summary>
    /// Bridge the gap between Server side White Board Modules and Networking module
    /// </summary>

    public class ClientBoardCommunicator : IClientBoardCommunicator, INotificationHandler
    {

        private ISerializer serializer;
        private ICommunicator communicator;
        private string module_identifier;
        private List<IServerUpdateListener> subscribers;

        /// <summary>
        /// Constructor to initialize a communicator and a serializer
        /// </summary>
        public ClientBoardCommunicator()
        {
            serializer = new Serializer();
            communicator = new Communicator();
            module_identifier = "Whiteboard";
            communicator.Subscribe(module_identifier, this);
            subscribers = new List<IServerUpdateListener>();
        }

        public void OnDataReceived(string data)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// serializes the shape objects and passes it to communicator.send()
        /// </summary>
        /// <param name="clientUpdate"> the object to be passed to server</param>
        public void Send(BoardServerShape clientUpdate) 
        {
            string xml_obj = serializer.Serialize(clientUpdate);
            communicator.Send(xml_obj, module_identifier);
        }
        /// <summary>
        /// publishes deserialized objects to listeners
        /// </summary>
        /// <param name="listener">subscriber</param>
        public void Subscribe(IServerUpdateListener listener) 
        {
            subscribers.Add(listener);
        }
        


    }


}