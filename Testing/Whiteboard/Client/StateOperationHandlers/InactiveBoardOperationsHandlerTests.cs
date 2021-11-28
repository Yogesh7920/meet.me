/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/25/2021
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
    class InactiveBoardOperationsHandlerTests
    {
        private InactiveBoardOperationsHandler _handler;
        private Mock<IClientBoardStateManagerInternal> _mockStateManager;

        [SetUp]
        public void SetUp()
        {
            _handler = new InactiveBoardOperationsHandler();
            _mockStateManager = new();
            _handler.SetStateManager(_mockStateManager.Object);
        }

        [Test]
        public void ChangeShapeFill_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.ChangeShapeFill(new(0, 0, 0), "123"));
        }

        [Test]
        public void ChangeStrokeColor_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.ChangeStrokeColor(new(0, 0, 0), "123"));
        }

        [Test]
        public void ChangeStrokeWidth_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.ChangeStrokeWidth(3, "123"));
        }

        [Test]
        public void CreateShape_RequestChange_ReturnsEmptyList()
        {
            Assert.IsEmpty(_handler.CreateShape(ShapeType.ELLIPSE, new(0, 0), new(0, 0), 1, new(0, 0, 0), "12"));
        }

        [Test]
        public void ModifyShapeRealTime_InactiveStateCreation_ReturnUndoList()
        {
            // setup return value from manager.
            string uid = "123";
            MainShape mainShape = new Line(2, 2, new(1, 1), new(2, 2));
            BoardShape shape = new(mainShape, 0, DateTime.Now, DateTime.Now, uid, "1", Operation.CREATE);
            _mockStateManager.Setup(m => m.GetBoardShape(It.IsAny<string>())).Returns(shape);

            // shape creation.
            Coordinate start = new(1, 1);
            Coordinate end = new(2, 2);
            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.ROTATE, start, end, uid, DragPos.NONE);

            // Assertions to verify the correctness.
            Assert.AreEqual(2, operations.Count);
            Assert.AreEqual(uid, operations[0].WindowsShape.Uid);
            Assert.AreEqual(uid, operations[1].WindowsShape.Uid);
            Assert.AreEqual(UXOperation.DELETE, operations[0].UxOperation);
            Assert.AreEqual(UXOperation.CREATE, operations[1].UxOperation);
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
            List<UXShape> operations = _handler.ModifyShapeRealTime(RealTimeOperation.ROTATE, start, end, "uid", DragPos.NONE);

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
