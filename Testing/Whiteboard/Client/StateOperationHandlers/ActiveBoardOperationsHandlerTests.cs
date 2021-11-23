/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/22/2021
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
        public void CreateShape_NewObjectCreationFirstCall_NoServerStateUpdate()
        {
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(1, 1);
            Coordinate end = new(3, 3);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            _handler.SetLastDrawn(null);
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE,start, end, strokeWidth, strokecolor, null);
            _mockStateManager.Verify(m => m.GetUser(), Times.Once());
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 1);

            //Verifying the UXShape received
            CheckUXShape(operations[0], UXOperation.CREATE, ShapeType.RECTANGLE, new Coordinate(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(operations[0].WindowsShape.Width, 2);
            Assert.AreEqual(operations[0].WindowsShape.Height, 2);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(lastDrawn.Uid, operations[0].WindowsShape.Uid);
            Assert.AreEqual(lastDrawn.RecentOperation, Operation.CREATE);
            Assert.AreEqual(lastDrawn.MainShapeDefiner.ShapeIdentifier, ShapeType.RECTANGLE);
        }

        [Test]
        public void CreateShape_NewObjectCreationSuccessorCalls_NoServerStateUpdate()
        {
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");

            // creation of _lastDrawnShape for real time rendering
            string uid = "123";
            SetHandlerBoardShape(uid, RealTimeOperation.CREATE, Operation.CREATE);

            // Calls for the start and end of calling the function for real time rendering
            Coordinate start = new(3, 3);
            Coordinate end = new(5, 5);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE,start, end, strokeWidth, strokecolor, uid);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            CheckUXShape(operations[0], UXOperation.DELETE, ShapeType.RECTANGLE, new(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(operations[0].WindowsShape.Uid, uid);


            // The UXObject at list position 2 should be the one to be created.
            CheckUXShape(operations[1], UXOperation.CREATE, ShapeType.RECTANGLE, new(3, 3), 0);
            Assert.AreEqual(operations[1].WindowsShape.Uid, uid);
            Assert.AreEqual(operations[1].WindowsShape.Width, 4);
            Assert.AreEqual(operations[1].WindowsShape.Height, 4);
            Assert.IsTrue(operations[1].TranslationCoordinate.Equals(new(3, 3)));


            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(lastDrawn.Uid, uid);
            Assert.AreEqual(lastDrawn.RecentOperation, Operation.CREATE);
            Assert.AreEqual(lastDrawn.MainShapeDefiner.ShapeIdentifier, ShapeType.RECTANGLE);
        }

        [Test]
        public void CreateShape_NewObjectCreationSuccessorCalls_SendServerStateUpdate()
        {
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            // creation of _lastDrawnShape for real time rendering
            string uid = "123";
            SetHandlerBoardShape(uid, RealTimeOperation.CREATE, Operation.CREATE);

            // Calls for the start and end of calling the function for real time rendering

            Coordinate start = new(3, 3);
            Coordinate end = new(5, 5);

            // Properties for the shape to be rendered
            BoardColor strokecolor = new(2, 3, 4);
            float strokeWidth = 2;

            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, start, end, strokeWidth, strokecolor, uid, true);


            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNull(lastDrawn);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => ((obj.Uid == uid) && (obj.MainShapeDefiner.Height == 4) && (obj.MainShapeDefiner.Width == 4) && obj.MainShapeDefiner.Center.Equals(start)))
                ), Times.Once());
        }

        [Test]
        public void CreateShape_InvalidRequest_ThrowsException()
        {
            string uid = "123";
            SetHandlerBoardShape(uid, RealTimeOperation.CREATE, Operation.CREATE);
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, new(3, 3), new(5,5), 2, new(2, 3, 4), "12", true);
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateShape_ServerReturnFalse_ThrowsException()
        {
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);

            string uid = "123";
            SetHandlerBoardShape(uid, RealTimeOperation.CREATE, Operation.CREATE);
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, new(3, 3), new(5, 5), 2, new(2, 3, 4), uid, true);
            Assert.IsNull(operations);
        }

        //Testing the Modify shape realtime operation
        [Test]
        public void ModifyShapeRealTime_LocalModification_ModifyAndNoServerUpdate()
        {
            string uid = "123";
            MainShape mainShape = new Line(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            _handler.SetLastDrawn(null);

            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.TRANSLATE, new(2, 2), new(3, 3), "123", DragPos.NONE, false);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            CheckUXShape(operations[0], UXOperation.DELETE, ShapeType.LINE, new(2, 2), 0);
            Assert.IsNotNull(operations[0].WindowsShape.Uid);
            Assert.AreEqual(operations[0].WindowsShape.Uid, uid);


            // The UXObject at list position 2 should be the one to be created.
            CheckUXShape(operations[1], UXOperation.CREATE, ShapeType.LINE, new(3, 3), 0);
            Assert.AreEqual(operations[1].WindowsShape.Uid, uid);

            System.Windows.Shapes.Line operationLine = (System.Windows.Shapes.Line) operations[1].WindowsShape;
            Assert.AreEqual(operationLine.X1, 2);
            Assert.AreEqual(operationLine.Y1, 2);
            Assert.AreEqual(operationLine.X2, 4);
            Assert.AreEqual(operationLine.Y2, 4);
            Assert.IsTrue(operations[1].TranslationCoordinate.Equals(new(3, 3)));

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(lastDrawn.Uid, uid);
            Assert.AreEqual(lastDrawn.RecentOperation, Operation.MODIFY);
            Assert.AreEqual(lastDrawn.MainShapeDefiner.ShapeIdentifier, ShapeType.LINE);

        }

        [Test]
        public void ModifyShapeRealTime_LocalSuccessorModification_ModifyAndNoServerUpdate()
        {
            string uid = "123";
            SetHandlerBoardShape(uid, RealTimeOperation.ROTATE, Operation.MODIFY);

            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.ROTATE, new(3, 3), new(2, 3), uid, DragPos.NONE, false);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            //Verifying the UXShape received
            // The UXobject at list position 1 should be the one to be deleted
            CheckUXShape(operations[0], UXOperation.DELETE, ShapeType.RECTANGLE, new(2, 2), 0);
            Assert.AreEqual(operations[0].WindowsShape.Uid, uid);

            // The UXObject at list position 2 should be the one to be created.
            CheckUXShape(operations[1], UXOperation.CREATE, ShapeType.RECTANGLE, new(2, 2), (float)-Math.PI/4);
            Assert.AreEqual(operations[1].WindowsShape.Uid, uid);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(lastDrawn.Uid, uid);
            Assert.AreEqual(Operation.MODIFY, lastDrawn.RecentOperation);
            Assert.AreEqual(ShapeType.RECTANGLE, lastDrawn.MainShapeDefiner.ShapeIdentifier);

        }

        [Test]
        public void ModifyShapeRealTime_LocalModification_ModifyAndSendServerUpdate()
        {
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);

            string uid = "123";
            SetHandlerBoardShape(uid, RealTimeOperation.RESIZE, Operation.MODIFY);

            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.RESIZE, new(3, 3), new(4, 4), uid, DragPos.TOP_RIGHT, true);
            Assert.IsNotNull(operations);
            Assert.IsTrue(operations.Count == 2);

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNull(lastDrawn);

            Coordinate cen = new(2, 2);
            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => ((obj.Uid == uid) && DecimalEqual(obj.MainShapeDefiner.Height, 4) && DecimalEqual(obj.MainShapeDefiner.Height, 4) && obj.MainShapeDefiner.Center.Equals(cen)))
                ), Times.Once());
        }

        [Test]
        public void ChangeStrokeWidth_ChangeWidth_SuccessfulChange()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid);
            List<UXShape> operations = _handler.ChangeStrokeWidth(2, "123");

            // Previous Shape Checks
            Assert.AreEqual(operations[0].WindowsShape.Uid, uid);

            // New Shape checks
            Assert.AreEqual(operations[1].WindowsShape.Uid, uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && DecimalEqual(obj.MainShapeDefiner.StrokeWidth, 2) && (obj.RecentOperation == Operation.MODIFY)))
                ), Times.Once());
        }

        [Test]
        public void ChangeStrokeWidth_ServerUpdateFailure_ReturnsNull()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.ChangeStrokeWidth(2, "123");

            Assert.IsNull(operations);
        }

        // to do
        [Test]
        public void ChangeStrokeColor_ChangeColor_SuccessfulChange()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid);
            BoardColor color = new(300, 300, 300);
            BoardColor expectedColor = new(255, 255, 255);
            List<UXShape> operations = _handler.ChangeStrokeColor(color.Clone(), "123");

            // Previous Shape Checks
            Assert.AreEqual(operations[0].WindowsShape.Uid, uid);

            // New Shape checks
            Assert.AreEqual(operations[1].WindowsShape.Uid, uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && obj.MainShapeDefiner.StrokeColor.Equals(expectedColor) && (obj.RecentOperation == Operation.MODIFY)))
                ), Times.Once());
        }

        [Test]
        public void ChangeStrokeColor_ServerUpdateFailure_ReturnsNull()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.ChangeStrokeColor(new(1,2,3), "123");

            Assert.IsNull(operations);
        }

        // to do
        [Test]
        public void ChangeShapeFill_ChangesShapeFill_SuccessfulChange()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid);
            BoardColor color = new(255, 255, 3);
            List<UXShape> operations = _handler.ChangeShapeFill(color.Clone(), "123");

            // Previous Shape Checks
            Assert.AreEqual(operations[0].WindowsShape.Uid, uid);

            // New Shape checks
            Assert.AreEqual(operations[1].WindowsShape.Uid, uid);

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && obj.MainShapeDefiner.ShapeFill.Equals(color) && (obj.RecentOperation == Operation.MODIFY)))
                ), Times.Once());
        }

        [Test]
        public void ChangeShapeFill_ServerUpdateFailure_ReturnsNull()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.ChangeShapeFill(new(1, 2, 3), "123");

            Assert.IsNull(operations);
        }


        [Test]
        public void Delete_ValidId_SuccessDelete()
        {
            string uid = "123";
            setupManagerForNonRealTimeOperations(uid);
            List<UXShape> operations = _handler.Delete("123");
            Assert.IsTrue(operations.Count == 1);
            Assert.AreEqual(operations[0].WindowsShape.Uid, "123");

            _mockStateManager.Verify(m => m.SaveOperation(
                It.Is<BoardShape>(obj => (obj.Uid.Equals("123") && (obj.RecentOperation == Operation.DELETE)))
                ), Times.Once());

        }

        [Test]
        public void Delete_InvalidId_SuccessDelete()
        {

            string uid = "123";
            setupManagerForNonRealTimeOperations(uid, false);
            List<UXShape> operations = _handler.Delete( "123");

            Assert.IsNull(operations);

        }



        void setupManagerForNonRealTimeOperations(string uid, bool setSuccess = true)
        {
            MainShape mainShape = new Line(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);
            if (setSuccess)
            {
                _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(true);
            }
            else
            {
                _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);
            }
        }

        private bool DecimalEqual(float a, float b)
        {
            return (Math.Abs(Math.Round(a, 2) - Math.Round(b, 2)) < 0.02);
        }

        private void SetHandlerBoardShape(string uid, RealTimeOperation operation, Operation recentOperation)
        {
            // creating a shape with start 1,1 end 3,3
            MainShape mainShape = new Rectangle(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", recentOperation);
            _handler.SetLastDrawn(shape, new(3, 3), operation);
        }

        public void CheckUXShape(UXShape uxshape, UXOperation uXOperation, ShapeType shapetype, Coordinate translationCord, float angle)
        {
            Assert.AreEqual(uxshape.ShapeIdentifier, shapetype);
            Assert.AreEqual(uxshape.UxOperation, uXOperation);
            Assert.IsTrue(uxshape.TranslationCoordinate.Equals(translationCord));
            Assert.IsTrue(DecimalEqual(angle, uxshape.AngleOfRotation));
        }

    }


}
