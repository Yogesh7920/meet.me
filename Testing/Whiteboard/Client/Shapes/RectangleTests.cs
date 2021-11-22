/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 15/12/2021
 * Date Modified: 15/22/2021
**/

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard;


namespace Testing.Whiteboard
{
    [TestFixture]
    class RectangleTests
    {

        private MainShape _rectangle;

        [SetUp]
        public void SetUp()
        {
            _rectangle = new Rectangle();
        }

        [Test]
        public void ShapeMaker_NewShape_ReturnsNewShape()
        {
            MainShape newRectangle = _rectangle.ShapeMaker(new Coordinate(1, 1), new Coordinate(3, 5), null);

            Comparators.Compare(newRectangle, new Coordinate(1, 1), new Coordinate(2, 3), 2, 4, 1, new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);

        }

        
        [Test, TestCaseSource(typeof(TestIterators), "ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape_TestCases")]
        public void ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape(float expectedWidth, float expectedHeight, Coordinate expectedCenter, Coordinate stopDrag)
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new (34, 5, 6);
            BoardColor fillColor = new (34, 5, 64);
            Coordinate start = new (3, 4);
            Coordinate center = new (4, 5);
            float angleOfRotation = 0;

            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor.Clone(), fillColor.Clone(), start.Clone(), center.Clone(), null, angleOfRotation);
            MainShape modification1 = _rectangle.ShapeMaker(new Coordinate(5, 6), stopDrag, previousMainShape);
            Assert.That(ReferenceEquals(previousMainShape, modification1));
            Comparators.Compare(modification1, start, expectedCenter, expectedHeight, expectedWidth, strokeWidth, strokeColor, fillColor, angleOfRotation);
        }


        [Test]
        public void Clone__CreatesClone()
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new (34, 5, 6);
            BoardColor fillColor = new (34, 5, 64);
            Coordinate start = new (3, 4);
            Coordinate center = new (4, 5);
            float angleOfRotation = (float)3.00;

            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor, fillColor, start, center, null, angleOfRotation);
            MainShape ClonedShape = previousMainShape.Clone();
            Comparators.Compare(ClonedShape, start, center, height, width, strokeWidth, strokeColor, fillColor, angleOfRotation);
            Assert.IsFalse(ReferenceEquals(previousMainShape, ClonedShape));
            Assert.IsFalse(ReferenceEquals(strokeColor, ClonedShape.StrokeColor));
            Assert.IsFalse(ReferenceEquals(fillColor, ClonedShape.ShapeFill));
            Assert.IsFalse(ReferenceEquals(start, ClonedShape.Start));
            Assert.IsFalse(ReferenceEquals(center, ClonedShape.Center));
            Assert.IsNull(ClonedShape.GetPoints());
        }

        


        [Test, TestCaseSource(typeof(TestIterators), "Rotate_Quad1_TestCases")]
        public void Rotate_Quad1ForwardAndBackwardRotation_RotatedShape(Coordinate end, float rotationAngle)
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new (34, 5, 6);
            BoardColor fillColor = new (34, 5, 64);
            Coordinate shapeStart = new (3, 4);
            Coordinate center = new (0, 0);
            float angleOfRotation = 0;
            Coordinate start = new (0, 1);

            // Perform Forward Rotation
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor, fillColor, shapeStart, center, null, angleOfRotation);
            previousMainShape.Rotate(start, end);
            Assert.IsTrue(Math.Abs(Math.Round(rotationAngle, 2) - Math.Round(previousMainShape.AngleOfRotation, 2)) < 0.02);

            // Perform Backward Rotation
            previousMainShape.Rotate(end, start);
            Assert.IsTrue(Math.Abs(Math.Round(angleOfRotation, 2) - Math.Round(previousMainShape.AngleOfRotation, 2)) < 0.02);

        }


        [Test, TestCaseSource(typeof(TestIterators), "Rotate_Quad2_TestCases")]
        public void Rotate_Quad2ForwardAndBackwardRotation_RotatedShape(Coordinate end, float rotationAngle)
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new (34, 5, 6);
            BoardColor fillColor = new (34, 5, 64);
            Coordinate shapeStart = new (3, 4);
            Coordinate center = new (0, 0);
            float angleOfRotation = (float)Math.PI / 2;
            Coordinate start = new (0, 1);

            // Perform Forward Rotation
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor, fillColor, shapeStart, center, null, angleOfRotation);
            previousMainShape.Rotate(start, end);
            Assert.IsTrue(Math.Abs(Math.Round(rotationAngle, 2) - Math.Round(previousMainShape.AngleOfRotation, 2)) < 0.02);

            // Perform Backward Rotation
            previousMainShape.Rotate(end, start);
            Assert.IsTrue(Math.Abs(Math.Round(angleOfRotation, 2) - Math.Round(previousMainShape.AngleOfRotation, 2)) < 0.02);
        }


        [Test, TestCaseSource(typeof(TestIterators), "Resize_AllLatch_TestCases")]
        public void Resize_AllLatch_ResizedShape(Coordinate end, float expectedHeight, float expectedWidth, DragPos dragPos)
        {
            float height = 4;
            float width = 4;
            float strokeWidth = 1;
            BoardColor strokeColor = new (34, 5, 6);
            BoardColor fillColor = new (34, 5, 64);
            Coordinate shapeStart = new (3, 4);
            Coordinate center = new (1, 1);
            float angleOfRotation = (float)Math.PI / 6;
            Coordinate start = new (0, 0);

            // create a new shape
            MainShape previousMainShape = new Rectangle(height, width, strokeWidth, strokeColor.Clone(), fillColor.Clone(), shapeStart.Clone(), center.Clone(), null, angleOfRotation);
            previousMainShape.ResizeAboutCenter(start, end, dragPos);

            // check whether the state formed is correct
            Comparators.Compare(previousMainShape, shapeStart, center, expectedHeight, expectedWidth, strokeWidth, strokeColor, fillColor, angleOfRotation);

        }

    }


}
