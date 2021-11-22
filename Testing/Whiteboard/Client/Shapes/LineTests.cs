/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 15/22/2021
 * Date Modified: 15/22/2021
**/

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard;

namespace Testing.Whiteboard.Client.Shapes
{
    [TestFixture]
    class LineTests
    {
        private MainShape _line;

        [SetUp]
        public void SetUp()
        {
            _line = new Line();
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

            MainShape previousMainShape = new Line(height, width, strokeWidth, strokeColor, fillColor, start, center, null, angleOfRotation);
            MainShape ClonedShape = previousMainShape.Clone();
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
            MainShape newEllipse = _line.ShapeMaker(new Coordinate(1, 1), new Coordinate(-3, -5), null);

            Comparators.Compare(newEllipse, new Coordinate(1, 1), new Coordinate(-1, -2), 4, 6, 1, new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);

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

            MainShape previousMainShape = new Line(height, width, strokeWidth, strokeColor.Clone(), fillColor.Clone(), start.Clone(), center.Clone(), null, angleOfRotation);
            MainShape modification1 = _line.ShapeMaker(new Coordinate(5, 6), stopDrag, previousMainShape);
            Assert.That(ReferenceEquals(previousMainShape, modification1));
            Comparators.Compare(modification1, start, expectedCenter, expectedHeight, expectedWidth, strokeWidth, strokeColor, fillColor, angleOfRotation);
        }
    }
}
