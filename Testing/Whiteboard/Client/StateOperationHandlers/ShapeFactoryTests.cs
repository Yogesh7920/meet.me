/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/25/2021
 * Date Modified: 11/25/2021
**/

using System;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    internal class ShapeFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _random = new Random();
        }

        private Random _random;

        [Test]
        public void MainShapeCreatorFactory_createRectangle_ReturnsRectangle()
        {
            var sType = ShapeType.Rectangle;
            var rectangle = CreateShape(sType);

            // Checking the identity of the shape. Other params correctness checked in shape specific tests.
            Assert.AreEqual(sType, rectangle.ShapeIdentifier);
        }

        [Test]
        public void MainShapeCreatorFactory_createEllipse_ReturnsEllipse()
        {
            var sType = ShapeType.Ellipse;
            var ellipse = CreateShape(sType);

            Assert.AreEqual(sType, ellipse.ShapeIdentifier);
        }

        [Test]
        public void MainShapeCreatorFactory_createLine_ReturnsLine()
        {
            var sType = ShapeType.Line;
            var line = CreateShape(sType);

            Assert.AreEqual(sType, line.ShapeIdentifier);
        }

        [Test]
        public void MainShapeCreatorFactory_createPolyLine_ReturnsPolyLine()
        {
            var sType = ShapeType.Polyline;
            var polyline = CreateShape(sType);

            Assert.AreEqual(sType, polyline.ShapeIdentifier);
        }

        private MainShape CreateShape(ShapeType shapeType)
        {
            Coordinate start = new(_random.Next(0, 10), _random.Next(0, 10));
            Coordinate end = new(_random.Next(0, 10), _random.Next(0, 10));
            return ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, null);
        }

        [Test]
        public void MainShapeCreatorFactory_createUnknownShape_ThrowsException()
        {
            var sType = ShapeType.None;

            var e = Assert.Throws<Exception>(delegate { CreateShape(sType); });
            Assert.That(e.Message, Is.EqualTo("Invalid Object type"));
        }
    }
}