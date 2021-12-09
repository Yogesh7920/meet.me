/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/27/2021
 * Date Modified: 11/28/2021
**/

using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    internal class WhiteBoardOperationsHandlerTests
    {
        [SetUp]
        public void SetUp()
        {
            _handler = new WhiteBoardOperationHandler(new Coordinate(0, 0));
            _mockBoardOperationsState = new Mock<ActiveBoardOperationsHandler>();
            _handler.SetOperationHandler(_mockBoardOperationsState.Object);
        }

        private WhiteBoardOperationHandler _handler;
        private Mock<ActiveBoardOperationsHandler> _mockBoardOperationsState;

        [Test]
        public void ChangeShapeFill_ReceivesValue_ReturnsSameValue()
        {
            List<UXShape> ret = new() {new UXShape(), new UXShape()};
            _mockBoardOperationsState.Setup(m => m.ChangeShapeFill(It.IsAny<BoardColor>(), It.IsAny<string>()))
                .Returns(ret);

            var operations = _handler.ChangeShapeFill(new BoardColor(0, 0, 0), "<+_+>");
            Assert.AreEqual(2, operations.Count);

            ret = null;
            _mockBoardOperationsState.Setup(m => m.ChangeShapeFill(It.IsAny<BoardColor>(), It.IsAny<string>()))
                .Returns(ret);

            operations = _handler.ChangeShapeFill(new BoardColor(0, 0, 0), "<+_+>");
            Assert.IsNull(operations);
        }

        [Test]
        public void ChangeStrokeColor_ReceivesValue_ReturnsSameValue()
        {
            List<UXShape> ret = new() {new UXShape(), new UXShape()};
            _mockBoardOperationsState.Setup(m => m.ChangeStrokeColor(It.IsAny<BoardColor>(), It.IsAny<string>()))
                .Returns(ret);

            var operations = _handler.ChangeStrokeColor(new BoardColor(0, 0, 0), "<+_+>");
            Assert.AreEqual(2, operations.Count);

            ret = null;
            _mockBoardOperationsState.Setup(m => m.ChangeStrokeColor(It.IsAny<BoardColor>(), It.IsAny<string>()))
                .Returns(ret);

            operations = _handler.ChangeStrokeColor(new BoardColor(0, 0, 0), "<+_+>");
            Assert.IsNull(operations);
        }

        [Test]
        public void ChangeStrokeWidth_ReceivesValue_ReturnsSameValue()
        {
            List<UXShape> ret = new() {new UXShape(), new UXShape()};
            _mockBoardOperationsState.Setup(m => m.ChangeStrokeWidth(It.IsAny<float>(), It.IsAny<string>()))
                .Returns(ret);

            var operations = _handler.ChangeStrokeWidth(2, "<+_+>");
            Assert.AreEqual(2, operations.Count);

            ret = null;
            _mockBoardOperationsState.Setup(m => m.ChangeStrokeWidth(It.IsAny<float>(), It.IsAny<string>()))
                .Returns(ret);

            operations = _handler.ChangeStrokeWidth(2, "<+_+>");
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateEllipse_ReceivesValue_ReturnsSameValue()
        {
            MockForCreation(new List<UXShape> {new(), new()});

            var operations =
                _handler.CreateEllipse(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.AreEqual(2, operations.Count);

            MockForCreation(null);

            operations = _handler.CreateEllipse(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateLine_ReceivesValue_ReturnsSameValue()
        {
            MockForCreation(new List<UXShape> {new(), new()});

            var operations =
                _handler.CreateLine(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.AreEqual(2, operations.Count);

            MockForCreation(null);

            operations = _handler.CreateLine(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.IsNull(operations);
        }

        [Test]
        public void CreatePolyline_ReceivesValue_ReturnsSameValue()
        {
            MockForCreation(new List<UXShape> {new(), new()});

            var operations =
                _handler.CreatePolyline(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.AreEqual(2, operations.Count);

            MockForCreation(null);

            operations =
                _handler.CreatePolyline(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.IsNull(operations);
        }

        [Test]
        public void CreateRectangle_ReceivesValue_ReturnsSameValue()
        {
            MockForCreation(new List<UXShape> {new(), new()});

            var operations =
                _handler.CreateRectangle(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.AreEqual(2, operations.Count);

            MockForCreation(null);

            operations =
                _handler.CreateRectangle(new Coordinate(0, 0), new Coordinate(0, 0), 1, new BoardColor(0, 0, 0));
            Assert.IsNull(operations);
        }

        [Test]
        public void DeleteShape_ReceivesValue_ReturnsSameValue()
        {
            List<UXShape> ret = new() {new UXShape(), new UXShape()};
            _mockBoardOperationsState.Setup(m => m.Delete(It.IsAny<string>())).Returns(ret);

            var operations = _handler.DeleteShape("<+_+>");
            Assert.AreEqual(2, operations.Count);

            ret = null;
            _mockBoardOperationsState.Setup(m => m.Delete(It.IsAny<string>())).Returns(ret);

            operations = _handler.DeleteShape("<+_+>");
            Assert.IsNull(operations);
        }

        [Test]
        public void Redo_ReceivesValue_ReturnsSameValue()
        {
            List<UXShape> ret = new() {new UXShape(), new UXShape()};
            _mockBoardOperationsState.Setup(m => m.Redo()).Returns(ret);

            var operations = _handler.Redo();
            Assert.AreEqual(2, operations.Count);

            ret = null;
            _mockBoardOperationsState.Setup(m => m.Redo()).Returns(ret);

            operations = _handler.Redo();
            Assert.AreEqual(0, operations.Count);
        }

        [Test]
        public void Undo_ReceivesValue_ReturnsSameValue()
        {
            List<UXShape> ret = new() {new UXShape(), new UXShape()};
            _mockBoardOperationsState.Setup(m => m.Undo()).Returns(ret);

            var operations = _handler.Undo();
            Assert.AreEqual(2, operations.Count);

            ret = null;
            _mockBoardOperationsState.Setup(m => m.Undo()).Returns(ret);

            operations = _handler.Undo();
            Assert.AreEqual(0, operations.Count);
        }

        [Test]
        public void ResizeShape_ReceivesValue_ReturnsSameValue()
        {
            MockForModification(new List<UXShape> {new(), new()});

            var operations = _handler.ResizeShape(new Coordinate(0, 0), new Coordinate(0, 0), "<+_+>", DragPos.None);
            Assert.AreEqual(2, operations.Count);

            MockForModification(null);

            operations = _handler.ResizeShape(new Coordinate(0, 0), new Coordinate(0, 0), "<+_+>", DragPos.None);
            Assert.IsNull(operations);
        }

        [Test]
        public void RotateShape_ReceivesValue_ReturnsSameValue()
        {
            MockForModification(new List<UXShape> {new(), new()});

            var operations = _handler.RotateShape(new Coordinate(0, 0), new Coordinate(0, 0), "<+_+>");
            Assert.AreEqual(2, operations.Count);

            MockForModification(null);

            operations = _handler.RotateShape(new Coordinate(0, 0), new Coordinate(0, 0), "<+_+>");
            Assert.IsNull(operations);
        }

        [Test]
        public void TranslateShape_ReceivesValue_ReturnsSameValue()
        {
            MockForModification(new List<UXShape> {new(), new()});

            var operations = _handler.TranslateShape(new Coordinate(0, 0), new Coordinate(0, 0), "<+_+>");
            Assert.AreEqual(2, operations.Count);

            MockForModification(null);

            operations = _handler.TranslateShape(new Coordinate(0, 0), new Coordinate(0, 0), "<+_+>");
            Assert.IsNull(operations);
        }

        [Test]
        public void SwitchState_switchingState_SuccessfulSwitch()
        {
            _handler.SwitchState();
            Assert.That(_handler.GetBoardOperationsState(), Is.TypeOf<InactiveBoardOperationsHandler>());
            _handler.SwitchState();
            Assert.That(_handler.GetBoardOperationsState(), Is.TypeOf<ActiveBoardOperationsHandler>());
        }

        private void MockForModification(List<UXShape> ret)
        {
            _mockBoardOperationsState.Setup(m => m.ModifyShapeRealTime(It.IsAny<RealTimeOperation>(),
                    It.IsAny<Coordinate>(), It.IsAny<Coordinate>(),
                    It.IsAny<string>(), It.IsAny<DragPos>(), It.IsAny<bool>()))
                .Returns(ret);
        }

        private void MockForCreation(List<UXShape> ret)
        {
            _mockBoardOperationsState.Setup(m => m.CreateShape(It.IsAny<ShapeType>(), It.IsAny<Coordinate>(),
                    It.IsAny<Coordinate>(), It.IsAny<float>(), It.IsAny<BoardColor>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(ret);
        }
    }
}