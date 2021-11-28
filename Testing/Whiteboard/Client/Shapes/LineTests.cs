/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/28/2021
**/

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    class LineTests
    {
        private MainShape _line;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _line = new Line();
            _random = new();
        }

        [Test]
        public void Clone__CreatesLineClone_ReturnsClone()
        {
            // setting some parameters of the line to be cloned
            float height = _random.Next(0, 10);
            float width = _random.Next(0, 10);
            float strokeWidth = _random.Next(0, 3);
            BoardColor strokeColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            BoardColor fillColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            Coordinate start = new(3, 4);
            Coordinate center = new(4, 5);
            float angleOfRotation = (float)3.00;

            // creating a line to be cloned
            MainShape previousMainShape = new Line(height, width, strokeWidth, strokeColor, fillColor, start, center, null, angleOfRotation);
            MainShape ClonedShape = previousMainShape.Clone();

            // checker on clone to verify it is correct.
            Comparators.Compare(ClonedShape, start, center, height, width, strokeWidth, strokeColor, fillColor, angleOfRotation);
            Assert.IsFalse(ReferenceEquals(previousMainShape, ClonedShape));
            Assert.IsFalse(ReferenceEquals(strokeColor, ClonedShape.StrokeColor));
            Assert.IsFalse(ReferenceEquals(fillColor, ClonedShape.ShapeFill));
            Assert.IsFalse(ReferenceEquals(start, ClonedShape.Start));
            Assert.IsFalse(ReferenceEquals(center, ClonedShape.Center));
            Assert.IsNull(ClonedShape.GetPoints());
        }
        [Test]
        public void ShapeMaker_NewShape_ReturnsNewShape()
        {
            // creation of ellipse from stratch.
            MainShape newLine = _line.ShapeMaker(new Coordinate(-1, 2), new Coordinate(1, 0), null);

            // verify if it is the expected ellipse
            float expectedWidth = (float)Math.Sqrt(8);
            Comparators.Compare(newLine, new(-1, 2), new(0, 1), 0,
                                expectedWidth, 1, new(0, 0, 0), new(255, 255, 255), (float)(3 * Math.PI / 4));
        }

        [Test, TestCaseSource(typeof(TestIterators), "ShapeMaker_PreviousLine_ReturnsModifiedPreviousShape_TestCases")]
        public void ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape(float expectedWidth, Coordinate expectedCenter,
                                                                          Coordinate stopDrag, float expectedAngle)
        {
            // setting parameters for ellipse.
            float height = 0;
            float width = (float)Math.Sqrt(8);
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate start = new(2, 2);
            Coordinate center = new(3, 3);
            float angleOfRotation = (float)Math.PI / 4;

            // creating a ellipse to be modified by shapeMaker.
            MainShape previousMainShape = new Line(height, width, strokeWidth, strokeColor.Clone(),
                                                   fillColor.Clone(), start.Clone(), center.Clone(), null, angleOfRotation);
            MainShape modification1 = _line.ShapeMaker(new Coordinate(4, 4), stopDrag, previousMainShape);

            // Modified shape verification
            Assert.That(ReferenceEquals(previousMainShape, modification1));
            Comparators.Compare(modification1, start, expectedCenter, 0, expectedWidth, strokeWidth, strokeColor,
                                fillColor, expectedAngle);
        }

        [Test, TestCaseSource(typeof(TestIterators), "Resize_AllLatchsforLine_TestCases")]
        public void ResizeAboutCenter(float expectedWidth, Coordinate end, DragPos drapgPos)
        {
            // setting parameters for line.
            float height = 0;
            float width = (float)Math.Sqrt(32);
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate start = new(1, 1);
            Coordinate center = new(3, 3);
            float angleOfRotation = (float)Math.PI / 6;

            // create a new shape
            // creating a line to be modified by shapeMaker.
            MainShape previousMainShape = new Line(height, width, strokeWidth, strokeColor.Clone(),
                                                   fillColor.Clone(), start.Clone(), center.Clone(), null, angleOfRotation);

            previousMainShape.ResizeAboutCenter(new Coordinate(0, 0), end, drapgPos);

            // check whether the state formed is correct
            Comparators.Compare(previousMainShape, start, center, 0, expectedWidth,
                                strokeWidth, strokeColor, fillColor, angleOfRotation);
        }
    }
}