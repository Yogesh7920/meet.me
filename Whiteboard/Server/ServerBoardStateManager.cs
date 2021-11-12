/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/12/2021
 * Date Modified: 11/12/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Server-side state management for Whiteboard.
    /// Non-extendable class having functionalities to maintain state at server side. 
    /// </summary>
    public sealed class ServerBoardStateManager : IServerBoardStateManager
    {
        private readonly IServerCheckPointHandler _serverCheckPointHandler;

        // data structures to maintain state
        private readonly Dictionary<string, BoardShape> _mapIdToBoardShape;
        private readonly Dictionary<string, QueueElement> _mapIdToQueueElement;
        private readonly BoardPriorityQueue _priorityQueue;

        /// <summary>
        /// Constructor initializing all the attributes. 
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
        /// Fetches the checkpoint and updates the server state. 
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <param name="userId">The user who requested the checkpoint.</param>
        /// <returns>BoardServerShape containing all shape information to broadcast to all clients.</returns>
        public BoardServerShape FetchCheckpoint([NotNull] int checkpointNumber, [NotNull] string userId)
        {
            try
            {
                List<BoardShape> boardShapes = _serverCheckPointHandler.FetchCheckpoint(checkpointNumber);
                BoardServerShape boardServerShape = new(boardShapes, Operation.FETCH_CHECKPOINT, userId, checkpointNumber);
                Trace.WriteLine("ServerBoardStateManager.FetchCheckpoint: Checkpoint fetched.");
                return boardServerShape;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ServerBoardStateManager.FetchCheckpoint: Exception occurred.");
                Trace.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Fetches the state of the server to send to newly joined user. 
        /// </summary>
        /// <param name="userId">The newly joined user who requested the state fetch.</param>
        /// <returns>BoardServerShape containing all shape updates and no. of checkpoints to send to the client.</returns>
        public BoardServerShape FetchState([NotNull] string userId)
        {
            try
            {
                // convert current state into sorted list of shapes (increasing timestamp)
                List<BoardShape> boardShapes = GetOrderedList();

                // number of checkpoints currently saved at the server
                int checkpointNumber = GetCheckpointsNumber();
                BoardServerShape serverShape = new(boardShapes, Operation.FETCH_STATE, userId, checkpointNumber);
                return serverShape;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ServerBoardStateManager.FetchState: Exception occurred.");
                Trace.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets the number of checkpoints saved at server. 
        /// </summary>
        /// <returns>Number specifying the number of checkpoints.</returns>
        public int GetCheckpointsNumber()
        {
            return _serverCheckPointHandler.GetCheckpointsNumber();
        }

        /// <summary>
        /// Saves the checkpoint at the server. 
        /// </summary>
        /// <param name="userId">Id of the user who requested to save this checkpoint.</param>
        /// <returns>BoardServerShape object specifying the checkpoint number which was created.</returns>
        public BoardServerShape SaveCheckpoint([NotNull] string userId)
        {
            try
            {
                // sending sorted list of shapes to ServerCheckPointHandler
                List<BoardShape> boardShapes = GetOrderedList();
                int checkpointNumber = _serverCheckPointHandler.SaveCheckpoint(boardShapes, userId);
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
        /// Saves the updates on state at the server.
        /// </summary>
        /// <param name="boardServerShape">Object containing the update information for shape.</param>
        /// <returns>Boolean to indicate success status of update.</returns>
        public bool SaveUpdate([NotNull] BoardServerShape boardServerShape)
        {
            try
            {
                // expecting one operation update at a time
                if (boardServerShape.ShapeUpdates.Count != BoardConstants.SINGLE_UPDATE_SIZE)
                {
                    throw new NotSupportedException("Multiple shape operation.");
                }

                if (boardServerShape.OperationFlag == Operation.CREATE)
                {
                    // Get the board shape and create a new queue element
                    BoardShape boardShape = boardServerShape.ShapeUpdates[0];
                    QueueElement queueElement = new(boardShape.Uid, boardShape.LastModifiedTime);

                    // Checking pre-conditions
                    PreConditionChecker(boardShape, Operation.CREATE);

                    // Add the update in respective data structures
                    _mapIdToBoardShape.Add(boardShape.Uid, boardShape);
                    _priorityQueue.Insert(queueElement);
                    _mapIdToQueueElement.Add(boardShape.Uid, queueElement);
                    
                    return true;
                }

                else if(boardServerShape.OperationFlag == Operation.MODIFY)
                {
                    // Get the modified board shape
                    BoardShape boardShape = boardServerShape.ShapeUpdates[0];

                    // Checking pre-conditions
                    PreConditionChecker(boardShape, Operation.MODIFY);

                    // Modify the update in respective data structures
                    _mapIdToBoardShape[boardShape.Uid] = boardShape;
                    QueueElement queueElement = _mapIdToQueueElement[boardShape.Uid];
                    _priorityQueue.IncreaseTimestamp(queueElement, boardShape.LastModifiedTime);

                    return true;
                }

                else if(boardServerShape.OperationFlag == Operation.DELETE)
                {
                    // Get the shape to be deleted
                    BoardShape boardShape = boardServerShape.ShapeUpdates[0];

                    // Checking pre-conditions
                    PreConditionChecker(boardShape, Operation.DELETE);

                    // Delete from the respective data structures
                    _mapIdToBoardShape.Remove(boardShape.Uid);
                    _priorityQueue.DeleteElement(_mapIdToQueueElement[boardShape.Uid]);
                    _mapIdToQueueElement.Remove(boardShape.Uid);

                    return true;
                }
                else
                {
                    // No other flags are supported in SaveUpdate.
                    throw new NotSupportedException("Operation not supported.");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ServerBoardStateManager.SaveUpdate: Exception occurred.");
                Trace.WriteLine(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Converts current state to sorted list of BoardShapes, sorted in increasing order of timestamp
        /// </summary>
        /// <returns>Sorted list of BoardShape</returns>
        private List<BoardShape> GetOrderedList()
        {
            List<QueueElement> queueElements = new();
            List<BoardShape> boardShapes = new();

            // Getting all elements from the queue
            while (_priorityQueue.GetSize() != BoardConstants.EMPTY_SIZE)
            {
                queueElements.Add(_priorityQueue.Extract());
            }

            // Adding elements in the list in inceasing order of their timestamp
            for (int i = queueElements.Count - 1; i >= 0; i--)
            {
                boardShapes.Add(_mapIdToBoardShape[queueElements[i].Id]);
            }

            // inserting element back in the priority queue
            // reverse order is better in terms of better average time-complexity
            for (int i = 0; i < queueElements.Count; i++)
            {
                _priorityQueue.Insert(queueElements[i]);
            }

            return boardShapes;
        }

        /// <summary>
        /// Checks pre-condtions for SaveUpdate
        /// </summary>
        /// <param name="boardShape">BoardShape signifying the output.</param>
        /// <param name="operation">The operation specified in BoardServerShape containing boardShape.</param>
        private void PreConditionChecker(BoardShape boardShape, [NotNull] Operation operation)
        {
            if (boardShape == null)
            {
                Trace.WriteLine("ServerBoardStateManager.PreConditionChecker: Null BoardShape");
                throw new NullReferenceException();
            }

            if(operation != boardShape.RecentOperation)
            {
                Trace.WriteLine("ServerBoardStateManager.PreConditionChecker: Operation equality condition failed.");
                throw new InvalidOperationException("Operation type should be same.");
            }
            
            if(operation == Operation.CREATE)
            {
                // Remove the key,value pair from the map if an entry with that key already exist.
                if (_mapIdToBoardShape.ContainsKey(boardShape.Uid) || _mapIdToQueueElement.ContainsKey(boardShape.Uid))
                {
                    Trace.WriteLine("ServerBoardStateManager.PreConditionChecker: Create condition failed.");
                    throw new InvalidOperationException("Shape with same id already exists.");
                }
            }
            else if(operation == Operation.DELETE || operation == Operation.MODIFY)
            {
                // The maps should contain this shape's UID. 
                if (!_mapIdToBoardShape.ContainsKey(boardShape.Uid) || !_mapIdToQueueElement.ContainsKey(boardShape.Uid))
                {
                    Trace.WriteLine("ServerBoardStateManager.PreConditionCheker: Shape to be deleted/modified doesn't exist.");
                    throw new KeyNotFoundException("Shape id not found.");
                }
            }
            else
            {
                Trace.WriteLine("ServerBoardStateManager.PreConditionChecker: Unexpected operation.");
                throw new InvalidOperationException();
            }
        }
    }
}