/**
 *Owned By: Chandan Srivastava
 *Created By: Chandan Srivastava
 *Date Created: 27/10/2021
* Date Modified: 27/10/2021
* */

namespace Whiteboard
{
    /// <summary>
    /// Interface to specify the functions handled by ClinetCheckPointHandler
    /// </summary>
    public interface IClientCheckPointHandler
    {
        /// <summary>
        ///  Gets and sets checkpoint number.
        /// </summary>
        int CheckpointNumber { get; set; }

        /// <summary>
        /// creates and saves the checkpoint
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="currentCheckpointState"></param>
        void SaveCheckpoint(string UserId,int currentCheckpointState);

        /// <summary>
        ///  Fetches the checkpoint from server.
        /// </summary>
        /// <param name="checkpointNumber"></param>
        /// <param name="UserId"></param>
        /// <param name="currentCheckpointState"></param>
        void FetchCheckpoint(int checkpointNumber,string UserId, int currentCheckpointState);
    }
}
