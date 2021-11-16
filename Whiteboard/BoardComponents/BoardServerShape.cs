/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 11/12/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Object used for sending Updates to server.
    /// </summary>
    public class BoardServerShape
    {

        /// <summary>
        /// List of Boardshapes signifying updates sent to and received from the server.
        /// </summary>
        public List<BoardShape> ShapeUpdates { get; set; }

        /// <summary>
        /// The operation performed on state.
        /// </summary>
        public Operation OperationFlag { get; set; }

        /// <summary>
        /// The user id which requested the update.
        /// </summary>
        public string RequesterId { get; set; }

        /// <summary>
        /// Time of request creation.
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// Count of checkpoints.
        /// </summary>
        public int CheckpointNumber { get; set; }
        public int CurrentCheckpointState { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BoardServerShape()
        {
        }

        /// <summary>
        /// Constructor for Boardserver shape.
        /// </summary>
        /// <param name="shapeUpdates">List of Boardshapes.</param>
        /// <param name="operation">Operation performed on state.</param>
        /// <param name="requesterId">User id.</param>
        /// <param name="checkpointNumber">Count of checkpoints.</param>
        public BoardServerShape(List<BoardShape> shapeUpdates, Operation operation, string requesterId, int checkpointNumber = 0, int currentCheckpointState = 0)
        {
            ShapeUpdates = shapeUpdates;
            OperationFlag = operation;
            RequesterId = requesterId;
            CheckpointNumber = checkpointNumber;
            CurrentCheckpointState = currentCheckpointState;
            RequestTime = DateTime.Now;
        }

    }
}