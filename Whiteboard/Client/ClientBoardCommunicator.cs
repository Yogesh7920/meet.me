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
    /// Bridge between Client side White Board Modules and Networking module's Communicator
    /// </summary>
    public class ClientBoardCommunicator : IMessageListener, IClientBoardStateManager
    {
        /// <summary>
        /// serialize the xml string to Board server shape and 
        /// publish the deserialized object to subscribers
        /// </summary>
        /// <param name="data"> xml string received from server side</param>
        public void OnMessageReceived(string data) 
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// serialize the shape objects and pass it to communicator.send()
        /// </summary>
        /// <param name="clientUpdate"> the object to be passed to server</param>
        public void Send(List<BoardServerShape> clientUpdate) 
        { 
            throw new NotImplementedException();
        }
        /// <summary>
        /// Subscription to receive deserialized objects from ClientBoardCommunicator
        /// </summary>
        /// <param name="listener"></param>
        public void Subscribe(IServerUpdateListener listener) 
        {
            throw new NotImplementedException();
        }
        


    }


}