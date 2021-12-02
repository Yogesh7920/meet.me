/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/20/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    internal class ClientBoardStateManagerTesting
    {
        [SetUp]
        public void SetUp()
        {
            _clientBoardStateManager = ClientBoardStateManager.Instance;
            _mockCommunicator = new Mock<IClientBoardCommunicator>();
            _mockCheckpointHandler = new Mock<IClientCheckPointHandler>();
            _mockCommunicator.Setup(m => m.Subscribe(It.IsAny<IServerUpdateListener>()));

            _clientBoardStateManager.Start();
            _clientBoardStateManager.SetCommunicatorAndCheckpointHandler(_mockCommunicator.Object,
                _mockCheckpointHandler.Object);
        }

        private ClientBoardStateManager _clientBoardStateManager;
        private Mock<IClientBoardCommunicator> _mockCommunicator;
        private Mock<IClientCheckPointHandler> _mockCheckpointHandler;


        [Test]
        public void Subscribe_FetchStateRequestSentToCommunicator()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            Mock<IClientBoardStateListener> listener = new();
            BoardServerShape expected = new(null, Operation.FETCH_STATE, _clientBoardStateManager.GetUser());

            // Act
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");

            // Assert
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
        }

        [Test]
        public void GetUser_ReturnsUserId()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");

            // Act and Assert
            Assert.AreEqual("user-id", _clientBoardStateManager.GetUser());
        }

        [Test]
        public void GetUser_UserIdNull_ThrowsNullReferenceException()
        {
            // Act and Assert
            Assert.Throws<NullReferenceException>(() => _clientBoardStateManager.GetUser());
        }

        [Test]
        public void ClearWhiteBoard_UserLevelLow_RequestNotSent()
        {
            // Arrange
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            _clientBoardStateManager.SetUserLevel(BoardConstants.LOW_USER_LEVEL);

            // Act
            _clientBoardStateManager.ClearWhiteBoard();

            // Assert
            _mockCommunicator.Verify(m => m.Send(It.IsAny<BoardServerShape>()), Times.Never);
        }

        [Test]
        public void ClearWhiteBoard_UserLevelHigh_RequestSent()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            _clientBoardStateManager.SetUserLevel(BoardConstants.HIGH_USER_LEVEL);
            BoardServerShape expected = new(null, Operation.CLEAR_STATE, _clientBoardStateManager.GetUser(),
                currentCheckpointState: BoardConstants.INITIAL_CHECKPOINT_STATE);

            // Act
            _clientBoardStateManager.ClearWhiteBoard();

            // Assert
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
        }

        [Test]
        public void SaveCheckpoint_CheckpointHandlerCalled()
        {
            // Arrange
            _mockCheckpointHandler.Setup(m => m.SaveCheckpoint(It.IsAny<string>(), It.IsAny<int>()));

            // Act and Assert
            _clientBoardStateManager.SaveCheckpoint();
            _mockCheckpointHandler.Verify(m => m.SaveCheckpoint(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void SaveOperation_CreateRequest_ReturnsTrue()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            BoardServerShape expected = new(new List<BoardShape> {createShape}, Operation.CREATE, "user-id",
                currentCheckpointState: 0);

            // Act
            var ret = _clientBoardStateManager.SaveOperation(createShape);

            // Assert
            Assert.IsTrue(ret);
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(createShape,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
        }

        [Test]
        public void SaveOperation_CreateWhenIdAlreadyPresent_ReturnsFalse()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _clientBoardStateManager.SaveOperation(createShape);

            var createShapeNew = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            createShapeNew.Uid = createShape.Uid;

            // Act
            var ret = _clientBoardStateManager.SaveOperation(createShapeNew);

            // Assert
            Assert.IsFalse(ret);
            _mockCommunicator.Verify(m => m.Send(It.IsAny<BoardServerShape>()), Times.Once());
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(createShape,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
        }

        [Test]
        [TestCase(Operation.MODIFY)]
        [TestCase(Operation.DELETE)]
        public void SaveOperation_ModifyOrDeleteIdNotFound_ReturnsFalse(Operation operation)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var operationShape = StateManagerHelper.GetCompleteBoardShape(operation);

            // Act
            var ret = _clientBoardStateManager.SaveOperation(operationShape);

            // Assert
            Assert.IsFalse(ret);
            _mockCommunicator.Verify(m => m.Send(It.IsAny<BoardServerShape>()), Times.Never);
        }

        [Test]
        public void SaveOperation_ModifyRequest_ReturnsTrue()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _clientBoardStateManager.SaveOperation(createShape);

            var modifyShape = StateManagerHelper.GetCompleteBoardShape(Operation.MODIFY);
            modifyShape.Uid = createShape.Uid;
            BoardServerShape expected = new(new List<BoardShape> {modifyShape}, Operation.MODIFY, "user-id",
                currentCheckpointState: 0);

            // Act
            var ret = _clientBoardStateManager.SaveOperation(modifyShape);

            // Assert
            Assert.IsTrue(ret);
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(modifyShape,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
        }

        [Test]
        public void SaveOperation_DeleteRequest_ReturnsTrue()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _clientBoardStateManager.SaveOperation(createShape);

            var deleteShape = StateManagerHelper.GetCompleteBoardShape(Operation.DELETE);
            deleteShape.Uid = createShape.Uid;
            BoardServerShape expected = new(new List<BoardShape> {deleteShape}, Operation.DELETE, "user-id",
                currentCheckpointState: 0);

            // Act
            var ret = _clientBoardStateManager.SaveOperation(deleteShape);

            // Assert
            Assert.IsTrue(ret);
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(createShape.Uid));
        }

        [Test]
        [TestCase(Operation.CLEAR_STATE)]
        [TestCase(Operation.FETCH_STATE)]
        [TestCase(Operation.FETCH_CHECKPOINT)]
        [TestCase(Operation.CREATE_CHECKPOINT)]
        [TestCase(Operation.NONE)]
        public void SaveOperation_InvalidOperationFlag_ReturnsFalse(Operation operation)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var shape = StateManagerHelper.GetCompleteBoardShape(operation);

            // Act
            var ret = _clientBoardStateManager.SaveOperation(shape);

            // Assert
            Assert.IsFalse(ret);
        }

        [Test]
        public void GetBoardShape_NotPresent_ReturnsNull()
        {
            // Arrange
            var shapeId = "abc123";

            // Act and assert
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(shapeId));
        }

        [Test]
        public void GetBoardShape_ShapePresent_ReturnsShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _clientBoardStateManager.SaveOperation(createShape);

            // Act and assert
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(createShape,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
        }

        [Test]
        public void FetchCheckpoint_RequestSent()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            _mockCheckpointHandler.Setup(m => m.FetchCheckpoint(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()));

            // Act and assert
            _clientBoardStateManager.FetchCheckpoint(1);
            _mockCheckpointHandler.Verify(m => m.FetchCheckpoint(
                It.Is<int>(num => num == 1),
                It.Is<string>(str => str == "user-id"),
                It.Is<int>(num => num == 0)
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_FetchStateDifferentUser_DoNothing()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            BoardServerShape update = new(StateManagerHelper.GenerateSortedRandomBoardShapes(3, Operation.CREATE),
                Operation.FETCH_STATE, "user-2", 1, 1);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(update.ShapeUpdates[0].Uid));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(update.ShapeUpdates[1].Uid));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(update.ShapeUpdates[2].Uid));
        }

        [Test]
        public void OnMessageReceived_FetchStateSameUser_StateFetched()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            BoardServerShape update = new(boardShapes, Operation.FETCH_STATE, "user-id", 2, 1);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act 
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            for (var i = 0; i < boardShapes.Count; i++)
            {
                // asserting on state
                var shapeStored = _clientBoardStateManager.GetBoardShape(boardShapes[i].Uid);
                Assert.IsNotNull(shapeStored);
                Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes[i], shapeStored));
            }

            // asserting on UX update
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => StateManagerHelper.CompareUXShapeOrder(obj, boardShapes) &&
                                                  obj[0].OperationType == Operation.FETCH_STATE &&
                                                  obj[0].CheckpointNumber == 2)
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_FetchCheckpoint_CheckpointFetched()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            BoardServerShape update = new(boardShapes, Operation.FETCH_CHECKPOINT, "user-id", 2, 1);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act 
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            for (var i = 0; i < boardShapes.Count; i++)
            {
                // asserting on state
                var shapeStored = _clientBoardStateManager.GetBoardShape(boardShapes[i].Uid);
                Assert.IsNotNull(shapeStored);
                Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes[i], shapeStored));
            }

            // asserting on UX update
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => StateManagerHelper.CompareUXShapeOrder(obj, boardShapes) &&
                                                  obj[0].OperationType == Operation.FETCH_CHECKPOINT)
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_FetchCheckpointDuplicateShapesInState_NullSentToUX()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(2, Operation.CREATE);
            boardShapes[0].Uid = boardShapes[1].Uid;
            BoardServerShape update = new(boardShapes, Operation.FETCH_CHECKPOINT, "user-id", 2, 1);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act 
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(update.ShapeUpdates[0].Uid));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(update.ShapeUpdates[1].Uid));
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => obj == null)
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_CreateCheckpoint_UxNotified()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            BoardServerShape update = new(null, Operation.CREATE_CHECKPOINT, "user-id", 2);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act 
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            // asserting on UX update
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => obj.Count == 1 && obj[0].CheckpointNumber == 2 &&
                                                  obj[0].OperationType == Operation.CREATE_CHECKPOINT)
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_CreateCheckpointCurrentStateNotEqual_DoNothing()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            BoardServerShape update = new(null, Operation.CREATE_CHECKPOINT, "user-id", 2, 1);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        [TestCase(Operation.CREATE)]
        [TestCase(Operation.MODIFY)]
        [TestCase(Operation.DELETE)]
        public void OnMessageReceived_UpdateSameUser_DoNothing(Operation operation)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            BoardServerShape update = new(StateManagerHelper.GetListCompleteBoardShapes(1, operation), operation,
                "user-1", 2);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        [TestCase(Operation.CREATE)]
        [TestCase(Operation.MODIFY)]
        [TestCase(Operation.DELETE)]
        public void OnMessageReceived_UpdateMoreThanOneUpdate_DoNothing(Operation operation)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            BoardServerShape update = new(StateManagerHelper.GetListCompleteBoardShapes(2, operation), operation,
                "user-2", 2);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        [TestCase(Operation.CREATE)]
        [TestCase(Operation.MODIFY)]
        [TestCase(Operation.DELETE)]
        public void OnMessageReceived_UpdateCurrentCheckpointStateNotMatches_DoNothing(Operation operation)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            BoardServerShape update = new(StateManagerHelper.GetListCompleteBoardShapes(1, operation), operation,
                "user-2", 2, 1);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        public void OnMessageReceived_CreateRequest_UXNotified()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++) _clientBoardStateManager.SaveOperation(prevState[i]);

            // server update
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(1, Operation.CREATE);
            BoardServerShape update = new(boardShapes, Operation.CREATE, "user-2", 2);

            // finding epected order of outcomes
            var expected = GetExpectedOrder(prevState, boardShapes);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            // asserting on state
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes[0],
                _clientBoardStateManager.GetBoardShape(boardShapes[0].Uid)));
            for (var i = 0; i < prevState.Count; i++)
                Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
            // asserting on UX update
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => StateManagerHelper.CompareUXShapeOrder(obj, expected))
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_CreateRequestShapeAlreadyExist_DoNothing()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++) _clientBoardStateManager.SaveOperation(prevState[i]);

            // server update
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(1, Operation.CREATE);
            boardShapes[0].Uid = prevState[0].Uid;
            BoardServerShape update = new(boardShapes, Operation.CREATE, "user-2", 2);

            // finding epected order of outcomes
            var expected = GetExpectedOrder(prevState, boardShapes);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            // asserting on state
            for (var i = 0; i < prevState.Count; i++)
                Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        [TestCase(5, 1)]
        [TestCase(10, 4)]
        public void OnMessageReceived_ModifyRequest_UXNotified(int noOfShapesAlreadyPresent, int timeToAdd)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(noOfShapesAlreadyPresent, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++) _clientBoardStateManager.SaveOperation(prevState[i]);

            // server update
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(1, Operation.MODIFY);
            boardShapes[0].Uid = prevState[0].Uid;
            boardShapes[0].LastModifiedTime.AddMinutes(timeToAdd);
            BoardServerShape update = new(boardShapes, Operation.MODIFY, "user-2", 2);

            // finding epected order of outcomes
            var expected = GetExpectedOrder(prevState, boardShapes, Operation.MODIFY);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            // asserting on state
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes[0],
                _clientBoardStateManager.GetBoardShape(boardShapes[0].Uid)));
            for (var i = 0; i < prevState.Count; i++)
                Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
            // asserting on UX update
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => StateManagerHelper.CompareUXShapeOrder(obj, expected))
            ), Times.Once());
        }

        [Test]
        public void OnMessageReceived_DeleteRequest_UXNotified()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(10, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++) _clientBoardStateManager.SaveOperation(prevState[i]);

            // server update
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(1, Operation.DELETE);
            boardShapes[0].Uid = prevState[0].Uid;
            BoardServerShape update = new(boardShapes, Operation.DELETE, "user-2", 2);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(boardShapes[0].Uid));
            for (var i = 1; i < prevState.Count; i++)
                Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj =>
                    obj.Count == 1 && obj[0].OperationType == Operation.DELETE && obj[0].ShapeId == boardShapes[0].Uid)
            ), Times.Once());
        }

        [Test]
        [TestCase(Operation.MODIFY)]
        [TestCase(Operation.DELETE)]
        public void OnMessageReceived_ModifyOrDeleteShapeNotFound_DoNothing(Operation operation)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // server update
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(1, operation);
            BoardServerShape update = new(boardShapes, operation, "user-2", 2);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(boardShapes[0].Uid));
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        [TestCase(Operation.CREATE, Operation.MODIFY)]
        [TestCase(Operation.CREATE, Operation.DELETE)]
        [TestCase(Operation.MODIFY, Operation.CREATE)]
        [TestCase(Operation.MODIFY, Operation.DELETE)]
        [TestCase(Operation.DELETE, Operation.CREATE)]
        [TestCase(Operation.DELETE, Operation.MODIFY)]
        public void OnMessageReceived_OperationMismatch_DoNothing(Operation boardShapeOp, Operation serverShapeOp)
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // server update
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(1, boardShapeOp);
            BoardServerShape update = new(boardShapes, serverShapeOp, "user-2", 2);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(boardShapes[0].Uid));
            listener.Verify(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()), Times.Never);
        }

        [Test]
        public void OnMessageReceived_ClearStateRequest_UXNotified()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(10, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++) _clientBoardStateManager.SaveOperation(prevState[i]);

            // server update
            BoardServerShape update = new(null, Operation.CLEAR_STATE, "user-1", 2);

            // Act
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            for (var i = 1; i < prevState.Count; i++)
                Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => obj.Count == 1 && obj[0].OperationType == Operation.CLEAR_STATE)
            ), Times.Once());
        }

        [Test]
        public void DoUndo_StackEmpty_ReturnsNull()
        {
            // Act and Assert
            Assert.IsNull(_clientBoardStateManager.DoUndo());
        }

        [Test]
        public void DoRedo_StackEmpty_ReturnsNull()
        {
            // Act and Assert
            Assert.IsNull(_clientBoardStateManager.DoRedo());
        }

        [Test]
        public void DoUndo_UndoCreateOperation_ReturnsUXShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _clientBoardStateManager.SaveOperation(createShape);
            var deleteShape = createShape.Clone();
            deleteShape.RecentOperation = Operation.DELETE;
            BoardServerShape expected = new(new List<BoardShape> {deleteShape}, Operation.DELETE, "user-1",
                currentCheckpointState: 0);

            _mockCommunicator.Reset();
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var ret = _clientBoardStateManager.DoUndo();

            Assert.IsNotNull(ret);
            Assert.IsTrue(ret.Count == 1 && ret[0].WindowsShape.Uid == createShape.Uid &&
                          ret[0].UxOperation == UXOperation.DELETE && ret[0].OperationType == Operation.DELETE);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(createShape.Uid));
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.AtLeastOnce);
        }

        [Test]
        public void DoUndo_UndoDeleteOperation_ReturnsUXShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            var createShapeCopy = createShape.Clone();
            _clientBoardStateManager.SaveOperation(createShape);
            var deleteShape = StateManagerHelper.GetCompleteBoardShape(Operation.DELETE);
            deleteShape.Uid = createShape.Uid;
            _clientBoardStateManager.SaveOperation(deleteShape);
            BoardServerShape expected = new(new List<BoardShape> {createShapeCopy}, Operation.CREATE, "user-1",
                currentCheckpointState: 0);

            _mockCommunicator.Reset();
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var ret = _clientBoardStateManager.DoUndo();


            Assert.IsNotNull(ret);
            Assert.IsTrue(ret.Count == 1 && ret[0].WindowsShape.Uid == createShape.Uid &&
                          ret[0].UxOperation == UXOperation.CREATE && ret[0].OperationType == Operation.CREATE);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(createShapeCopy,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.AtLeastOnce);
        }

        [Test]
        public void DoUndo_UndoModifyOperation_ReturnsUXShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            var createShapeCopy = createShape.Clone();
            _clientBoardStateManager.SaveOperation(createShape);
            var modifyShape = StateManagerHelper.GetCompleteBoardShape(Operation.MODIFY);
            modifyShape.Uid = createShape.Uid;
            _clientBoardStateManager.SaveOperation(modifyShape);

            var ret = _clientBoardStateManager.DoUndo();
            Assert.IsNotNull(ret);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(createShapeCopy,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
            Assert.IsTrue(ret.Count == 2 && ret[0].WindowsShape.Uid == ret[1].WindowsShape.Uid &&
                          ret[0].UxOperation == UXOperation.DELETE && ret[1].UxOperation == UXOperation.CREATE
                          && ret[0].WindowsShape.Uid == createShape.Uid);
        }

        [Test]
        public void DoRedo_RedoCreateOperation_ReturnsUXShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            var createShapeCopy = createShape.Clone();
            _clientBoardStateManager.SaveOperation(createShape);
            _clientBoardStateManager.DoUndo();
            BoardServerShape expected = new(new List<BoardShape> {createShapeCopy}, Operation.CREATE, "user-1",
                currentCheckpointState: 0);

            _mockCommunicator.Reset();
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var ret = _clientBoardStateManager.DoRedo();

            // Assert
            Assert.IsNotNull(ret);
            Assert.IsTrue(ret.Count == 1 && ret[0].WindowsShape.Uid == createShape.Uid &&
                          ret[0].UxOperation == UXOperation.CREATE && ret[0].OperationType == Operation.CREATE);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(createShapeCopy,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
            Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(createShape.Uid));
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.AtLeastOnce);
        }

        [Test]
        public void DoRedo_RedoDeleteOperation_ReturnsUXShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            var createShapeCopy = createShape.Clone();
            _clientBoardStateManager.SaveOperation(createShape);
            var deleteShape = createShape.Clone();
            deleteShape.RecentOperation = Operation.DELETE;
            deleteShape.Uid = createShape.Uid;
            _clientBoardStateManager.SaveOperation(deleteShape);
            _clientBoardStateManager.DoUndo();
            BoardServerShape expected = new(new List<BoardShape> {deleteShape}, Operation.DELETE, "user-1",
                currentCheckpointState: 0);

            _mockCommunicator.Reset();
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var ret = _clientBoardStateManager.DoRedo();

            // Assert
            Assert.IsNotNull(ret);
            Assert.IsTrue(ret.Count == 1 && ret[0].WindowsShape.Uid == createShape.Uid &&
                          ret[0].UxOperation == UXOperation.DELETE && ret[0].OperationType == Operation.DELETE);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(createShape.Uid));
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.AtLeastOnce);
        }

        [Test]
        public void DoRedo_RedoModifyOperation_ReturnsUXShape()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-1");
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            _clientBoardStateManager.SaveOperation(createShape);
            var modifyShape = StateManagerHelper.GetCompleteBoardShape(Operation.MODIFY);
            modifyShape.Uid = createShape.Uid;
            var expected = modifyShape.Clone();
            expected.RecentOperation = Operation.CREATE;
            _clientBoardStateManager.SaveOperation(modifyShape);
            _clientBoardStateManager.DoUndo();

            var ret = _clientBoardStateManager.DoRedo();

            // Assert
            Assert.IsNotNull(ret);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(expected,
                _clientBoardStateManager.GetBoardShape(createShape.Uid)));
            Assert.IsTrue(ret.Count == 2 && ret[0].WindowsShape.Uid == ret[1].WindowsShape.Uid &&
                          ret[0].UxOperation == UXOperation.DELETE && ret[1].UxOperation == UXOperation.CREATE
                          && ret[0].WindowsShape.Uid == createShape.Uid);
        }

        [Test]
        public void OnMessageReceived_FetchCheckpointWhenShapesInState_UXNotified()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            var boardShapes = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            BoardServerShape update = new(boardShapes, Operation.FETCH_CHECKPOINT, "user-id", 2, 1);
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++) _clientBoardStateManager.SaveOperation(prevState[i]);

            // Act
            _mockCommunicator.Reset();
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            _clientBoardStateManager.OnMessageReceived(update);

            // Assert
            for (var i = 0; i < prevState.Count; i++)
                Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
            for (var i = 0; i < boardShapes.Count; i++)
            {
                // asserting on state
                var shapeStored = _clientBoardStateManager.GetBoardShape(boardShapes[i].Uid);
                Assert.IsNotNull(shapeStored);
                Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes[i], shapeStored));
            }

            // asserting on UX update
            listener.Verify(m => m.OnUpdateFromStateManager(
                It.Is<List<UXShapeHelper>>(obj => StateManagerHelper.CompareUXShapeOrder(obj, boardShapes) &&
                                                  obj[0].OperationType == Operation.FETCH_CHECKPOINT)
            ), Times.Once());
        }

        [Test]
        public void CombinedCases_ClientServerOperations_MultipleAsserts()
        {
            // Arrange
            _clientBoardStateManager.SetUser("user-id");
            Mock<IClientBoardStateListener> listener = new();
            _clientBoardStateManager.Subscribe(listener.Object, "client-UX");
            listener.Setup(m => m.OnUpdateFromStateManager(It.IsAny<List<UXShapeHelper>>()));
            _mockCheckpointHandler.Setup(m => m.CheckpointNumber);
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));

            // creating previous state
            var prevState = StateManagerHelper.GetListCompleteBoardShapes(5, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++)
            {
                prevState[i].Uid = i.ToString();
                _clientBoardStateManager.SaveOperation(prevState[i]);
            }

            // Multiple Acts and asserts

            // server update to delete first shape
            var deleteShape = prevState[0].Clone();
            deleteShape.RecentOperation = Operation.DELETE;
            BoardServerShape boardServerShape = new(new List<BoardShape> {deleteShape}, Operation.DELETE, "user-2");
            _clientBoardStateManager.OnMessageReceived(boardServerShape);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[0].Uid));

            // client undo (fifth shape inserted must be deleted)
            var ret = _clientBoardStateManager.DoUndo();
            Assert.IsNotNull(ret);
            Assert.IsTrue(ret[0].WindowsShape.Uid == prevState[4].Uid && ret[0].UxOperation == UXOperation.DELETE);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[4].Uid));

            // server modify 
            var modifyShape = StateManagerHelper.GetCompleteBoardShape(Operation.MODIFY);
            modifyShape.Uid = prevState[3].Uid;
            modifyShape.LastModifiedTime = prevState[3].LastModifiedTime;
            BoardServerShape modifyUpdate = new(new List<BoardShape> {modifyShape}, Operation.MODIFY, "user-2");
            _clientBoardStateManager.OnMessageReceived(modifyUpdate);
            var ret2 = _clientBoardStateManager.GetBoardShape(prevState[3].Uid);
            Assert.IsNotNull(ret2);
            Assert.IsTrue(ret2.LastModifiedTime == modifyShape.LastModifiedTime &&
                          ret2.ShapeOwnerId == modifyShape.ShapeOwnerId);

            // client undo (fourth shape inserted must be deleted)
            var ret3 = _clientBoardStateManager.DoUndo();
            Assert.IsNotNull(ret3);
            Assert.IsTrue(ret3[0].WindowsShape.Uid == prevState[3].Uid && ret3[0].UxOperation == UXOperation.DELETE);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[3].Uid));

            // server modify again on fourth shape won't work
            _clientBoardStateManager.OnMessageReceived(modifyUpdate);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(modifyShape.Uid));

            // server delete third shape
            var deleteShape2 = prevState[2].Clone();
            deleteShape2.RecentOperation = Operation.DELETE;
            BoardServerShape deleteUpdate = new(new List<BoardShape> {deleteShape2}, Operation.DELETE, "user-2");
            _clientBoardStateManager.OnMessageReceived(deleteUpdate);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[0].Uid));

            // undo will not work for third shape (already deleted) [second one will be deleted]
            var ret4 = _clientBoardStateManager.DoUndo();
            Assert.IsNotNull(ret4);
            Assert.IsTrue(ret4[0].WindowsShape.Uid == prevState[1].Uid && ret4[0].UxOperation == UXOperation.DELETE);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(prevState[1].Uid));

            // redo (second one will be recreated)
            var ret5 = _clientBoardStateManager.DoRedo();
            Assert.IsNotNull(ret5);
            Assert.IsTrue(ret5[0].WindowsShape.Uid == prevState[1].Uid && ret5[0].UxOperation == UXOperation.CREATE);
            Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(prevState[1].Uid));

            // Client creates a shape
            var createShape = StateManagerHelper.GetCompleteBoardShape(Operation.CREATE);
            Assert.IsTrue(_clientBoardStateManager.SaveOperation(createShape));
            Assert.NotNull(_clientBoardStateManager.GetBoardShape(createShape.Uid));

            // Client deletes shape
            var deleteShape3 = prevState[1].Clone();
            deleteShape3.RecentOperation = Operation.DELETE;
            Assert.IsTrue(_clientBoardStateManager.SaveOperation(deleteShape3));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(deleteShape3.Uid));

            // Client modifies shape
            var modifyShape2 = StateManagerHelper.GetCompleteBoardShape(Operation.MODIFY);
            modifyShape2.Uid = createShape.Uid;
            modifyShape2.LastModifiedTime = createShape.LastModifiedTime.AddMinutes(2);
            Assert.IsTrue(_clientBoardStateManager.SaveOperation(modifyShape2));
            Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(modifyShape2.Uid));

            // Client modifies deleted shape
            var modifyShape3 = StateManagerHelper.GetCompleteBoardShape(Operation.MODIFY);
            modifyShape3.Uid = deleteShape3.Uid;
            modifyShape3.LastModifiedTime = createShape.LastModifiedTime.AddMinutes(2);
            Assert.IsFalse(_clientBoardStateManager.SaveOperation(modifyShape3));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(modifyShape3.Uid));

            // Client deletes a deleted shape
            Assert.IsFalse(_clientBoardStateManager.SaveOperation(deleteShape3));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(deleteShape3.Uid));

            // Server update delete a deleted shape
            _clientBoardStateManager.OnMessageReceived(new BoardServerShape(new List<BoardShape> {deleteShape3},
                Operation.DELETE, "user-2"));
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(deleteShape3.Uid));

            // Server update deletes modifyShape2
            modifyShape2.RecentOperation = Operation.DELETE;
            BoardServerShape update = new(new List<BoardShape> {modifyShape2}, Operation.DELETE, "user-2");
            _clientBoardStateManager.OnMessageReceived(update);
            Assert.IsNull(_clientBoardStateManager.GetBoardShape(modifyShape2.Uid));

            // Client tries to undo but this won't happen
            var ret6 = _clientBoardStateManager.DoUndo();
            Assert.IsNotNull(ret6);
            Assert.IsTrue(UXOperation.CREATE == ret6[0].UxOperation && prevState[1].Uid == ret6[0].WindowsShape.Uid &&
                          ret.Count == 1);

            // Fetch Checkpoint server update
            BoardServerShape fetchCheckpoint = new(prevState, Operation.FETCH_CHECKPOINT, "user-4", 2, 1);
            _clientBoardStateManager.OnMessageReceived(fetchCheckpoint);
            for (var i = 0; i < prevState.Count; i++)
                Assert.IsNotNull(_clientBoardStateManager.GetBoardShape(prevState[i].Uid));
        }

        private static List<BoardShape> GetExpectedOrder(List<BoardShape> prevState, List<BoardShape> boardShapes,
            Operation operation = Operation.CREATE, int indexOfModify = 0)
        {
            List<BoardShape> expected = new();
            for (var i = 0; i < prevState.Count; i++)
                if (prevState[i].LastModifiedTime >= boardShapes[0].LastModifiedTime)
                    expected.Add(prevState[i]);
            if (operation == Operation.MODIFY) expected.Add(prevState[indexOfModify]);
            expected.Add(boardShapes[0]);
            for (var i = 0; i < prevState.Count; i++)
                if (prevState[i].LastModifiedTime >= boardShapes[0].LastModifiedTime)
                    expected.Add(prevState[i]);
            return expected;
        }
    }
}