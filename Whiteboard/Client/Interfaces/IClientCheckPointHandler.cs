/**
 *Owned By: Chandan Srivastava
 *Created By: Chandan Srivastava
 *Date Created: 27/10/2021
* Date Modified: 27/10/2021
* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Interface to specify the functions handled by ClinetCheckPointHandler
    /// </summary>
    public interface IClientCheckPointHandler
    {
        /// <summary>
        /// Creates and saves checkpoint. 
        /// </summary>
        /// <returns>The number/identifier of the created checkpoint.</returns>
        int SaveCheckpoint();

        /// <summary>
        /// Fetches the checkpoint from server. 
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <returns>List of UXShapes to ClientBoardStateManager.</returns>
        List<UXShape> FetchCheckpoint(int checkpointNumber);

    }
}