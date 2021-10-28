/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 28 Oct 2021
**/

using System;
using System.Collections.Generic;
using Networking;

namespace Whiteboard
{
    /// <summary>
    /// Bridge the gap between Server side White Board Modules and Networking module
    /// </summary>
    public sealed class ServerBoardCommunicator : INotificationHandler , IServerBoardCommunicator
    {


        private static ServerBoardCommunicator instance = null;
        private static ISerializer serializer;
        private static ICommunicator communicator;
        private static string moduleIdentifier = "Whiteboard";
        /// <summary>
        /// private constructor for a singleton
        /// </summary>
        private ServerBoardCommunicator() { }

        /// <summary>
        /// instance getter
        /// </summary>
        public static ServerBoardCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerBoardCommunicator();
                    serializer = new Serializer();
                    communicator = new Communicator();
                    communicator.Subscribe(moduleIdentifier, instance);
                }
                return instance;
            }
        }

        public void OnDataReceived(string data)
        {
            string c_data = data;
            BoardServerShape deserializedShape = serializer.Deserialize<BoardServerShape>(data);
            // for now broadcasting the object to all clients 
            communicator.Send(data, moduleIdentifier);
        }

        /// <summary>
        /// serializes the shape objects and passes it to communicator.send()
        /// </summary>
        /// <param name="clientUpdate"> the object to be passed to server</param>
        public void Send(BoardServerShape clientUpdate, string clientId = "all")
        {
            string xml_obj = serializer.Serialize(clientUpdate);
            if (clientId == "all")
            {
                communicator.Send(xml_obj, moduleIdentifier);
            }
            else 
            {
                communicator.Send(xml_obj, moduleIdentifier, clientId);
            }

        }

    }


}