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
    /// Interface to be used by Client side state manager
    /// functionalities include
    /// 1. sending an object using send()
    /// 2. subscribing to receive objects from server side using subscribe()
    /// </summary>
    public interface IClientBoardCommunicator
    {
        /// <summary>
        /// to send objects from statemanager to communicator after serializing
        /// </summary>
        /// <param name="clientUpdate"> update from client to server side</param>
        void Send(List<BoardServerShape> clientUpdate);
        
        /// <summary>
        /// subscribe to receive deserialized objects from Communicator
        /// </summary>
        /// <param name="listener"> the subscriber </param>
        void Subscribe(IServerUpdateListener listener);
    }


}