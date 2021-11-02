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
    public class ServerBoardCommunicator : IServerMessageListener, IServerBoardCommunicator
    {
        /// <summary>
        /// 1. sends/broadcasts the xml string using IServerCommunicator.send/broadcast
        /// 2. deserializes the xml string to Board server shape and passes to server state manager
        /// </summary>
        /// <param name="data"> xml string received from server side</param>
        public void OnMessageReceived(string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// serializes the shape objects and passes it to IServerCommunicator
        /// </summary>
        /// <param name="serverShapes"> the object to be passed to clients</param>
        /// <param name="clientID"> client id to whom to send these objects to</param>
        public void Send(List<BoardServerShape> clientUpdate, string clientID) 
        { 
            throw new NotImplementedException();
        }

    }


}