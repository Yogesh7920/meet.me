/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 13 Oct 2021
**/

using System;
using System.Collections.Generic;

namespace Whiteboard
{
    /// <summary>
    /// Bridge the gap between Server side White Board Modules and Networking module
    /// </summary>

    public class ClientBoardCommunicator : IClientBoardCommunicator, IMessageListener
    {
        /// <summary>
        /// deserializes the xml string to Board server shape
        /// publishes the deserialized object to subscribers
        /// </summary>
        /// <param name="data"> xml string received from server side</param>
        public void OnMessageReceived(string data) 
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// serializes the shape objects and passes it to communicator.send()
        /// </summary>
        /// <param name="clientUpdate"> the object to be passed to server</param>
        public void Send(List<BoardServerShape> clientUpdate) 
        { 
            throw new NotImplementedException();
        }
        /// <summary>
        /// publishes deserialized objects to listeners
        /// </summary>
        /// <param name="listener">subscriber</param>
        public void Subscribe(IServerUpdateListener listener) 
        {
            throw new NotImplementedException();
        }
        


    }


}