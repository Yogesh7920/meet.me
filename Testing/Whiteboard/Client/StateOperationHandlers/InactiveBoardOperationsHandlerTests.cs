/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/25/2021
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
    internal class InactiveBoardOperationsHandlerTests
    {
        [SetUp]
        public void SetUp()
        {
            _handler = new InactiveBoardOperationsHandler();
            _mockStateManager = new Mock<IClientBoardStateManagerInternal>();
            _handler.SetStateManager(_mockStateManager.Object);
        }

        private InactiveBoardOperationsHandler _handler;
        private Mock<IClientBoardStateManagerInternal> _mockStateManager;

        [Test]
        public void ChangeShapeFill_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.ChangeShapeFill(new BoardColor(0, 0, 0), "123"));
        }

        [Test]
        public void ChangeStrokeColor_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.ChangeStrokeColor(new BoardColor(0, 0, 0), "123"));
        }

        [Test]
        public void ChangeStrokeWidth_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.ChangeStrokeWidth(3, "123"));
        }

        [Test]
        public void CreateShape_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.CreateShape(ShapeType.Ellipse, new Coordinate(0, 0), new Coordinate(0, 0), 1,
                new BoardColor(0, 0, 0), "12"));
        }

        [Test]
        public void ModifyShapeRealTime_InactiveStateCreation_ReturnUndoList()
        {
            // setup return value from manager.
            var uid = "123";
            MainShape mainShape = new Line(2, 2, new Coordinate(1, 1), new Coordinate(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.Create);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);

            // shape creation.
            Coordinate start = new(1, 1);
            Coordinate end = new(2, 2);
            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Rotate, start, end, uid, DragPos.None);

            // Assertions to verify the correctness.
            Assert.AreEqual(2, operations.Count);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);
            Assert.AreEqual(UXOperation.Delete, operations[0].UxOperation);
            Assert.AreEqual(UXOperation.Create, operations[1].UxOperation);
        }

        [Test]
        public void ModifyShapeRealTime_InactiveStateCreationServerException_ReturnNull()
        {
            // setup return value from manager.
            BoardShape shape = null;
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);

            // shape creation.
            Coordinate start = new(1, 1);
            Coordinate end = new(2, 2);
            var operations = _handler.ModifyShapeRealTime(RealTimeOperation.Rotate, start, end, "uid", DragPos.None);

            // Assertions to verify the correctness.
            Assert.AreEqual(null, operations);
        }

        [Test]
        public void Delete_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.Delete("123"));
        }

        [Test]
        public void Undo_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.Undo());
        }

        [Test]
        public void Redo_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.Redo());
        }
    }
}