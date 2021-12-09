/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    internal class PolylineTests
    {
        private MainShape _polyline;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _polyline = new Polyline();
            _random = new Random();
        }

        [Test]
        public void Clone__CreatesPolylineClone_ReturnsClone()
        {
            // setting some parameters of the polyline to be cloned
            float height = _random.Next(0, 10);
            float width = _random.Next(0, 10);
            float strokeWidth = _random.Next(0, 3);
            BoardColor strokeColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            BoardColor fillColor = new(_random.Next(0, 200), _random.Next(0, 200), _random.Next(0, 200));
            Coordinate start = new(3, 4);
            Coordinate center = new(4, 5);
            var angleOfRotation = (float) 3.00;
            List<Coordinate> elementList = new();
            elementList.Add(start);

            // Creating a polyline to be cloned
            MainShape previousMainShape = new Polyline(height, width, strokeWidth, strokeColor, fillColor,
                start, center, elementList, angleOfRotation);
            var ClonedShape = previousMainShape.Clone();

            // checking clone polyline params
            Comparators.Compare(ClonedShape, start, center, height, width, strokeWidth, strokeColor, fillColor,
                angleOfRotation);
            Assert.IsFalse(ReferenceEquals(previousMainShape, ClonedShape));
            Assert.IsFalse(ReferenceEquals(strokeColor, ClonedShape.StrokeColor));
            Assert.IsFalse(ReferenceEquals(fillColor, ClonedShape.ShapeFill));
            Assert.IsFalse(ReferenceEquals(start, ClonedShape.Start));
            Assert.IsFalse(ReferenceEquals(center, ClonedShape.Center));
            Assert.IsFalse(ReferenceEquals(elementList, ClonedShape.GetPoints()));
            CollectionAssert.AreEqual(elementList, ClonedShape.GetPoints());
        }

        [Test]
        public void ShapeMaker_CreateNewShape_ReturnsNewShape()
        {
            // creation polyline from stratch based on start and end points
            Coordinate start = new(1, 1);
            Coordinate end = new(-3, -5);
            var newPolyline = _polyline.ShapeMaker(start, end);

            // check correctness of polyline
            // by default height and width should be set to 0
            float expectedHeight = 0;
            float expectedWidth = 0;
            Comparators.Compare(newPolyline, new Coordinate(1, 1), new Coordinate(0, 0), expectedHeight,
                expectedWidth, 1, new BoardColor(0, 0, 0), new BoardColor(255, 255, 255), 0);

            // check whether the list of points in correctly made.
            List<Coordinate> elementList = new();
            elementList.Add(start);
            elementList.Add(end);

            CollectionAssert.AreEqual(elementList, newPolyline.GetPoints());
        }

        [Test]
        public void ShapeMaker_ModifyPreviousShape_ReturnsModifiedPreviousShape()
        {
            // setting params for a new
            Coordinate start = new(1, 1);
            float strokeWidth = 1;
            BoardColor strokeColor = new(34, 5, 6);
            BoardColor fillColor = new(34, 5, 64);
            float angleOfRotation = 0;
            List<Coordinate> points = new();
            points.Add(start.Clone());
            points.Add(new Coordinate(2, 2));
            var pointClone = points.Select(cord => new Coordinate(cord.R, cord.C)).ToList();

            // creation of polyline for modification
            MainShape previousMainShape = new Polyline(0, 0, strokeWidth, strokeColor.Clone(), fillColor.Clone(),
                new Coordinate(0, 0), new Coordinate(0, 0), points, 0);

            // modifying and parameter checking
            pointClone.Add(new Coordinate(4, 3));
            var modification1 = _polyline.ShapeMaker(new Coordinate(2, 2), new Coordinate(4, 3), previousMainShape);
            Assert.That(ReferenceEquals(previousMainShape, modification1));
            Comparators.Compare(modification1, new Coordinate(0, 0), new Coordinate(0, 0), 0, 0, strokeWidth,
                strokeColor, fillColor, angleOfRotation);

            CollectionAssert.AreEqual(pointClone, modification1.GetPoints());
        }

        [Test]
        public void Resize_PreviousShape_ReturnFalse()
        {
            // Check resize of polyline not possible
            Coordinate start = new(1, 1);
            Coordinate end = new(-3, -5);
            var newPolyline = _polyline.ShapeMaker(start, end);
            Assert.IsFalse(newPolyline.ResizeAboutCenter(start.Clone(), end.Clone(), DragPos.TopRight));
        }

        [Test]
        public void Rotate_RotationOfPolyline_ReturnFalse()
        {
            // Check rotate of polyline not possible
            Coordinate start = new(1, 1);
            Coordinate end = new(-3, -5);
            var newPolyline = _polyline.ShapeMaker(start, end);
            Assert.IsFalse(newPolyline.Rotate(start.Clone(), end.Clone()));
        }
    }
}