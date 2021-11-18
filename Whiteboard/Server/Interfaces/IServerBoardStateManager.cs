/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/12/2021
 * Date Modified: 10/12/2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Internal interface specifying the functionalities to be used by ServerBoardCommunicator.
    /// </summary>
    public interface IServerBoardStateManager
    {
        /// <summary>
        ///     Saves the updates on state at the server.
        /// </summary>
        /// <param name="boardServerShape">Object containing the update information for shape.</param>
        /// <returns>Boolean to indicate success status of update.</returns>
        bool SaveUpdate(BoardServerShape boardServerShape);

        /// <summary>
        ///     Fetches the state of the server to send to newly joined user.
        /// </summary>
        /// <param name="userId">The newly joined user who requested the state fetch.</param>
        /// <returns>BoardServerShape containing all shape updates and no. of checkpoints to send to the client.</returns>
        BoardServerShape FetchState(string userId);

        /// <summary>
        ///     Saves the checkpoint at the server.
        /// </summary>
        /// <param name="userId">Id of the user who requested to save this checkpoint.</param>
        /// <returns>BoardServerShape object specifying the checkpoint number which was created.</returns>
        BoardServerShape SaveCheckpoint(string userId);

        /// <summary>
        ///     Fetches the checkpoint and updates the server state.
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <param name="userId">The user who requested the checkpoint.</param>
        /// <returns>BoardServerShape containing all shape information to broadcast to all clients.</returns>
        BoardServerShape FetchCheckpoint(int checkpointNumber, string userId);

        /// <summary>
        ///     Gets the number of checkpoints saved at server.
        /// </summary>
        /// <returns>Number specifying the number of checkpoints.</returns>
        int GetCheckpointsNumber();
    }
}