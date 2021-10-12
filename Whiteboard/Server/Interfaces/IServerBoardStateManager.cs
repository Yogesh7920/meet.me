/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/12/2021
 * Date Modified: 10/12/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Internal interface specifying the functionalities to be used by ServerBoardCommunicator. 
    /// </summary>
    public interface IServerBoardStateManager
    {
        /// <summary>
        /// Saves the updates on state at the server.
        /// </summary>
        /// <param name="boardServerShapes">The serializable object containing all information of a shape.</param>
        /// <returns>Boolean to indicate success status of update.</returns>
        bool SaveUpdate(List<BoardServerShape> boardServerShapes);

        /// <summary>
        /// Fetches the state of the server to send to newly joined user. 
        /// </summary>
        /// <returns>List of BoardServerShape to send to a client.</returns>
        List<BoardServerShape> FetchState();

        /// <summary>
        /// Saves the checkpoint at the server. 
        /// </summary>
        /// <param name="userId">Id of the user who requested to save this checkpoint.</param>
        /// <returns>The number/identifier of the created checkpoint.</returns>
        int SaveCheckpoint(string userId);

        /// <summary>
        /// Fetches the checkpoint and updates the server state. 
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <param name="userId">The user who requested the checkpoint.</param>
        /// <returns>List of BoardServerShape to broadcast to all clients.</returns>
        List<BoardServerShape> FetchCheckpoint(int checkpointNumber, string userId);

        /// <summary>
        /// Gets the number of checkpoints saved at server. 
        /// </summary>
        /// <returns>Number specifying the number of checkpoints.</returns>
        int GetCheckpointsNumber();
    }
}
