/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/11/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Whiteboard
{
    /// <summary>
    ///     Client-side state management for Whiteboard.
    ///     Non-extendable class having functionalities to maintain state at client side.
    /// </summary>
    public sealed class ClientBoardStateManager : IClientBoardStateManager, IClientBoardStateManagerInternal,
        IServerUpdateListener
    {
        // Attribute holding the single instance of this class. 
        private static ClientBoardStateManager s_instance;

        // Check if running as part of NUnit
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        // lock for the state
        private readonly object _stateLock = new();

        // no. of states till the client can undo
        private readonly int _undoRedoCapacity = BoardConstants.UndoRedoStackSize;

        // no. of checkpoints stored on the server
        private int _checkpointsNumber;

        // Instances of other classes
        private IClientBoardCommunicator _clientBoardCommunicator;
        private IClientCheckPointHandler _clientCheckPointHandler;

        // Clients subscribed to state manager
        private Dictionary<string, IClientBoardStateListener> _clients;

        // The current base state.
        private int _currentCheckpointState;

        // Attribute holding current user id
        private string _currentUserId;

        // To maintain the Shape-Ids that were recently deleted. 
        // Required in cases of Delete and then Modify situations when updates are not yet reached to all clients
        private HashSet<string> _deletedShapeIds;

        // data structures to maintain state
        private Dictionary<string, BoardShape> _mapIdToBoardShape;
        private Dictionary<string, QueueElement> _mapIdToQueueElement;
        private BoardPriorityQueue _priorityQueue;
        private BoardStack _redoStack;

        // data structures required for undo-redo
        private BoardStack _undoStack;

        // The level of user.
        private int _userLevel;

        /// <summary>
        ///     Private constructor.
        /// </summary>
        private ClientBoardStateManager()
        {
        }

        /// <summary>
        ///     Getter for s_instance.
        /// </summary>
        public static ClientBoardStateManager Instance
        {
            get
            {
                Trace.Indent();
                Trace.WriteLineIf(s_instance == null,
                    "[Whiteboard] ClientBoardStateManager.Instance: Creating and storing a new instance.");

                // Create a new instance if not yet created.
                s_instance = s_instance is null ? new ClientBoardStateManager() : s_instance;
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.Instance: Returning the stored instance.s");
                Trace.Unindent();
                return s_instance;
            }
        }

        /// <summary>
        ///     Fetches the checkpoint from server and updates the current state.
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <returns>List of UXShapes for UX to render.</returns>
        public void FetchCheckpoint([NotNull] int checkpointNumber)
        {
            Trace.WriteLine("[Whiteboard] ClientBoardStateManager.FetchCheckpoint: Fetch checkpoint request received.");
            _clientCheckPointHandler.FetchCheckpoint(checkpointNumber, _currentUserId, _currentCheckpointState);
        }

        /// <summary>
        ///     Creates and saves checkpoint.
        /// </summary>
        /// <returns>The number/identifier of the created checkpoint.</returns>
        public void SaveCheckpoint()
        {
            Trace.WriteLine("[Whiteboard] ClientBoardStateManager.SaveCheckpoint: Create checkpoint request received.");
            _clientCheckPointHandler.SaveCheckpoint(_currentUserId, _currentCheckpointState);
        }

        /// <summary>
        ///     Sets the current user id.
        /// </summary>
        /// <param name="userId">user Id of the current user.</param>
        public void SetUser([NotNull] string userId)
        {
            Trace.WriteLine("[Whiteboard] ClientBoardStateManager.SetUser: User-Id is set.");
            _currentUserId = userId;
        }

        /// <summary>
        ///     Initializes state managers attributes.
        /// </summary>
        public void Start()
        {
            // initializing all attributes 
            _checkpointsNumber = BoardConstants.EmptySize;
            _currentCheckpointState = BoardConstants.InitialCheckpointState;
            _clientBoardCommunicator = ClientBoardCommunicator.Instance;
            _clientCheckPointHandler = new ClientCheckPointHandler();
            _clients = new Dictionary<string, IClientBoardStateListener>();
            _deletedShapeIds = new HashSet<string>();
            _currentUserId = null;
            _userLevel = BoardConstants.LowUserLevel;

            InitializeDataStructures();

            // subscribing to ClientBoardCommunicator
            _clientBoardCommunicator.Subscribe(this);
            Trace.WriteLine("[Whiteboard] ClientBoardStateManager.Start: Initialization done.");
        }

        /// <summary>
        ///     Subscribes to notifications from ClientBoardStateManager to get updates.
        /// </summary>
        /// <param name="listener">The subscriber. </param>
        /// <param name="identifier">The identifier of the subscriber. </param>
        public void Subscribe([NotNull] IClientBoardStateListener listener, [NotNull] string identifier)
        {
            try
            {
                lock (_stateLock)
                {
                    // Cleaning current state since new state will be called
                    NullifyDataStructures();

                    // Adding subscriber 
                    _clients.Add(identifier, listener);
                }

                // Creating BoardServerShape object and requesting communicator
                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.Subscribe: Sending fetch state request to communicator.");
                BoardServerShape boardServerShape = new(null, Operation.FetchState, _currentUserId,
                    currentCheckpointState: _currentCheckpointState);
                _clientBoardCommunicator.Send(boardServerShape);

                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.Subscribe: Fetch state request sent to communicator.");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.Subscribe: Exception occurred.");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     Clears the whiteboard state.
        /// </summary>
        /// <returns>True to freeze state.</returns>
        public bool ClearWhiteBoard()
        {
            Trace.WriteLine(
                "[Whiteboard] ClientBoardStateManager.ClearWhiteBoard: Sending Clear_State request to server.");

            // Only user with higher user level can clear complete state.
            if (_userLevel == BoardConstants.HighUserLevel)
            {
                // reset state in sync to currentCheckpointState = 0.
                _clientBoardCommunicator.Send(new BoardServerShape(null, Operation.ClearState, _currentUserId,
                    currentCheckpointState: BoardConstants.InitialCheckpointState));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Does the redo operation for client.
        /// </summary>
        /// <returns>List of UXShapes for UX to render.</returns>
        public List<UXShape> DoRedo()
        {
            try
            {
                lock (_stateLock)
                {
                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.DoRedo: Redo is called.");

                    if (_redoStack.IsEmpty())
                    {
                        Trace.WriteLine("[Whiteboard] ClientBoardStateManager.DoRedo: Stack is empty.");
                        return null;
                    }

                    // updates for UX
                    List<UXShape> uXShapes = new();
                    var flag = 0;

                    // if the shape got deleted from server updates, user can't undo-redo that, hence do redo on next entry.s
                    while (uXShapes.Count == 0)
                    {
                        if (flag != 0) _undoStack.Pop();
                        if (_redoStack.IsEmpty())
                        {
                            Trace.WriteLine(
                                "[Whiteboard] ClientBoardStateManager.DoRedo: Stack got emptied. No valid shape available in state.");
                            return null;
                        }

                        // get the top element and put it in undo stack
                        var tuple = _redoStack.Top();
                        _redoStack.Pop();
                        _undoStack.Push(tuple.Item2?.Clone(), tuple.Item1?.Clone());
                        uXShapes = UndoRedoRollback(tuple);
                    }

                    return uXShapes;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.DoRedo: An exception occured.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Does the undo operation for client.
        /// </summary>
        /// <returns>List of UXShapes for UX to render.</returns>
        public List<UXShape> DoUndo()
        {
            try
            {
                lock (_stateLock)
                {
                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.DoUndo: Undo is called.");

                    if (_undoStack.IsEmpty())
                    {
                        Trace.WriteLine("[Whiteboard] ClientBoardStateManager.DoUndo: Stack is empty.");
                        return null;
                    }

                    // updates for UX
                    List<UXShape> uXShapes = new();
                    var flag = 0;

                    // if the shape got deleted from server updates, user can't undo-redo that, hence do redo on next entry.s
                    while (uXShapes.Count == 0)
                    {
                        if (flag != 0) _redoStack.Pop();
                        if (_undoStack.IsEmpty())
                        {
                            Trace.WriteLine(
                                "[Whiteboard] ClientBoardStateManager.DoUndo: Stack got emptied. No valid shape available in state.");
                            return null;
                        }

                        // get the top element and put it in redo stack
                        var tuple = _undoStack.Top();
                        _undoStack.Pop();
                        _redoStack.Push(tuple.Item2?.Clone(), tuple.Item1?.Clone());
                        uXShapes = UndoRedoRollback(tuple);
                    }

                    return uXShapes;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.DoUndo: An exception occured.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Fetches the BoardShape object from the map.
        /// </summary>
        /// <param name="id">Unique identifier for a BoardShape object.</param>
        /// <returns>BoardShape object with unique id equal to id.</returns>
        public BoardShape GetBoardShape([NotNull] string id)
        {
            try
            {
                lock (_stateLock)
                {
                    if (_mapIdToBoardShape.ContainsKey(id)) return _mapIdToBoardShape[id];
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.GetBoardShape: Exception occured.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Provides the current user's id.
        /// </summary>
        /// <returns>The user id of current user.</returns>
        public string GetUser()
        {
            return _currentUserId ?? throw new NullReferenceException("Current user-id not set");
        }

        /// <summary>
        ///     Saves the update on a shape in the state and sends it to server for broadcast.
        /// </summary>
        /// <param name="boardShape">The object describing shape.</param>
        /// <returns>Boolean to indicate success status of update.</returns>
        public bool SaveOperation([NotNull] BoardShape boardShape)
        {
            try
            {
                if (boardShape.RecentOperation == Operation.Create)
                {
                    lock (_stateLock)
                    {
                        // Checking pre-conditions for Create
                        PreConditionChecker(Operation.Create, boardShape.Uid);

                        // New QueueElement
                        QueueElement queueElement = new(boardShape.Uid, boardShape.LastModifiedTime);

                        // Add the update in respective data structures
                        _mapIdToBoardShape.Add(boardShape.Uid, boardShape);
                        _priorityQueue.Insert(queueElement);
                        _mapIdToQueueElement.Add(boardShape.Uid, queueElement);

                        // create a deep copy and put it in undo stack
                        _undoStack.Push(null, boardShape.Clone());

                        _deletedShapeIds.Remove(boardShape.Uid);
                        Trace.WriteLine(
                            "[Whiteboard] ClientBoardStateManager.SaveOperation: State updated for Create operation.");
                    }

                    // Send the update to server
                    _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShape},
                        Operation.Create, _currentUserId, currentCheckpointState: _currentCheckpointState));
                    return true;
                }

                if (boardShape.RecentOperation == Operation.Modify)
                {
                    lock (_stateLock)
                    {
                        // A case when server thread got the lock and deleted the shape just before the client thread
                        if (_deletedShapeIds.Contains(boardShape.Uid))
                        {
                            Trace.WriteLine(
                                "[Whiteboard] ClientBoardStateManager.SaveOperation: Modify on deleted shape.");
                            return false;
                        }

                        // Checking pre-conditions for Modify
                        PreConditionChecker(Operation.Modify, boardShape.Uid);

                        // create a deep copy and add previous & new one to undo-stack
                        _undoStack.Push(_mapIdToBoardShape[boardShape.Uid].Clone(), boardShape.Clone());

                        // Modify accordingly in respective data structures
                        _mapIdToBoardShape[boardShape.Uid] = boardShape;
                        var queueElement = _mapIdToQueueElement[boardShape.Uid];
                        _priorityQueue.IncreaseTimestamp(queueElement, boardShape.LastModifiedTime);
                        Trace.WriteLine(
                            "[Whiteboard] ClientBoardStateManager.SaveOperation: State updated for Modify operation.");
                    }

                    // Send the update to server
                    _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShape},
                        Operation.Modify, _currentUserId, currentCheckpointState: _currentCheckpointState));
                    GC.Collect();
                    return true;
                }

                if (boardShape.RecentOperation == Operation.Delete)
                {
                    lock (_stateLock)
                    {
                        // A case when server thread got the lock and deleted the shape just before the client thread
                        if (_deletedShapeIds.Contains(boardShape.Uid))
                        {
                            Trace.WriteLine(
                                "[Whiteboard] ClientBoardStateManager.SaveOperation: Delete on deleted shape.");
                            return false;
                        }

                        // Checking pre-conditions for Delete
                        PreConditionChecker(Operation.Delete, boardShape.Uid);

                        // create a deep copy and push it in undo stack
                        _undoStack.Push(_mapIdToBoardShape[boardShape.Uid].Clone(), null);

                        // Delete from respective data structures
                        _mapIdToBoardShape.Remove(boardShape.Uid);
                        _priorityQueue.DeleteElement(_mapIdToQueueElement[boardShape.Uid]);
                        _mapIdToQueueElement.Remove(boardShape.Uid);
                        _deletedShapeIds.Add(boardShape.Uid);

                        Trace.WriteLine(
                            "[Whiteboard] ClientBoardStateManager.SaveOperation: State updated for Delete operation.");
                    }

                    // Send the update to server
                    _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShape},
                        Operation.Delete, _currentUserId, currentCheckpointState: _currentCheckpointState));
                    GC.Collect();
                    return true;
                }

                // No other flags are supported in SaveOperation.
                throw new NotSupportedException("Operation not supported.");
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.SaveOperation: Exception occurred.");
                Trace.WriteLine(e.Message);
            }

            return false;
        }

        /// <summary>
        ///     Sets the user level.
        /// </summary>
        /// <param name="userLevel">The user level.</param>
        public void SetUserLevel(int userLevel)
        {
            _userLevel = userLevel;
        }

        /// <summary>
        ///     Manages state and notifies UX on receiving an update from ClientBoardCommunicator.
        /// </summary>
        /// <param name="serverUpdate">BoardServerShapes signifying the update.</param>
        public void OnMessageReceived([NotNull] BoardServerShape serverUpdate)
        {
            try
            {
                // a case of state fetching for newly joined client
                if (serverUpdate.OperationFlag == Operation.FetchState && serverUpdate.RequesterId == _currentUserId)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: FetchState (subscribe) request's result arrived.");
                    lock (_stateLock)
                    {
                        // converting network update to UXShapes and sending them to UX
                        var uXShapeHelpers = UpdateStateOnFetch(serverUpdate);
                        NotifyClients(uXShapeHelpers);
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }
                // a case of checkpoint fetching
                else if (serverUpdate.OperationFlag == Operation.FetchCheckpoint)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: FetchCheckpoint request's result arrived.");
                    lock (_stateLock)
                    {
                        // Nullify current state
                        NullifyDataStructures();

                        // converting network update to UXShapes and sending them to UX
                        var uXShapeHelpers = UpdateStateOnFetch(serverUpdate);
                        NotifyClients(uXShapeHelpers);
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }
                // a case of new checkpoint created
                else if (serverUpdate.OperationFlag == Operation.CreateCheckpoint)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: CreateCheckpoint request's result arrived.");

                    // checking sync conditions
                    CheckCountAndCurrentCheckpoint(serverUpdate, false);

                    lock (_stateLock)
                    {
                        // update number of checkpoints in state
                        _checkpointsNumber = serverUpdate.CheckpointNumber;
                        _clientCheckPointHandler.CheckpointNumber = _checkpointsNumber;

                        // notify UX to display new number
                        NotifyClients(new List<UXShapeHelper> {new(_checkpointsNumber, Operation.CreateCheckpoint)});
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }
                // when other users create a shape
                else if (serverUpdate.OperationFlag == Operation.Create && serverUpdate.RequesterId != _currentUserId)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: Create request's result arrived.");

                    // checking conditions on server update.
                    CheckCountAndCurrentCheckpoint(serverUpdate);

                    lock (_stateLock)
                    {
                        NotifyClients(ServerOperationUpdate(serverUpdate.ShapeUpdates[0], Operation.Create));
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }

                else if (serverUpdate.OperationFlag == Operation.Modify && serverUpdate.RequesterId != _currentUserId)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: Modify request's result arrived.");

                    // checking conditions on server update.
                    CheckCountAndCurrentCheckpoint(serverUpdate);

                    // Client deleted the shape but server didn't receive it yet.
                    if (_deletedShapeIds.Contains(serverUpdate.ShapeUpdates[0].Uid))
                    {
                        Trace.WriteLine(
                            "[Whiteboard] ClientBoardStateManager.OnMessageReceived: Modify on deleted shape.");
                        return;
                    }

                    lock (_stateLock)
                    {
                        NotifyClients(ServerOperationUpdate(serverUpdate.ShapeUpdates[0], Operation.Modify));
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }
                else if (serverUpdate.OperationFlag == Operation.Delete && serverUpdate.RequesterId != _currentUserId)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: Delete request's result arrived.");

                    // checking conditions on server update.
                    CheckCountAndCurrentCheckpoint(serverUpdate);

                    // Client just deleted the shape and server didn't receive the request yet.
                    if (_deletedShapeIds.Contains(serverUpdate.ShapeUpdates[0].Uid))
                    {
                        Trace.WriteLine(
                            "[Whiteboard] ClientBoardStateManager.OnMessageReceived: Delete on deleted shape.");
                        return;
                    }

                    lock (_stateLock)
                    {
                        NotifyClients(ServerOperationUpdate(serverUpdate.ShapeUpdates[0], Operation.Delete));
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }
                else if (serverUpdate.OperationFlag == Operation.ClearState)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.OnMessageReceived: ClearState request's result arrived.");

                    // checking conditions on server update.
                    CheckCountAndCurrentCheckpoint(serverUpdate, false, false);

                    lock (_stateLock)
                    {
                        // set checkpoint state
                        _currentCheckpointState = BoardConstants.InitialCheckpointState;

                        // clear the state and notify the UX for the same
                        NullifyDataStructures();
                        NotifyClients(new List<UXShapeHelper> {new(_checkpointsNumber, Operation.ClearState)});
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Clients Notified.");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.OnMessageReceived: Exception occured");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     Function to set mock communicator and checkpointHandler when running in test mode.
        /// </summary>
        /// <param name="clientBoardCommunicator">ClientBoardCommunicator instance</param>
        /// <param name="clientCheckPointHandler">ClientCheckpointHandler instance</param>
        public void SetCommunicatorAndCheckpointHandler(IClientBoardCommunicator clientBoardCommunicator,
            IClientCheckPointHandler clientCheckPointHandler)
        {
            if (IsRunningFromNUnit)
            {
                _clientBoardCommunicator = clientBoardCommunicator;
                _clientCheckPointHandler = clientCheckPointHandler;
            }
        }

        /// <summary>
        ///     Initializes the data structures which are maintaining the state.
        /// </summary>
        /// <param name="initializeUndoRedo">Initialize undo-redo stacks or not. By default it is true.</param>
        private void InitializeDataStructures(bool initializeUndoRedo = true)
        {
            _mapIdToBoardShape = new Dictionary<string, BoardShape>();
            _mapIdToQueueElement = new Dictionary<string, QueueElement>();
            _priorityQueue = new BoardPriorityQueue();
            if (initializeUndoRedo)
            {
                _redoStack = new BoardStack(_undoRedoCapacity);
                _undoStack = new BoardStack(_undoRedoCapacity);
            }
        }

        /// <summary>
        ///     Nullifies all the state maintaining data structures.
        /// </summary>
        /// <param name="nullifyUndoRedo">Mullify undo-redo stacks or not. By default it is true.</param>
        private void NullifyDataStructures(bool nullifyUndoRedo = true)
        {
            // Emptying current state is equivalent to delete all shapes.
            foreach (var shapeId in _mapIdToBoardShape.Keys) _deletedShapeIds.Add(shapeId);

            _mapIdToBoardShape.Clear();
            _mapIdToQueueElement.Clear();
            _priorityQueue.Clear();
            if (nullifyUndoRedo)
            {
                _redoStack.Clear();
                _undoStack.Clear();
            }

            GC.Collect();
        }

        /// <summary>
        ///     Updates local state on Fetch State or Fetch Checkpoint from server.
        /// </summary>
        /// <param name="boardServerShape">BoardServerShape object having the whole update.</param>
        /// <returns>List of UXShape to notify client.</returns>
        private List<UXShapeHelper> UpdateStateOnFetch(BoardServerShape boardServerShape)
        {
            try
            {
                var boardShapes = boardServerShape.ShapeUpdates;
                List<UXShapeHelper> uXShapeHelpers = new();

                // Sorting boardShapes
                boardShapes.Sort(delegate(BoardShape boardShape1, BoardShape boardShape2)
                {
                    return boardShape1.LastModifiedTime.CompareTo(boardShape2.LastModifiedTime);
                });

                // updating checkpoint number for subscribe result
                if (boardServerShape.OperationFlag == Operation.FetchState)
                {
                    _checkpointsNumber = boardServerShape.CheckpointNumber;
                    _clientCheckPointHandler.CheckpointNumber = _checkpointsNumber;
                }

                // updating the state number so its in sync with server
                _currentCheckpointState = boardServerShape.CurrentCheckpointState;

                // updating state
                for (var i = 0; i < boardShapes.Count; i++)
                {
                    var boardShapeId = boardShapes[i].Uid;
                    // insert in id to BoardShape map
                    if (_mapIdToBoardShape.ContainsKey(boardShapeId) || _mapIdToQueueElement.ContainsKey(boardShapeId))
                        // if already there is some reference present, then raise an Exception
                        throw new Exception("Same board shape present twice. Problem with state fetching.");
                    _mapIdToBoardShape.Add(boardShapeId, boardShapes[i]);
                    // insert in priority queue and id to QueueElement map
                    QueueElement queueElement = new(boardShapeId, boardShapes[i].LastModifiedTime);
                    _mapIdToQueueElement.Add(boardShapeId, queueElement);
                    _priorityQueue.Insert(queueElement);

                    // The ids which were considered to be deleted
                    if (_deletedShapeIds.Contains(boardShapeId)) _deletedShapeIds.Remove(boardShapeId);
                    // converting BoardShape to UXShape and adding it in the list
                    uXShapeHelpers.Add(new UXShapeHelper(UXOperation.Create, boardShapes[i].MainShapeDefiner,
                        boardShapeId, _checkpointsNumber, boardServerShape.OperationFlag));
                }

                return uXShapeHelpers.Count == 0
                    ? new List<UXShapeHelper> {new(_checkpointsNumber, boardServerShape.OperationFlag)}
                    : uXShapeHelpers;
            }
            catch (Exception e)
            {
                NullifyDataStructures();
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.UpdateStateOnFetch: Exception occurred.");
                Trace.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Notifies clients with List of UXShapes.
        /// </summary>
        /// <param name="uXShapes">List of UX Shapes for UX to render</param>
        private void NotifyClients(List<UXShapeHelper> uXShapeHelpers)
        {
            try
            {
                lock (this)
                {
                    // Sending each client the updated UXShapes. 
                    foreach (var entry in _clients)
                    {
                        Trace.WriteLine("[Whiteboard] ClientBoardStateManager.NotifyClient: Notifying client.");
                        entry.Value.OnUpdateFromStateManager(uXShapeHelpers);
                    }

                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.NotifyClient: All clients notified.");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.NotifyClients: Exception occurred.");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     Checks pre-condtions for SaveOperation andOnMessageReceived
        /// </summary>
        /// <param name="operation">The operation to be performed.</param>
        /// <param name="id">Id of the shape.</param>
        private void PreConditionChecker(Operation operation, string id)
        {
            if (operation == Operation.Create)
            {
                if (_mapIdToBoardShape.ContainsKey(id) || _mapIdToQueueElement.ContainsKey(id))
                {
                    Trace.WriteLine("[Whiteboard] ClientBoardStateManager.SaveOperation: Create condition failed.");
                    throw new InvalidOperationException("Shape already exists");
                }
            }

            else if (operation == Operation.Modify || operation == Operation.Delete)
            {
                if (!_mapIdToBoardShape.ContainsKey(id) || !_mapIdToQueueElement.ContainsKey(id))
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.SaveOperation: Modify/Delete condition failed.");
                    throw new InvalidOperationException("Shape does not exist");
                }
            }
        }

        /// <summary>
        ///     Find all the shapes which were inserted after timestamp
        /// </summary>
        /// <param name="timestamp">Timestamp to compare and find later shapes.</param>
        /// <returns>A Tuple of list of BoardShapes and Elements of Priority Queue.</returns>
        private Tuple<List<BoardShape>, List<QueueElement>> LaterShapes([NotNull] DateTime timestamp)
        {
            List<BoardShape> boardShapes = new();
            List<QueueElement> queueElements = new();
            while (_priorityQueue.GetSize() > BoardConstants.EmptySize && _priorityQueue.Top().Timestamp > timestamp)
            {
                boardShapes.Add(_mapIdToBoardShape[_priorityQueue.Top().Id]);
                queueElements.Add(_priorityQueue.Extract());
            }

            boardShapes.Reverse();
            queueElements.Reverse();
            return new Tuple<List<BoardShape>, List<QueueElement>>(boardShapes, queueElements);
        }

        /// <summary>
        ///     Converts a list of BoardShapes to UXShapes
        /// </summary>
        /// <param name="boardShapes">BoardShapes to be converted.</param>
        /// <param name="uXOperation">UXoperation for each UXShape</param>
        /// <param name="operationFlag">Operation which requires these changes.</param>
        /// <param name="uXShapes">List of UXShapes in which new UXShapes will be added.</param>
        /// <returns>List of UXShapes corresponding to boardShapes.</returns>
        private List<UXShapeHelper> ToUXShapeHelpers(List<BoardShape> boardShapes, UXOperation uXOperation,
            Operation operationFlag, List<UXShapeHelper> uXShapeHelpers = null)
        {
            // if null then initialize
            if (uXShapeHelpers == null) uXShapeHelpers = new List<UXShapeHelper>();

            // convert all BoardShapes to UXShapes
            for (var i = 0; i < boardShapes.Count; i++)
                uXShapeHelpers.Add(new UXShapeHelper(uXOperation, boardShapes[i].MainShapeDefiner, boardShapes[i].Uid,
                    _checkpointsNumber, operationFlag));
            return uXShapeHelpers;
        }

        /// <summary>
        ///     Updates state with boardShape and operation and prepare List of UXShapes to send to UX.
        /// </summary>
        /// <param name="boardShape">The boardShape to be updated.</param>
        /// <param name="operation">The operation to be performed.</param>
        /// <returns>List of UXShapes to update UX.</returns>
        private List<UXShapeHelper> ServerOperationUpdate([NotNull] BoardShape boardShape,
            [NotNull] Operation operation)
        {
            // Only Create, Modify and Delete is supported by this function
            if (operation != Operation.Create && operation != Operation.Delete && operation != Operation.Modify)
                throw new InvalidOperationException("Operation type not supported.");

            // Recent operation should match the operation desired.
            if (boardShape.RecentOperation != operation)
                throw new InvalidOperationException("Operation type should be same.");

            // checking pre-conditions 
            PreConditionChecker(operation, boardShape.Uid);

            if (operation == Operation.Delete)
            {
                // updating data structures
                _priorityQueue.DeleteElement(_mapIdToQueueElement[boardShape.Uid]);
                _mapIdToQueueElement.Remove(boardShape.Uid);
                var tempShape = _mapIdToBoardShape[boardShape.Uid].Clone();
                tempShape.RecentOperation = Operation.Delete;
                _mapIdToBoardShape.Remove(boardShape.Uid);
                _deletedShapeIds.Add(boardShape.Uid);

                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.ServerOperationUpdate: Delete case - state successfully updated.");
                return new List<UXShapeHelper>
                {
                    new(UXOperation.Delete, tempShape.MainShapeDefiner, tempShape.Uid, operationType: Operation.Delete)
                };
            }

            // Shapes having last modified time before the current update needs to be deleted in UX first
            var tuple = LaterShapes(boardShape.LastModifiedTime);
            var uXShapeHelpers = ToUXShapeHelpers(tuple.Item1, UXOperation.Delete, operation);

            if (operation == Operation.Create)
            {
                // update data structures
                _mapIdToBoardShape.Add(boardShape.Uid, boardShape);
                QueueElement queueElement = new(boardShape.Uid, boardShape.LastModifiedTime);
                _priorityQueue.Insert(queueElement);
                _mapIdToQueueElement.Add(boardShape.Uid, queueElement);
                _deletedShapeIds.Remove(boardShape.Uid);

                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.ServerOperationUpdate: Create case - state successfully updated.");
            }
            else
            {
                // delete previous shape
                uXShapeHelpers.Add(new UXShapeHelper(UXOperation.Delete,
                    _mapIdToBoardShape[boardShape.Uid].MainShapeDefiner, boardShape.Uid,
                    operationType: Operation.Modify));

                // update data structures
                _mapIdToBoardShape[boardShape.Uid] = boardShape;
                _priorityQueue.IncreaseTimestamp(_mapIdToQueueElement[boardShape.Uid], boardShape.LastModifiedTime);

                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.ServerOperationUpdate: Modify case - state successfully updated.");
            }

            // Inserting new shape and reinserting temporarily deleted shapes
            uXShapeHelpers.Add(new UXShapeHelper(UXOperation.Create, boardShape.MainShapeDefiner, boardShape.Uid,
                _checkpointsNumber, operation));
            uXShapeHelpers = ToUXShapeHelpers(tuple.Item1, UXOperation.Create, operation, uXShapeHelpers);

            // populating the priority queue back
            _priorityQueue.Insert(tuple.Item2);
            return uXShapeHelpers;
        }

        /// <summary>
        ///     Does a state rollback operation for undo-redo.
        /// </summary>
        /// <param name="tuple">Tuple containing the previous state of a shape and latest state of a shape.</param>
        /// <returns>List of UXShapes to notify UX of the change.</returns>
        private List<UXShape> UndoRedoRollback([NotNull] Tuple<BoardShape, BoardShape> tuple)
        {
            // both can't be null
            if (tuple.Item1 == null && tuple.Item2 == null)
                throw new Exception("Both items in tuples are null.");

            // when original operation was create

            if (tuple.Item1 == null && tuple.Item2 != null)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Case of rollback on create.");

                // clone the item and mark it to delete
                var boardShape = tuple.Item2.Clone();
                boardShape.RecentOperation = Operation.Delete;

                // If server update just deleted the object then return empty list
                if (_deletedShapeIds.Contains(boardShape.Uid))
                {
                    Trace.WriteLine(
                        "[Whiteboard] ClientBoardStateManager.UndoRedoRollback: The item got deleted from server update. Delete on deleted.");
                    return new List<UXShape>();
                }

                // adding the id to set
                _deletedShapeIds.Add(boardShape.Uid);

                // send update to server
                _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShape}, Operation.Delete,
                    _currentUserId, currentCheckpointState: _currentCheckpointState));
                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Sent delete request to server.");

                // update state and send UXShapes to UX
                return UXShape.ToUXShape(ServerOperationUpdate(boardShape, Operation.Delete));
            }

            // when original operation was delete

            if (tuple.Item1 != null && tuple.Item2 == null)
            {
                Trace.WriteLine("[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Case of rollback on delete.");

                // clone the item and mark it to create
                var boardShape = tuple.Item1.Clone();
                boardShape.RecentOperation = Operation.Create;

                // send update to server
                _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShape}, Operation.Create,
                    _currentUserId, currentCheckpointState: _currentCheckpointState));
                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Sent create request to server.");

                // update state and send UXShapes to UX
                return UXShape.ToUXShape(ServerOperationUpdate(boardShape, Operation.Create));
            }

            // when original operation was modify

            Trace.WriteLine("[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Case of rollback on modify.");

            // mark new one to delete and previous one to create
            var boardShapePrev = tuple.Item1.Clone();
            var boardShapeNew = tuple.Item2.Clone();
            boardShapeNew.RecentOperation = Operation.Delete;
            boardShapePrev.RecentOperation = Operation.Create;

            // If server update just deleted the object then return empty list
            if (_deletedShapeIds.Contains(boardShapeNew.Uid))
            {
                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.UndoRedoRollback: The item got deleted from server update. Delete on deleted.");
                return new List<UXShape>();
            }

            // send updates to server
            _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShapeNew}, Operation.Delete,
                _currentUserId, currentCheckpointState: _currentCheckpointState));
            Trace.WriteLine(
                "[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Sent delete request to server for new.");
            _clientBoardCommunicator.Send(new BoardServerShape(new List<BoardShape> {boardShapePrev}, Operation.Create,
                _currentUserId, currentCheckpointState: _currentCheckpointState));
            Trace.WriteLine(
                "[Whiteboard] ClientBoardStateManager.UndoRedoRollback: Sent create request to server for old.");

            // Get respective UXShapes and update state
            var uXShapes = UXShape.ToUXShape(ServerOperationUpdate(boardShapeNew, Operation.Delete));
            uXShapes.AddRange(UXShape.ToUXShape(ServerOperationUpdate(boardShapePrev, Operation.Create)));
            return uXShapes;
        }

        /// <summary>
        ///     Checks the update count and checkpointState conditions.
        /// </summary>
        /// <param name="serverUpdate">The BoardServerShape containing the update. </param>
        /// <param name="count">Checks update count condition if true.</param>
        /// <param name="checkpointState">Checks current checkpoint state condition if true.</param>
        private void CheckCountAndCurrentCheckpoint(BoardServerShape serverUpdate, bool count = true,
            bool checkpointState = true)
        {
            // only single update is supported.
            if (count && serverUpdate.ShapeUpdates.Count != BoardConstants.SingleUpdateSize)
                throw new NotSupportedException("Multiple Shape Operation.");

            // state number should match with the server
            if (checkpointState && serverUpdate.CurrentCheckpointState != _currentCheckpointState)
            {
                Trace.WriteLine(
                    "[Whiteboard] ClientBoardStateManager.CheckCountAndCurrentCheckpoint: Current State doesn't match.");
                throw new Exception("CurrentCheckpointState equality condition failed. Server-Client out of sync.");
            }
        }
    }
}