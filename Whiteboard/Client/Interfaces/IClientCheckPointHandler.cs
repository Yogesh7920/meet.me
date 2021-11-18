/**
 *Owned By: Chandan Srivastava
 *Created By: Chandan Srivastava
 *Date Created: 27/10/2021
* Date Modified: 27/10/2021
* */

namespace Whiteboard
{
    /// <summary>
    ///     Interface to specify the functions handled by ClinetCheckPointHandler
    /// </summary>
    public interface IClientCheckPointHandler
    {
        /// <summary>
        ///     Gets and sets checkpoint number.
        /// </summary>
        int CheckpointNumber { get; set; }

        /// <summary>
        ///     Creates and saves checkpoint.
        /// </summary>
        void SaveCheckpoint();

        /// <summary>
        ///     Fetches the checkpoint from server.
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        void FetchCheckpoint(int checkpointNumber);
    }
}