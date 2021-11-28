/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/28/2021
**/

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    class ActiveBoardOperationsHandlerTests
    {
        private ActiveBoardOperationsHandler _handler;
        private Mock<IClientBoardStateManagerInternal> _mockStateManager;

        [SetUp]
        public void SetUp()
        {
            _handler = new ActiveBoardOperationsHandler();
            _mockStateManager = new();
            _handler.SetStateManager(_mockStateManager.Object);
        }

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
            List<UXShape> operations = _handler.CreateShape(ShapeType.ELLIPSE, start, end, strokeWidth, strokecolor, null);
            _mockStateManager.Verify(m => m.GetUser(), Times.Once());
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 1);

            // Verifying the UXShape received
            Comparators.CheckUXShape(operations[0], UXOperation.CREATE, ShapeType.ELLIPSE, new Coordinate(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(2, operations[0].WindowsShape.Width);
            Assert.AreEqual(2, operations[0].WindowsShape.Height);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(operations[0].WindowsShape.Uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.CREATE, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.ELLIPSE, lastDrawn.MainShapeDefiner.ShapeIdentifier);
        }

        [Test]
        public void CreateShape_NewObjectCreationSuccessorCallsNoServerStateUpdate_ReturnsList()
        {
            // mock the state manager to return userid so that handler does not throw an exception
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");

            // creation of _lastDrawnShape for real time rendering
            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.CREATE, Operation.CREATE);

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(3, 3);
            Coordinate end = new(5, 5);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, start, end, strokeWidth, strokecolor, uid);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Comparators.CheckUXShape(operations[0], UXOperation.DELETE, ShapeType.RECTANGLE, new(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);


            // The UXObject at list position 2 should be the one to be created.
            Comparators.CheckUXShape(operations[1], UXOperation.CREATE, ShapeType.RECTANGLE, new(3, 3), 0);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);
            Assert.AreEqual(4, operations[1].WindowsShape.Width);
            Assert.AreEqual(4, operations[1].WindowsShape.Height);
            Assert.IsTrue(operations[1].TranslationCoordinate.Equals(new(3, 3)));


            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.CREATE, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.RECTANGLE, lastDrawn.MainShapeDefiner.ShapeIdentifier);
        }

        [Test]
        public void CreateShape_NewShapeCreationSuccessorCallsSendServerStateUpdate_ReturnsList()
        {
            // mock 2 functionalities of the state manager to return values as needed
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            // creation of _lastDrawnShape for real time rendering
            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.CREATE, Operation.CREATE);

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(3, 3);
            Coordinate end = new(5, 5);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            // create shape succedding the first operation i.e last drawn shape exists in the context of handler
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, start, end, strokeWidth, strokecolor, uid, true);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNull(lastDrawn);

            // verify the boardshape being sent to the server
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => ((obj.Uid == uid) && (obj.MainShapeDefiner.Height == 4) &&
                                          (obj.MainShapeDefiner.Width == 4) && obj.MainShapeDefiner.Center.Equals(start)))
                ), Times.Once());
        }

        [Test]
        public void CreateShape_InvalidRequestToRealTimeCreation_ReturnsNull()
        {
            // set last drawn in context of handler
            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.CREATE, Operation.CREATE);

            // user gives invalid id to the shape id to continue the real time create operation in progress.
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, new(3, 3), new(5, 5), 2, new(2, 3, 4), "12", true);
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateShape_ServerReturnFalseWhileUpdating_ReturnsNull()
        {
            // mimic the state manager sending a false
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);

            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.CREATE, Operation.CREATE);
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, new(3, 3), new(5, 5), 2, new(2, 3, 4), uid, true);
            Assert.IsNull(operations);
        }

        //Testing the Modify shape realtime operation
        [Test]
        public void ModifyShapeRealTime_LocalModificationToShapeWithNoServerUpdate_ReturnList()
        {
            // Setting the return value of stateManager when GetShape with id is called for the shape to be modified.
            string uid = "123";
            float angleOfRotation = (float)(Math.PI / 2);
            // Testing with a line and translate function
            MainShape mainShape = new Line(angleOfRotation, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            _handler.SetLastDrawn(null);

            // Perform modication
            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.TRANSLATE, new(2, 2), new(3, 3), "123", DragPos.NONE, false);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Comparators.CheckUXShape(operations[0], UXOperation.DELETE, ShapeType.LINE, new(0, 0), angleOfRotation);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);


            // The UXObject at list position 2 should be the one to be created.
            Comparators.CheckUXShape(operations[1], UXOperation.CREATE, ShapeType.LINE, new(0, 0), angleOfRotation);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            System.Windows.Shapes.Line operationLine = (System.Windows.Shapes.Line)operations[1].WindowsShape;
            Assert.AreEqual(2, operationLine.X1);
            Assert.AreEqual(3, operationLine.Y1);
            Assert.AreEqual(4, operationLine.X2);
            Assert.AreEqual(3, operationLine.Y2);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.MODIFY, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.LINE, lastDrawn.MainShapeDefiner.ShapeIdentifier);

        }

        [Test]
        public void ModifyShapeRealTime_LocalSuccessorModificationNoServerUpdate_ReturnsList()
        {
            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.ROTATE, Operation.MODIFY);

            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.ROTATE, new(3, 3), new(2, 3), uid, DragPos.NONE, false);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Comparators.CheckUXShape(operations[0], UXOperation.DELETE, ShapeType.RECTANGLE, new(2, 2), 0);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // The UXObject at list position 2 should be the one to be created.
            Comparators.CheckUXShape(operations[1], UXOperation.CREATE, ShapeType.RECTANGLE, new(2, 2), (float)-Math.PI / 4);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(uid, lastDrawn.Uid);
            Assert.AreEqual(Operation.MODIFY, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.RECTANGLE, lastDrawn.MainShapeDefiner.ShapeIdentifier);

        }

        [Test]
        public void ModifyShapeRealTime_LocalModificationWithServerUpdate_ReturnList()
        {
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            // Calling resize operation on the uid
            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.RESIZE, Operation.MODIFY);

            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.RESIZE, new(3, 3), new(4, 4), uid, DragPos.TOP_RIGHT, true);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNull(lastDrawn);

            // check the object being sent to the server.
            Coordinate cen = new(2, 2);
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => ((obj.Uid == uid) && Comparators.AreDecimalEqual(obj.MainShapeDefiner.Height, 4) &&
                                           Comparators.AreDecimalEqual(obj.MainShapeDefiner.Height, 4) && obj.MainShapeDefiner.Center.Equals(cen)))
                ), Times.Once());
        }

        [Test]
        public void ModifyShapeRealTime_TryToPerformCreationThroughModification_ReturnNull()
        {
            // Setting the return value of stateManager when GetShape with id is called for the shape to be modified.
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            _handler.SetLastDrawn(null);

            // Perform modication
            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.CREATE, new(2, 2), new(3, 3), uid, DragPos.NONE, true);
            Assert.IsNull(operations);

        }

        [Test]
        public void ModifyShapeRealTime_ModificationToShapeServerUpdateFails_ReturnUndoList()
        {
            // Setting the return value of stateManager when GetShape with id is called for the shape to be modified.
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            _handler.SetLastDrawn(null);

            // Perform modication
            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.TRANSLATE, new(2, 2), new(3, 3), uid, DragPos.NONE, true);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // The UXObject at list position 2 should be the one to be created.
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            // verifying the output shape is the original one.
            System.Windows.Shapes.Line operationLine = (System.Windows.Shapes.Line)operations[1].WindowsShape;
            Assert.AreEqual(1, operationLine.X1);
            Assert.AreEqual(2, operationLine.Y1);
            Assert.AreEqual(3, operationLine.X2);
            Assert.AreEqual(2, operationLine.Y2);
        }

        [Test]
        public void ModifyShapeRealTime_InvalidRequestToRealTimeCreation_ReturnsNull()
        {
            // set last drawn in context of handler
            string uid = "123";
            SetHandlerLastDrawn(uid, RealTimeOperation.CREATE, Operation.MODIFY);

            // user gives invalid id to the shape id to continue the real time create operation in progress.
            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.TRANSLATE, new(3, 3), new(5, 5), "12", DragPos.NONE, true);
            Assert.IsNull(operations);
        }

        [Test]
        public void ChangeStrokeWidth_ChangeWidthSuccessfully_ReturnsList()
        {
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid);
            List<UXShape> operations = _handler.ChangeStrokeWidth(2, "123");

            // Previous Shape Checks
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // New Shape checks
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            // check the object being sent to the server.
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && Comparators.AreDecimalEqual(obj.MainShapeDefiner.StrokeWidth, 2) && (obj.RecentOperation == Operation.MODIFY)))
                ), Times.Once());
        }

        [Test]
        public void ChangeStrokeWidth_ServerUpdateFailure_ReturnsNull()
        {
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.ChangeStrokeWidth(2, "123");

            Assert.IsNull(operations);
        }

        // to do
        [Test]
        public void ChangeStrokeColor_ChangeColorSuccess_ReturnsList()
        {
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid);
            BoardColor color = new(300, 300, 300);
            BoardColor expectedColor = new(255, 255, 255);
            List<UXShape> operations = _handler.ChangeStrokeColor(color.Clone(), "123");

            // Previous Shape Checks
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // New Shape checks
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && obj.MainShapeDefiner.StrokeColor.Equals(expectedColor) && (obj.RecentOperation == Operation.MODIFY)))
                ), Times.Once());
        }

        [Test]
        public void ChangeStrokeColor_ServerUpdateFailure_ReturnsNull()
        {
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.ChangeStrokeColor(new(1, 2, 3), "123");

            Assert.IsNull(operations);
        }

        [Test]
        public void ChangeShapeFill_ChangesShapeFillSuccess_ReturnsList()
        {
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid);
            BoardColor color = new(255, 255, 3);
            List<UXShape> operations = _handler.ChangeShapeFill(color.Clone(), "123");

            // Previous Shape Checks
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);

            // New Shape checks
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && obj.MainShapeDefiner.ShapeFill.Equals(color) && (obj.RecentOperation == Operation.MODIFY)))
                ), Times.Once());
        }

        [Test]
        public void ChangeShapeFill_ServerUpdateFailure_ReturnsNull()
        {
            // server sending a failure in saveoperation
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.ChangeShapeFill(new(1, 2, 3), "123");

            Assert.IsNull(operations);
        }


        [Test]
        public void Delete_ValidIdSuccessDelete_ReturnsList()
        {
            // setup stateManager with an object to return
            string uid = "<+_+>";
            // setup return value from manager.
            MainShape mainShape = new Polyline(new(1, 1));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            // set up the response of server on update
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            List<UXShape> operations = _handler.Delete(uid);
            Assert.IsTrue(operations.Count == 1);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);
            Assert.AreEqual(UXOperation.DELETE, operations[0].UxOperation);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals(uid) && (obj.RecentOperation == Operation.DELETE)))
                ), Times.Once());

        }

        [Test]
        public void Delete_InvalidId_SuccessDelete()
        {
            // set up the manager to retun false in save state.
            string uid = "123";
            SetupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.Delete("123");

            Assert.IsNull(operations);

        }

        [Test]
        public void GetUserName()
        {

            // setup return value from manager.
            MainShape mainShape = new Line(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, "123", "alice", Operation.CREATE);
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
            MainShape mainShape = new Line(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            // set up the response of server on update
            if (setSuccess)
            {
                _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);
            }
            else
            {
                _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);
            }
        }

        private void SetHandlerLastDrawn(string uid, RealTimeOperation operation, Operation recentOperation)
        {
            // creating a shape with start 1,1 end 3,3
            MainShape mainShape = new Rectangle(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", recentOperation);
            _handler.SetLastDrawn(shape, new(3, 3), operation);
        }

    }


}