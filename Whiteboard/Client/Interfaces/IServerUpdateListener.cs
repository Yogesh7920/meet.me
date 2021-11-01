/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 13 Oct 2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Listens to publications from ClientBoardCommunicator
    /// </summary>
    public interface IServerUpdateListener
    {

        /// <summary>
        /// Receives deserialized BoardServerShape objects from ClientBoardCommunicator
        /// </summary>
        /// <param name="serverUpdate"> deserialized Object that was sent from ClientBoardCommunicator</param>
        void OnMessageReceived(BoardServerShape serverUpdate);
    }
}