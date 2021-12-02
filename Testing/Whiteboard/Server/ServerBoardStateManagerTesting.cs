/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/17/2021
 * Date Modified: 11/28/2021
**/


using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    internal class ServerBoardStateManagerTesting
    {
        [SetUp]
        public void SetUp()
        {
            _serverBoardStateManager = new ServerBoardStateManager();
            _mockCheckpointHandler = new Mock<IServerCheckPointHandler>();
            _serverBoardStateManager.SetCheckpointHandler(_mockCheckpointHandler.Object);
        }

        private ServerBoardStateManager _serverBoardStateManager;
        private Mock<IServerCheckPointHandler> _mockCheckpointHandler;

        [Test]
        [TestCase(1, "user-id", 5)]
        [TestCase(10, "xyz", 10)]
        public void FetchCheckpoint_CheckpointPresent_ReturnsBoardServerShapeReply(int checkpoint, string userId,
            int numInsertions)
        {
            // create current state 
            var prevState = StateManagerHelper.GenerateSortedRandomBoardShapes(5, Operation.CREATE);
            for (var i = 0; i < prevState.Count; i++)
                _serverBoardStateManager.SaveUpdate(new BoardServerShape(new List<BoardShape> {prevState[i]},
                    Operation.CREATE, userId));

            // generate random shapes which are present in checkpoint
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(numInsertions);

            // Say one shape was common in checkpoint and current state
            boardShapes[0].Uid = prevState[0].Uid;

            // make checkpoint handler return desired values
            _mockCheckpointHandler.Setup(m => m.FetchCheckpoint(It.IsAny<int>())).Returns(boardShapes);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(checkpoint);

            // Act and assert
            var reply = _serverBoardStateManager.FetchCheckpoint(checkpoint, userId);
            Assert.IsTrue(StateManagerHelper.CompareBoardServerShapes(reply,
                new BoardServerShape(boardShapes, Operation.FETCH_CHECKPOINT, userId, checkpoint, checkpoint)));
            var state = _serverBoardStateManager.FetchState(userId);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes, state.ShapeUpdates) &&
                          checkpoint == state.CheckpointNumber);
        }

        [Test]
        [TestCase(1, "user-id")]
        public void FetchCheckpoint_CheckpointNotPresent_ReturnsNull(int checkpoint, string userId)
        {
            // Arrange
            List<BoardShape> boardShapes = null;
            _mockCheckpointHandler.Setup(m => m.FetchCheckpoint(It.IsAny<int>())).Returns(boardShapes);

            // Act and assert
            var reply = _serverBoardStateManager.FetchCheckpoint(checkpoint, userId);
            Assert.IsNull(reply);
        }

        [Test]
        [TestCase("user-id")]
        public void FetchState_EmptyState_ReturnsBoardServerShape(string userId)
        {
            // Arrange
            Random random = new();
            var checkpoint = random.Next(0, 100);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(checkpoint);

            // Act and assert
            var reply = _serverBoardStateManager.FetchState(userId);
            Assert.IsTrue(StateManagerHelper.CompareBoardServerShapes(
                new BoardServerShape(new List<BoardShape>(), Operation.FETCH_STATE, userId, checkpoint), reply));
        }

        [Test]
        [TestCase(2)]
        public void GetCheckpointsNumber_ReturnsCheckpointNumber(int checkpoint)
        {
            // Arrange
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(checkpoint);

            // Act and Assert
            Assert.AreEqual(checkpoint, _serverBoardStateManager.GetCheckpointsNumber());
        }

        [Test]
        [TestCase(2)]
        public void SaveCheckpoint_ReturnsBoardServerShape(int checkpoint)
        {
            // Arrange
            var userId = "user-id";
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(checkpoint);
            _mockCheckpointHandler.Setup(m => m.SaveCheckpoint(It.IsAny<List<BoardShape>>(), userId))
                .Returns(checkpoint + 1);

            // Act and Assert
            var reply = _serverBoardStateManager.SaveCheckpoint(userId);
            Assert.IsTrue(StateManagerHelper.CompareBoardServerShapes(
                new BoardServerShape(null, Operation.CREATE_CHECKPOINT, userId, checkpoint + 1), reply));
        }

        [Test]
        [TestCase(0, 2)]
        [TestCase(0, 0)]
        [TestCase(2, 0)]
        public void SaveCheckpoint_IncrementConditionFails_ReturnsNull(int prevCheckpoint, int newCheckpoint)
        {
            // Arrange
            var userId = "user-id";
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(prevCheckpoint);
            _mockCheckpointHandler.Setup(m => m.SaveCheckpoint(It.IsAny<List<BoardShape>>(), userId))
                .Returns(newCheckpoint);

            // Act and Assert
            var reply = _serverBoardStateManager.SaveCheckpoint(userId);
            Assert.IsNull(reply);
        }

        [Test]
        public void SaveUpdate_MoreThanOneUpdate_ReturnsFalse()
        {
            // Arrange
            BoardServerShape boardServerShape = new(StateManagerHelper.GenerateSortedRandomBoardShapes(2),
                Operation.CREATE, "user-id");
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(1);

            // Act and Assert
            var reply = _serverBoardStateManager.SaveUpdate(boardServerShape);
            Assert.IsFalse(reply);

            // Check state too
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.AreEqual(0, state.ShapeUpdates.Count);
            Assert.AreEqual(1, state.CheckpointNumber);
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(0, 1)]
        [TestCase(0, 2)]
        public void SaveUpdate_CurrentCheckpointStateConditionFails_ReturnsFalse(int checkpoint,
            int currCheckpointState)
        {
            // Arrange
            var userId = "user-id";
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(10);
            _mockCheckpointHandler.Setup(m => m.FetchCheckpoint(It.IsAny<int>())).Returns(boardShapes);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(checkpoint);
            // changing current checkpoint value via fetch state
            _serverBoardStateManager.FetchCheckpoint(checkpoint, userId);
            // creating a request
            BoardServerShape boardServerShape = new(StateManagerHelper.GenerateSortedRandomBoardShapes(1),
                Operation.CREATE, userId, currentCheckpointState: currCheckpointState);

            // Act and assert
            var reply = _serverBoardStateManager.SaveUpdate(boardServerShape);
            Assert.IsFalse(reply);

            // Check state
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(boardShapes, state.ShapeUpdates));
            Assert.AreEqual(checkpoint, state.CheckpointNumber);
        }

        [Test]
        [TestCase(Operation.CREATE, Operation.MODIFY)]
        [TestCase(Operation.CREATE, Operation.DELETE)]
        [TestCase(Operation.CREATE_CHECKPOINT, Operation.CREATE)]
        [TestCase(Operation.MODIFY, Operation.CREATE)]
        [TestCase(Operation.MODIFY, Operation.DELETE)]
        [TestCase(Operation.FETCH_CHECKPOINT, Operation.MODIFY)]
        [TestCase(Operation.DELETE, Operation.CREATE)]
        [TestCase(Operation.DELETE, Operation.MODIFY)]
        [TestCase(Operation.FETCH_STATE, Operation.DELETE)]
        public void SaveUpdate_OperationMismatch_ReturnsFalse(Operation boardShapeOperation,
            Operation boardServerShapeOperation)
        {
            // Arrange
            BoardServerShape update = new(StateManagerHelper.GenerateSortedRandomBoardShapes(1, boardShapeOperation),
                boardServerShapeOperation, "user-id");

            // Act and Assert
            var reply = _serverBoardStateManager.SaveUpdate(update);
            Assert.IsFalse(reply);
        }

        [Test]
        [TestCase(Operation.FETCH_STATE)]
        [TestCase(Operation.FETCH_CHECKPOINT)]
        [TestCase(Operation.CREATE_CHECKPOINT)]
        [TestCase(Operation.NONE)]
        public void SaveUpdate_InvalidOperation_ReturnsFalse(Operation boardServerShapeOperation)
        {
            // Arrange
            BoardServerShape update = new(StateManagerHelper.GenerateSortedRandomBoardShapes(1),
                boardServerShapeOperation, "user-id");

            // Act and Assert
            var reply = _serverBoardStateManager.SaveUpdate(update);
            Assert.IsFalse(reply);
        }

        [Test]
        public void SaveUpdate_CreateSaved_ReturnsTrue()
        {
            // Arrange
            var userId = "user-id";
            BoardServerShape update = new(StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.CREATE),
                Operation.CREATE, userId);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);

            // Act and Assert
            var reply = _serverBoardStateManager.SaveUpdate(update);
            Assert.IsTrue(reply);

            // Check State
            var state = _serverBoardStateManager.FetchState(userId);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(update.ShapeUpdates, state.ShapeUpdates));
        }

        [Test]
        public void SaveUpdate_CreateShapeIdAlreadyExists_ReturnsFalse()
        {
            // Arrange
            // Create and insert shape in state
            var userId = "user-id";
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.CREATE);
            BoardServerShape update = new(boardShapes, Operation.CREATE, userId);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);
            _serverBoardStateManager.SaveUpdate(update);

            // Act and assert
            var reply = _serverBoardStateManager.SaveUpdate(update);
            Assert.IsFalse(reply);

            // check state
            var state = _serverBoardStateManager.FetchState(userId);
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(update.ShapeUpdates, state.ShapeUpdates));
        }

        [Test]
        [TestCase(Operation.MODIFY)]
        [TestCase(Operation.DELETE)]
        public void SaveUpdate_ModifyOrDeleteShapeNotFound_ReturnsFalse(Operation operation)
        {
            // Arrange
            BoardServerShape update = new(StateManagerHelper.GenerateSortedRandomBoardShapes(1, operation), operation,
                "user-id");
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);

            // Act and assert
            var reply = _serverBoardStateManager.SaveUpdate(update);
            Assert.IsFalse(reply);

            // Check state
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.AreEqual(0, state.ShapeUpdates.Count);
        }

        [Test]
        public void SaveUpdate_ShapeModified_ReturnsTrue()
        {
            // Arrange
            // Create a shape and insert it in state
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.CREATE);
            BoardServerShape createShape = new(boardShapes, Operation.CREATE, "user-id");
            _serverBoardStateManager.SaveUpdate(createShape);

            // Create modify request
            var modifiedBoardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1);
            modifiedBoardShapes[0].Uid = boardShapes[0].Uid;
            BoardServerShape modifyShape =
                new(modifiedBoardShapes, Operation.MODIFY, modifiedBoardShapes[0].ShapeOwnerId);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);

            // Act and assert
            var reply = _serverBoardStateManager.SaveUpdate(modifyShape);
            Assert.IsTrue(reply);

            // Check state
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.IsTrue(StateManagerHelper.CompareBoardShapes(modifiedBoardShapes, state.ShapeUpdates));
        }

        [Test]
        public void SaveUpdate_ShapeDeleted_ReturnsTrue()
        {
            // Arrange
            // Create a shape and insert it in state
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.CREATE);
            BoardServerShape createShape = new(boardShapes, Operation.CREATE, "user-id");
            _serverBoardStateManager.SaveUpdate(createShape);

            // Create delete request fot the shape
            var modifiedBoardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.DELETE);
            modifiedBoardShapes[0].Uid = boardShapes[0].Uid;
            BoardServerShape modifyShape =
                new(modifiedBoardShapes, Operation.DELETE, modifiedBoardShapes[0].ShapeOwnerId);

            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);

            // Act and Assert
            var reply = _serverBoardStateManager.SaveUpdate(modifyShape);
            Assert.IsTrue(reply);

            // Check state
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.AreEqual(0, state.ShapeUpdates.Count);
        }

        [Test]
        public void SaveUpdate_ClearState_ReturnsTrue()
        {
            // Arrange
            BoardServerShape clearState = new(null, Operation.CLEAR_STATE, "user-id", currentCheckpointState: 0);
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);

            // Act and Assert
            var reply = _serverBoardStateManager.SaveUpdate(clearState);
            Assert.IsTrue(reply);

            // Check state
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.AreEqual(0, state.ShapeUpdates.Count);
        }

        [Test]
        [TestCase(Operation.DELETE)]
        [TestCase(Operation.MODIFY)]
        public void CombinationCheck_DeleteDeleteOrModify_MultipleChecks(Operation operation)
        {
            // Arrange
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);
            // create
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.CREATE);
            BoardServerShape createShape = new(boardShapes, Operation.CREATE, "user-id");
            Assert.IsTrue(_serverBoardStateManager.SaveUpdate(createShape));

            // delete
            var modifiedBoardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.DELETE);
            modifiedBoardShapes[0].Uid = boardShapes[0].Uid;
            BoardServerShape modifyShape =
                new(modifiedBoardShapes, Operation.DELETE, modifiedBoardShapes[0].ShapeOwnerId);
            Assert.IsTrue(_serverBoardStateManager.SaveUpdate(modifyShape));

            // operation (modify/delete)
            var update = StateManagerHelper.GenerateSortedRandomBoardShapes(1, operation);
            update[0].Uid = boardShapes[0].Uid;

            // Act
            var reply = _serverBoardStateManager.SaveUpdate(new BoardServerShape(update, operation, "user-id"));

            // Assert
            Assert.IsFalse(reply);
            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.AreEqual(0, state.ShapeUpdates.Count);
        }

        [Test]
        public void CombinationCheck_CreateDeleteCreateDelete_MultipleChecks()
        {
            // Arrange
            _mockCheckpointHandler.Setup(m => m.GetCheckpointsNumber()).Returns(0);
            // create
            var boardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.CREATE);
            BoardServerShape createShape = new(boardShapes, Operation.CREATE, "user-id");
            Assert.IsTrue(_serverBoardStateManager.SaveUpdate(createShape));

            // delete
            var modifiedBoardShapes = StateManagerHelper.GenerateSortedRandomBoardShapes(1, Operation.DELETE);
            modifiedBoardShapes[0].Uid = boardShapes[0].Uid;
            BoardServerShape modifyShape =
                new(modifiedBoardShapes, Operation.DELETE, modifiedBoardShapes[0].ShapeOwnerId);
            Assert.IsTrue(_serverBoardStateManager.SaveUpdate(modifyShape));

            // create same back
            BoardServerShape createShapeAgain = new(boardShapes, Operation.CREATE, "user-id");
            Assert.IsTrue(_serverBoardStateManager.SaveUpdate(createShapeAgain));

            // delete again
            BoardServerShape modifyShapeAgain =
                new(modifiedBoardShapes, Operation.DELETE, modifiedBoardShapes[0].ShapeOwnerId);
            Assert.IsTrue(_serverBoardStateManager.SaveUpdate(modifyShapeAgain));

            var state = _serverBoardStateManager.FetchState("user-id");
            Assert.AreEqual(0, state.ShapeUpdates.Count);
        }
    }
}