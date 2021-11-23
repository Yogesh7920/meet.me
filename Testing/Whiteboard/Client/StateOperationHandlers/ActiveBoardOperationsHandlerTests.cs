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

        private void SetHandlerBoardShape(string uid)
        {
            // creating a shape with start 1,1 end 3,3
            MainShape mainShape = new Rectangle(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _handler.SetLastDrawn(shape, new(3, 3), RealTimeOperation.CREATE);
        }

        [Test]
        public void CreateShape_NewObjectCreationSuccessorCalls_NoServerStateUpdate()
        {
            _mockStateManager.Setup(m => m.GetUser()).Returns("1");

            // creation of _lastDrawnShape for real time rendering
            string uid = "123";
            SetHandlerBoardShape(uid);

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
            SetHandlerBoardShape(uid);

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
            SetHandlerBoardShape(uid);
            List<UXShape> operations = _handler.CreateShape(ShapeType.RECTANGLE, new(3, 3), new(5,5), 2, new(2, 3, 4), "12", true);
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateShape_ServerReturnFalse_ThrowsException()
        {
            _mockStateManager.Setup(m => m.SaveOperation(It.IsAny<BoardShape>())).Returns(false);

            string uid = "123";
            SetHandlerBoardShape(uid);
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
            Assert.AreEqual(operations[1].WindowsShape.Height, 2);
            Assert.AreEqual(operations[1].WindowsShape.Width, 2);

            System.Windows.Shapes.Line operationLine = (System.Windows.Shapes.Line) operations[1].WindowsShape;
            Assert.AreEqual(operationLine.X1, -1);
            Assert.AreEqual(operationLine.Y1, -1);
            Assert.AreEqual(operationLine.X2, 1);
            Assert.AreEqual(operationLine.Y2, 1);
            Assert.IsTrue(operations[1].TranslationCoordinate.Equals(new(3, 3)));

            // checking the _lastDrawn Object has correct details
            BoardShape lastDrawn = _handler.GetLastDrawn();
            Assert.IsNotNull(lastDrawn);
            Assert.AreEqual(lastDrawn.Uid, uid);
            Assert.AreEqual(lastDrawn.RecentOperation, Operation.MODIFY);
            Assert.AreEqual(lastDrawn.MainShapeDefiner.ShapeIdentifier, ShapeType.LINE);

        }


        public void CheckUXShape(UXShape uxshape, UXOperation uXOperation, ShapeType shapetype, Coordinate translationCord, float angle)
        {
            Assert.AreEqual(uxshape.ShapeIdentifier, shapetype);
            Assert.AreEqual(uxshape.UxOperation, uXOperation);
            Assert.IsTrue(uxshape.TranslationCoordinate.Equals(translationCord));
            Assert.AreEqual(uxshape.AngleOfRotation, angle);
        }

    }


}
