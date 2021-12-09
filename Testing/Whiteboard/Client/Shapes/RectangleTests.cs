/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/12/2021
 * Date Modified: 11/28/2021
**/

using System;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    internal class RectangleTests
    {
        [SetUp]
        public void SetUp()
        {
            _rectangle = new Rectangle();
            _random = new Random();
        }

        private MainShape _rectangle;
        private Random _random;

        [Test]
        public void ShapeMaker_NewRectangleCreation_ReturnsNewRectangle()
        {
            // creation of a new Rectangle without any previous inputs.
            var newRectangle = _rectangle.ShapeMaker(new Coordinate(1, 1), new Coordinate(3, 5));

            // compare expected and received
            float expectedHeight = 2;
            float expectedWidth = 4;
            Comparators.Compare(newRectangle, new Coordinate(1, 1), new Coordinate(2, 3), expectedHeight, expectedWidth,
                1,
                new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);
        }


        [Test]
        [TestCaseSource(typeof(TestIterators), "ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape_TestCases")]
        public void ShapeMaker_CreationFromPreviousShape_ReturnsModifiedShape(float expectedWidth, float expectedHeight,
            Coordinate expectedCenter, Coordinate stopDrag)
        {
            // setting some parameters of the previous Rectangle which will be used to create new shape.
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate start = new(3, 4);
            Coordinate center = new(4, 5);
            float angleOfRotation = 0;

            // create precious Rectangle from above params.
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor.Clone(),
                fillColor.Clone(), start.Clone(), center.Clone(), null, angleOfRotation);

            // Modify previous rectangle at the same pointer, to make a new shape
            var modification1 = _rectangle.ShapeMaker(new Coordinate(5, 6), stopDrag, previousMainShape);

            // Since the shape is modified at the same reference.. their points should be the same
            Assert.That(ReferenceEquals(previousMainShape, modification1));

            // Compare left over params
            Comparators.Compare(modification1, start, expectedCenter, expectedHeight, expectedWidth,
                strokeWidth, strokeColor, fillColor, angleOfRotation);
        }


        [Test]
        public void Clone__CreatesRectangleClone_ReturnsClone()
        {
            // setting some parameters of the Rectangle to be cloned
            float height = _random.Next(0, 10);
            float width = _random.Next(0, 10);
            float strokeWidth = _random.Next(0, 3);
            BoardColor strokeColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            BoardColor fillColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            Coordinate start = new(_random.Next(0, 10), _random.Next(0, 10));
            Coordinate center = new(_random.Next(0, 10), _random.Next(0, 10));
            var angleOfRotation = (float) 3.00;

            // creating a rectangle
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor, fillColor, start,
                center, null, angleOfRotation);

            // cloning the rectangle
            var ClonedShape = previousMainShape.Clone();

            // Performing Checks that cloned is different than original
            Comparators.Compare(ClonedShape, start, center, height, width, strokeWidth, strokeColor, fillColor,
                angleOfRotation);
            Assert.IsFalse(ReferenceEquals(previousMainShape, ClonedShape));
            Assert.IsFalse(ReferenceEquals(strokeColor, ClonedShape.StrokeColor));
            Assert.IsFalse(ReferenceEquals(fillColor, ClonedShape.ShapeFill));
            Assert.IsFalse(ReferenceEquals(start, ClonedShape.Start));
            Assert.IsFalse(ReferenceEquals(center, ClonedShape.Center));
            Assert.IsNull(ClonedShape.GetPoints());
        }

        [Test]
        public void AngleOfRotation_GreaterThanPi()
        {
            MainShape rectangle = new Rectangle(2, 2, new Coordinate(0, 0), new Coordinate(1, 1))
            {
                AngleOfRotation = (float) (2 * Math.PI + 1)
            };
            Comparators.CompareFloats(rectangle.AngleOfRotation, 1);
        }

        [Test]
        public void AngleOfRotation_LessThanPi()
        {
            MainShape rectangle = new Rectangle(2, 2, new Coordinate(0, 0), new Coordinate(1, 1))
            {
                AngleOfRotation = (float) (-2 * Math.PI - 1)
            };
            Comparators.CompareFloats(rectangle.AngleOfRotation, -1);
        }


        [Test]
        [TestCaseSource(typeof(TestIterators), "Rotate_Quad1_TestCases")]
        public void Rotate_Quad1ForwardAndBackwardRotation_ReturnsRotatedShape(Coordinate end, float rotationAngle)
        {
            // set shape params
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate shapeStart = new(3, 4);
            Coordinate center = new(0, 0);
            float angleOfRotation = 0;
            Coordinate start = new(0, 1);

            // Perform Forward Rotation
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor, fillColor, shapeStart,
                center, null, angleOfRotation);
            previousMainShape.Rotate(start, end);
            Comparators.CompareFloats(rotationAngle, previousMainShape.AngleOfRotation);

            // Perform Backward Rotation
            previousMainShape.Rotate(end, start);
            Comparators.CompareFloats(angleOfRotation, previousMainShape.AngleOfRotation);
        }


        [Test]
        [TestCaseSource(typeof(TestIterators), "Rotate_Quad2_TestCases")]
        public void Rotate_Quad2ForwardAndBackwardRotation_ReturnsRotatedShape(Coordinate end, float rotationAngle)
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate shapeStart = new(3, 4);
            Coordinate center = new(0, 0);
            var angleOfRotation = (float) Math.PI / 2;
            Coordinate start = new(0, 1);

            // Perform Forward Rotation
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor,
                fillColor, shapeStart, center, null, angleOfRotation);
            previousMainShape.Rotate(start, end);
            Comparators.CompareFloats(rotationAngle, previousMainShape.AngleOfRotation);

            // Perform Backward Rotation
            previousMainShape.Rotate(end, start);
            Comparators.CompareFloats(angleOfRotation, previousMainShape.AngleOfRotation);
        }


        [Test]
        [TestCaseSource(typeof(TestIterators), "Resize_AllLatch_TestCases")]
        public void Resize_AllLatch_ReturnsResizedShape(Coordinate end, float expectedHeight, float expectedWidth,
            DragPos dragPos)
        {
            float height = 4;
            float width = 4;
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate shapeStart = new(3, 4);
            Coordinate center = new(1, 1);
            var angleOfRotation = (float) Math.PI / 6;
            Coordinate start = new(0, 0);

            // create a new shape
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor.Clone(),
                fillColor.Clone(), shapeStart.Clone(), center.Clone(), null, angleOfRotation);
            var successFlag = previousMainShape.ResizeAboutCenter(start, end, dragPos);

            // check whether the state formed is correct
            Comparators.Compare(previousMainShape, shapeStart, center, expectedHeight, expectedWidth,
                strokeWidth, strokeColor, fillColor, angleOfRotation);
            Assert.IsTrue(successFlag);
        }

        [Test]
        public void Resize_DragPosisNone_ReturnsFalse()
        {
            Coordinate start = new(_random.Next(0, 10), _random.Next(0, 10));
            Coordinate center = new(_random.Next(0, 10), _random.Next(0, 10));
            float height = _random.Next(0, 10);
            float width = _random.Next(0, 10);
            MainShape rectangle = new Rectangle(height, width, start, center);
            var successFlag = rectangle.ResizeAboutCenter(new Coordinate(0, 0), new Coordinate(1, 1), DragPos.None);

            Assert.IsFalse(successFlag);
            Comparators.Compare(rectangle, start, center, height, width, 1,
                new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);
        }
    }
}