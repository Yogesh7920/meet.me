/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/12/2021
 * Date Modified: 10/12/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Whiteboard
{
    /// <summary>
    ///     Server-side state management for Whiteboard.
    ///     Non-extendable class having functionalities to maintain state at server side.
    /// </summary>
    public sealed class ServerBoardStateManager : IServerBoardStateManager
    {
        // data structures to maintain state
        private readonly Dictionary<string, BoardShape> _mapIdToBoardShape;
        private Dictionary<string, QueueElement> _mapIdToQueueElement;
        private readonly BoardPriorityQueue _priorityQueue;
        private readonly IServerCheckPointHandler _serverCheckPointHandler;

        /// <summary>
        ///     Constructor initializing all the attributes.
        /// </summary>
        public ServerBoardStateManager()
        {
            _serverCheckPointHandler = new ServerCheckPointHandler();

            // initialize state maintaining structures
            _mapIdToBoardShape = new Dictionary<string, BoardShape>();
            _mapIdToQueueElement = new Dictionary<string, QueueElement>();
            _priorityQueue = new BoardPriorityQueue();
            Trace.WriteLine("ServerBoardStateManager.ServerBoardStateManager: Initialized attributes.");
        }

        /// <summary>
        ///     Fetches the checkpoint and updates the server state.
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <param name="userId">The user who requested the checkpoint.</param>
        /// <returns>BoardServerShape containing all shape information to broadcast to all clients.</returns>
        public BoardServerShape FetchCheckpoint(int checkpointNumber, string userId)
        {
            try
            {
                var boardShapes = _serverCheckPointHandler.FetchCheckpoint(checkpointNumber);
                BoardServerShape boardServerShape =
                    new(boardShapes, Operation.FETCH_CHECKPOINT, userId, checkpointNumber);
                Trace.WriteLine("ServerBoardStateManager.FetchCheckpoint: Checkpoint fetched.");
                return boardServerShape;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ServerBoardStateManager.FetchCheckpoint: Exception occurred.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Fetches the state of the server to send to newly joined user.
        /// </summary>
        /// <param name="userId">The newly joined user who requested the state fetch.</param>
        /// <returns>BoardServerShape containing all shape updates and no. of checkpoints to send to the client.</returns>
        public BoardServerShape FetchState(string userId)
        {
            try
            {
                // convert current state into sorted list of shapes (increasing timestamp)
                var boardShapes = GetOrderedList();

                // number of checkpoints currently saved at the server
                var checkpointNumber = GetCheckpointsNumber();
                BoardServerShape serverShape = new(boardShapes, Operation.FETCH_STATE, userId, checkpointNumber);
                return serverShape;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ServerBoardStateManager.FetchState: Exception occurred.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Gets the number of checkpoints saved at server.
        /// </summary>
        /// <returns>Number specifying the number of checkpoints.</returns>
        public int GetCheckpointsNumber()
        {
            return _serverCheckPointHandler.GetCheckpointsNumber();
        }

        /// <summary>
        ///     Saves the checkpoint at the server.
        /// </summary>
        /// <param name="userId">Id of the user who requested to save this checkpoint.</param>
        /// <returns>BoardServerShape object specifying the checkpoint number which was created.</returns>
        public BoardServerShape SaveCheckpoint(string userId)
        {
            try
            {
                // sending sorted list of shapes to ServerCheckPointHandler
                var boardShapes = GetOrderedList();
                var checkpointNumber = _serverCheckPointHandler.SaveCheckpoint(boardShapes, userId);
                BoardServerShape boardServerShape = new(null, Operation.CREATE_CHECKPOINT, userId, checkpointNumber);
                Trace.WriteLine("ServerBoardStateManager.SaveCheckpoint: Checkpoint saved.");
                return boardServerShape;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ServerBoardStateManager.SaveCheckpoint: Exception occurred.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Saves the updates on state at the server.
        /// </summary>
        /// <param name="boardServerShape">Object containing the update information for shape.</param>
        /// <returns>Boolean to indicate success status of update.</returns>
        public bool SaveUpdate(BoardServerShape boardServerShape)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Converts current state to sorted list of BoardShapes, sorted in increasing order of timestamp
        /// </summary>
        /// <returns>Sorted list of BoardShape</returns>
        private List<BoardShape> GetOrderedList()
        {
            List<QueueElement> queueElements = new();
            List<BoardShape> boardShapes = new();

            // Getting all elements from the queue
            while (_priorityQueue.GetSize() != 0) queueElements.Add(_priorityQueue.Extract());

            // Adding elements in the list in inceasing order of their timestamp
            for (var i = queueElements.Count - 1; i >= 0; i--) boardShapes.Add(_mapIdToBoardShape[queueElements[i].Id]);

            // inserting element back in the priority queue
            // reverse order is better in terms of better average time-complexity
            for (var i = 0; i < queueElements.Count; i++) _priorityQueue.Insert(queueElements[i]);

            return boardShapes;
        }
    }
}