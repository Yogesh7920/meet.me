/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/28/2021
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
    class EllipseTests
    {
        private MainShape _ellipse;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _ellipse = new Ellipse();
            _random = new();
        }

        [Test]
        public void Clone__CreatesEllipseClone_ReturnsClone()
        {

            // setting some parameters of the Ellipse to be cloned
            float height = _random.Next(0, 10);
            float width = _random.Next(0, 10);
            float strokeWidth = _random.Next(0, 3);
            BoardColor strokeColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            BoardColor fillColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            Coordinate start = new(3, 4);
            Coordinate center = new(4, 5);
            float angleOfRotation = (float)3.00;

            // create an ellipse
            MainShape previousMainShape = new Ellipse(height, width, strokeWidth, strokeColor,
                                                      fillColor, start, center, null, angleOfRotation);

            // clone creation
            MainShape ClonedShape = previousMainShape.Clone();

            // Parameter checking of clone
            Comparators.Compare(ClonedShape, start, center, height, width, strokeWidth, strokeColor, fillColor, angleOfRotation);
            Assert.IsFalse(ReferenceEquals(previousMainShape, ClonedShape));
            Assert.IsFalse(ReferenceEquals(strokeColor, ClonedShape.StrokeColor));
            Assert.IsFalse(ReferenceEquals(fillColor, ClonedShape.ShapeFill));
            Assert.IsFalse(ReferenceEquals(start, ClonedShape.Start));
            Assert.IsFalse(ReferenceEquals(center, ClonedShape.Center));
            Assert.IsNull(ClonedShape.GetPoints());
        }

        [Test]
        public void ShapeMaker_CreateNewShape_ReturnsNewShape()
        {
            // create an ellipse from stratch
            MainShape newEllipse = _ellipse.ShapeMaker(new Coordinate(1, 1), new Coordinate(3, 5), null);

            // parameter checking
            float expectedHeight = 2;
            float expectedWidth = 4;
            Comparators.Compare(newEllipse, new Coordinate(1, 1), new Coordinate(2, 3), expectedHeight,
                                expectedWidth, 1, new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);

        }

        [Test, TestCaseSource(typeof(TestIterators), "ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape_TestCases")]
        public void ShapeMaker_CreationFromPreviousShape_ReturnsModifiedShape(float expectedWidth, float expectedHeight,
                                                                          Coordinate expectedCenter, Coordinate stopDrag)
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            Coordinate start = new(3, 4);
            Coordinate center = new(4, 5);
            float angleOfRotation = 0;

            // Creating a previous ellipse to be used in the creation of new ellipse by modifying the previous one itself.
            MainShape previousMainShape = new Ellipse(height, width, strokeWidth, strokeColor.Clone(),
                                                      fillColor.Clone(), start.Clone(), center.Clone(), null, angleOfRotation);

            // Modify the previous ellipse
            MainShape modification1 = _ellipse.ShapeMaker(new Coordinate(5, 6), stopDrag, previousMainShape);

            // Check other conditions
            Assert.That(ReferenceEquals(previousMainShape, modification1));
            Comparators.Compare(modification1, start, expectedCenter, expectedHeight, expectedWidth,
                                strokeWidth, strokeColor, fillColor, angleOfRotation);
        }

    }
}
