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

namespace Testing.Whiteboard
{
    class PolylineTests
    {
        private MainShape _polyline;

        [SetUp]
        public void SetUp()
        {
            _polyline = new Polyline();
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

            MainShape previousMainShape = new Polyline(height, width, strokeWidth, strokeColor, fillColor, start, center, null, angleOfRotation);
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
            Coordinate start = new (1, 1);
            Coordinate end = new (-3, -5);
            MainShape newPolyline = _polyline.ShapeMaker(start, end, null);
            Console.WriteLine(newPolyline.GetPoints().Count);

            Comparators.Compare(newPolyline, new Coordinate(1, 1), new Coordinate(0, 0), 0, 0, 1, new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);

            // check whether the list of points in correctly made.
            List<Coordinate> elementList = new();
            elementList.Add(start);
            elementList.Add(end);
            
            Console.WriteLine("Here I am!");
            
            CollectionAssert.AreEqual(elementList, newPolyline.GetPoints());

        }

        [Test]
        public void ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape()
        {
            Coordinate start = new(1, 1);
            float strokeWidth = 1;
            BoardColor strokeColor = new (34, 5, 6);
            BoardColor fillColor = new (34, 5, 64);
            float angleOfRotation = 0;
            List<Coordinate> points = new ();
            points.Add(start.Clone());
            points.Add(new Coordinate(2, 2));
            List<Coordinate> pointClone = points.Select(cord => new Coordinate(cord.R, cord.C)).ToList();

            MainShape previousMainShape = new Polyline(0, 0, strokeWidth, strokeColor.Clone(), fillColor.Clone(), new(0, 0), new Coordinate(0, 0), points, 0);

            pointClone.Add(new (4,3));
            MainShape modification1 = _polyline.ShapeMaker(new (2, 2), new (4,3), previousMainShape);
            Assert.That(ReferenceEquals(previousMainShape, modification1));
            Comparators.Compare(modification1, new Coordinate(0, 0), new Coordinate(0, 0), 0, 0, strokeWidth, strokeColor, fillColor, angleOfRotation);

            CollectionAssert.AreEqual(pointClone, modification1.GetPoints());

        }

        [Test]
        public void Resize_PreviousShape_ReturnFalse()
        {
            Coordinate start = new(1, 1);
            Coordinate end = new(-3, -5);
            MainShape newPolyline = _polyline.ShapeMaker(start, end, null);
            Assert.IsFalse(newPolyline.ResizeAboutCenter(start.Clone(), end.Clone(), DragPos.TOP_RIGHT));
        }
    }
}

