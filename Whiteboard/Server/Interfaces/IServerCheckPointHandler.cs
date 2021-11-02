/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 13/10/2021
 * Date Modified: 13/10/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Interface to specify the functions handled by ServerCheckPointHandler
    /// </summary>
    public interface IServerCheckPointHandler
    {
        /// <summary>
        /// Saves the checkpoint at the server. 
        /// </summary>
        /// <param name="userId">Id of the user who requested to save this checkpoint.</param>
        /// <returns>The number/identifier corresponding to the created checkpoint.</returns>
        int SaveCheckpoint(string userId);

        /// <summary>
        /// Fetches the checkpoint corresponding to provided userId and checkPointNumber
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <param name="userId">Id of the user who requested to fetch the checkpoint.</param>
        /// <returns>List of BoardServerShape to the ServerBoardStateManager.</returns>
        List<BoardServerShape> FetchCheckpoint(int checkpointNumber, string userId);

        /// <summary>
        /// To Get the total number of checkpoints saved at server side. 
        /// </summary>
        /// <returns>Number corresponding to the total number of checkpoints at server.</returns>
        int GetCheckpointsNumber();
    }
}