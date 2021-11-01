/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/11/2021
 * Date Modified: 10/11/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Interface to be used by UX and Dashboard team.
    /// Provides functionality to get the single created instance, initialize all variables, subscribe for notification and saving/fetching all checkpoints.
    /// </summary>
    public interface IClientBoardStateManager
    {
        /// <summary>
        /// Static method to get the single created instance of ClientBoardStateManager. 
        /// </summary>
        static ClientBoardStateManager Instance { get; }

        /// <summary>
        /// Initializes state managers attributes. 
        /// </summary>
        void Start();

        /// <summary>
        /// Subscribes to notifications from ClientBoardStateManager to get updates.
        /// </summary>
        /// <param name="listener">The subscriber. </param>
        /// <param name="identifier">The identifier of the subscriber. </param>
        /// <returns>List of UXShapes for UX to render along with an integer which specifies number of checkpoints saved on server.</returns>
        Tuple<List<UXShape>, int> Subscribe(IClientBoardStateListener listener, string identifier);

        /// <summary>
        /// Creates and saves checkpoint. 
        /// </summary>
        /// <returns>The number/identifier of the created checkpoint.</returns>
        int SaveCheckpoint();

        /// <summary>
        /// Fetches the checkpoint from server and updates the current state. 
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <returns>List of UXShapes for UX to render.</returns>
        List<UXShape> FetchCheckpoint(int checkpointNumber);

        /// <summary>
        /// Sets the current user id. 
        /// </summary>
        /// <param name="userId">user Id of the current user.</param>
        void SetUser(string userId);

    }
}
