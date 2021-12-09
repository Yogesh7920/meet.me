/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    internal class ActiveBoardOperationsHandlerTests
    {
        [SetUp]
        public void SetUp()
        {
            _handler = new ActiveBoardOperationsHandler();
            _mockStateManager = new Mock<IClientBoardStateManagerInternal>();
            _handler.SetStateManager(_mockStateManager.Object);
        }

        private ActiveBoardOperationsHandler _handler;
        private Mock<IClientBoardStateManagerInternal> _mockStateManager;

        [Test]
        public void CreateShape_NewObjectCreationFirstCallWithNoServerStateUpdate_ReturnsList()
        {
            // mock the state manager to return userid so that handler does not throw an exception
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(1, 1);
            Coordinate end = new(3, 3);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            // Though Iit will already be null, this is avoid failure if previous test affected the context of the class.
            _handler.SetLastDrawn(null);

            // Creation from stratch
            var operations = _handler.CreateShape(ShapeType.Ellipse, start, end, strokeWidth, strokecolor);
            _mockStateManager.Verify(m => m.GetUser(), Times.Once());
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 1);

            // Verifying the UXShape received
            Comparators.CheckUXShape(operations[0], UXOperation.Create, ShapeType.Ellipse, new Coordinate(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(2, operations[0].WindowsShape.Width);
            Assert.AreEqual(2, operations[0].WindowsShape.Height);

            // checking the _lastDrawn Object has correct details
            var lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(operations[0].WindowsShape.Uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.Create, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.Ellipse, lastDrawn.MainShapeDefiner.ShapeIdentifier);
        }

        [Test]
        public void CreateShape_NewObjectCreationSuccessorCallsNoServerStateUpdate_ReturnsList()
        {
            // mock the state manager to return userid so that handler does not throw an exception
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");

            // creation of _lastDrawnShape for real time rendering
            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Create, Operation.Create);

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(3, 3);
            Coordinate end = new(5, 5);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            var operations = _handler.CreateShape(ShapeType.Rectangle, start, end, strokeWidth, strokecolor, uid);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Comparators.CheckUXShape(operations[0], UXOperation.Delete, ShapeType.Rectangle, new Coordinate(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);


            // The UXObject at list position 2 should be the one to be created.
            Comparators.CheckUXShape(operations[1], UXOperation.Create, ShapeType.Rectangle, new Coordinate(3, 3), 0);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);
            Assert.AreEqual(4, operations[1].WindowsShape.Width);
            Assert.AreEqual(4, operations[1].WindowsShape.Height);
            Assert.IsTrue(operations[1].TranslationCoordinate.Equals(new Coordinate(3, 3)));


            // checking the _lastDrawn Object has correct details
            var lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.Create, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.Rectangle, lastDrawn.MainShapeDefiner.ShapeIdentifier);
        }

        [Test]
        public void CreateShape_NewShapeCreationSuccessorCallsSendServerStateUpdate_ReturnsList()
        {
            // mock 2 functionalities of the state manager to return values as needed
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            // creation of _lastDrawnShape for real time rendering
            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Create, Operation.Create);

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(3, 3);
            Coordinate end = new(5, 5);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            // create shape succedding the first operation i.e last drawn shape exists in the context of handler
            var operations = _handler.CreateShape(ShapeType.Rectangle, start, end, strokeWidth, strokecolor, uid, true);

            // checking the _lastDrawn Object has correct details
            var lastDrawn = _handler.GetLastDrawn();
            Assert.IsNull(lastDrawn);

            // verify the boardshape being sent to the server
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => obj.Uid == uid && obj.MainShapeDefiner.Height == 4 &&
                                         obj.MainShapeDefiner.Width == 4 && obj.MainShapeDefiner.Center.Equals(start))
            ), Times.Once());
        }

        [Test]
        public void CreateShape_InvalidRequestToRealTimeCreation_ReturnsNull()
        {
            // set last drawn in context of handler
            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Create, Operation.Create);

            // user gives invalid id to the shape id to continue the real time create operation in progress.
            var operations = _handler.CreateShape(ShapeType.Rectangle, new Coordinate(3, 3), new Coordinate(5, 5), 2,
                new BoardColor(2, 3, 4), "12", true);
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateShape_ServerReturnFalseWhileUpdating_ReturnsNull()
        {
            // mimic the state manager sending a false
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);

            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Create, Operation.Create);
            var operations = _handler.CreateShape(ShapeType.Rectangle, new Coordinate(3, 3), new Coordinate(5, 5), 2,
                new BoardColor(2, 3, 4), uid, true);
            Assert.IsNull(operations);
        }

        //Testing the Modify shape realtime operation
        [Test]
        public void ModifyShapeRealTime_LocalModificationToShapeWithNoServerUpdate_ReturnList()
        {
            // Setting the return value of stateManager when GetShape with id is called for the shape to be modified.
            var uid = "123";
            var angleOfRotation = (float) (Math.PI / 2);
            // Testing with a line and translate function
            MainShape mainShape = new Line(angleOfRotation, 2, new Coordinate(1, 1), new Coordinate(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.Create);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            _handler.SetLastDrawn(null);

            // Perform modication
            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Translate, new Coordinate(2, 2),
                new Coordinate(3, 3), "123", DragPos.None);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Comparators.CheckUXShape(operations[0], UXOperation.Delete, ShapeType.Line, new Coordinate(0, 0),
                angleOfRotation);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);


            // The UXObject at list position 2 should be the one to be created.
            Comparators.CheckUXShape(operations[1], UXOperation.Create, ShapeType.Line, new Coordinate(0, 0),
                angleOfRotation);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            var operationLine = (System.Windows.Shapes.Line) operations[1].WindowsShape;
            Assert.AreEqual(2, operationLine.X1);
            Assert.AreEqual(3, operationLine.Y1);
            Assert.AreEqual(4, operationLine.X2);
            Assert.AreEqual(3, operationLine.Y2);

            // checking the _lastDrawn Object has correct details
            var lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.Modify, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.Line, lastDrawn.MainShapeDefiner.ShapeIdentifier);
        }

        [Test]
        public void ModifyShapeRealTime_LocalSuccessorModificationNoServerUpdate_ReturnsList()
        {
            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Rotate, Operation.Modify);

            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Rotate, new Coordinate(3, 3),
                new Coordinate(2, 3), uid, DragPos.None);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Comparators.CheckUXShape(operations[0], UXOperation.Delete, ShapeType.Rectangle, new Coordinate(2, 2), 0);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // The UXObject at list position 2 should be the one to be created.
            Comparators.CheckUXShape(operations[1], UXOperation.Create, ShapeType.Rectangle, new Coordinate(2, 2),
                (float) -Math.PI / 4);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            // checking the _lastDrawn Object has correct details
            var lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.Modify, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.Rectangle, lastDrawn.MainShapeDefiner.ShapeIdentifier);
        }

        [Test]
        public void ModifyShapeRealTime_LocalModificationWithServerUpdate_ReturnList()
        {
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            // Calling resize operation on the uid
            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Resize, Operation.Modify);

            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Resize, new Coordinate(3, 3),
                new Coordinate(4, 4), uid, DragPos.TopRight, true);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            // checking the _lastDrawn Object has correct details
            var lastDrawn = _handler.GetLastDrawn();
            Assert.IsNull(lastDrawn);

            // check the object being sent to the server.
            Coordinate cen = new(2, 2);
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj =>
                    obj.Uid == uid && Comparators.AreDecimalEqual(obj.MainShapeDefiner.Height, 4) &&
                    Comparators.AreDecimalEqual(obj.MainShapeDefiner.Height, 4) &&
                    obj.MainShapeDefiner.Center.Equals(cen))
            ), Times.Once());
        }

        [Test]
        public void ModifyShapeRealTime_TryToPerformCreationThroughModification_ReturnNull()
        {
            // Setting the return value of stateManager when GetShape with id is called for the shape to be modified.
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            _handler.SetLastDrawn(null);

            // Perform modication
            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Create, new Coordinate(2, 2),
                new Coordinate(3, 3), uid, DragPos.None, true);
            Assert.IsNull(operations);
        }

        [Test]
        public void ModifyShapeRealTime_ModificationToShapeServerUpdateFails_ReturnUndoList()
        {
            // Setting the return value of stateManager when GetShape with id is called for the shape to be modified.
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            _handler.SetLastDrawn(null);

            // Perform modication
            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Translate, new Coordinate(2, 2),
                new Coordinate(3, 3), uid, DragPos.None, true);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // The UXObject at list position 2 should be the one to be created.
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            // verifying the output shape is the original one.
            var operationLine = (System.Windows.Shapes.Line) operations[1].WindowsShape;
            Assert.AreEqual(1, operationLine.X1);
            Assert.AreEqual(2, operationLine.Y1);
            Assert.AreEqual(3, operationLine.X2);
            Assert.AreEqual(2, operationLine.Y2);
        }

        [Test]
        public void ModifyShapeRealTime_InvalidRequestToRealTimeCreation_ReturnsNull()
        {
            // set last drawn in context of handler
            var uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.Create, Operation.Modify);

            // user gives invalid id to the shape id to continue the real time create operation in progress.
            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Translate, new Coordinate(3, 3),
                new Coordinate(5, 5), "12", DragPos.None, true);
            Assert.IsNull(operations);
        }

        [Test]
        public void ChangeStrokeWidth_ChangeWidthSuccessfully_ReturnsList()
        {
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid);
            var operations = _handler.ChangeStrokeWidth(2, "123");

            // Previous Shape Checks
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // New Shape checks
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            // check the object being sent to the server.
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj =>
                    obj.Uid.Equals("123") && Comparators.AreDecimalEqual(obj.MainShapeDefiner.StrokeWidth, 2) &&
                    obj.RecentOperation == Operation.Modify)
            ), Times.Once());
        }

        [Test]
        public void ChangeStrokeWidth_ServerUpdateFailure_ReturnsNull()
        {
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            var operations = _handler.ChangeStrokeWidth(2, "123");

            Assert.IsNull(operations);
        }

        // to do
        [Test]
        public void ChangeStrokeColor_ChangeColorSuccess_ReturnsList()
        {
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid);
            BoardColor color = new(300, 300, 300);
            BoardColor expectedColor = new(255, 255, 255);
            var operations = _handler.ChangeStrokeColor(color.Clone(), "123");

            // Previous Shape Checks
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // New Shape checks
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj =>
                    obj.Uid.Equals("123") && obj.MainShapeDefiner.StrokeColor.Equals(expectedColor) &&
                    obj.RecentOperation == Operation.Modify)
            ), Times.Once());
        }

        [Test]
        public void ChangeStrokeColor_ServerUpdateFailure_ReturnsNull()
        {
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            var operations = _handler.ChangeStrokeColor(new BoardColor(1, 2, 3), "123");

            Assert.IsNull(operations);
        }

        [Test]
        public void ChangeShapeFill_ChangesShapeFillSuccess_ReturnsList()
        {
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid);
            BoardColor color = new(255, 255, 3);
            var operations = _handler.ChangeShapeFill(color.Clone(), "123");

            // Previous Shape Checks
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // New Shape checks
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj =>
                    obj.Uid.Equals("123") && obj.MainShapeDefiner.ShapeFill.Equals(color) &&
                    obj.RecentOperation == Operation.Modify)
            ), Times.Once());
        }

        [Test]
        public void ChangeShapeFill_ServerUpdateFailure_ReturnsNull()
        {
            // server sending a failure in saveoperation
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            var operations = _handler.ChangeShapeFill(new BoardColor(1, 2, 3), "123");

            Assert.IsNull(operations);
        }


        [Test]
        public void Delete_ValidIdSuccessDelete_ReturnsList()
        {
            // setup stateManager with an object to return
            var uid = "<+_+>";
            // setup return value from manager.
            MainShape mainShape = new Polyline(new Coordinate(1, 1));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.Create);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            // set up the response of server on update
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            var operations = _handler.Delete(uid);
            Assert.IsTrue(operations.Count == 1);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);
            Assert.AreEqual(UXOperation.Delete, operations[0].UxOperation);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => obj.Uid.Equals(uid) && obj.RecentOperation == Operation.Delete)
            ), Times.Once());
        }

        [Test]
        public void Delete_InvalidId_SuccessDelete()
        {
            // set up the manager to retun false in save state.
            var uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            var operations = _handler.Delete("123");

            Assert.IsNull(operations);
        }

        [Test]
        public void GetUserName()
        {
            // setup return value from manager.
            MainShape mainShape = new Line(2, 2, new Coordinate(1, 1), new Coordinate(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, "123", "alice", Operation.Create);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);

            var username = _handler.GetUserName("123");

            Assert.AreEqual("alice", username);
        }

        [Test]
        public void GetUserName_ShapeDoesNotExistWithinServerReturnsNull()
        {
            // setup return value from manager.
            BoardShape shape = null;
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);

            var username = _handler.GetUserName("123");
            Assert.IsNull(username);
        }

        private void SetupManagerForNonRealTimeOperations(string uid, bool setSuccess = true)
        {
            // setup return value from manager.
            MainShape mainShape = new Line(2, 2, new Coordinate(1, 1), new Coordinate(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.Create);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            // set up the response of server on update
            if (setSuccess)
                _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);
            else
                _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);
        }

        private void SetHandlerLastDrawn(string uid, RealTimeOperation operation, Operation recentOperation)
        {
            // creating a shape with start 1,1 end 3,3
            MainShape mainShape = new Rectangle(2, 2, new Coordinate(1, 1), new Coordinate(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", recentOperation);
            _handler.SetLastDrawn(shape, new Coordinate(3, 3), operation);
        }
    }
}