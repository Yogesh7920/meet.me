/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/25/2021
 * Date Modified: 11/25/2021
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
    class ShapeFactoryTests
    {
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _random = new();
        }

        [Test]
        public void MainShapeCreatorFactory_createRectangle_ReturnsRectangle()
        {
            ShapeType sType = ShapeType.RECTANGLE;
            MainShape rectangle = CreateShape(sType);

            // Checking the identity of the shape. Other params correctness checked in shape specific tests.
            Assert.AreEqual(sType, rectangle.ShapeIdentifier);
        }

        [Test]
        public void MainShapeCreatorFactory_createEllipse_ReturnsEllipse()
        {
            ShapeType sType = ShapeType.ELLIPSE;
            MainShape ellipse = CreateShape(sType);

            Assert.AreEqual(sType, ellipse.ShapeIdentifier);
        }

        [Test]
        public void MainShapeCreatorFactory_createLine_ReturnsLine()
        {
            ShapeType sType = ShapeType.LINE;
            MainShape line = CreateShape(sType);

            Assert.AreEqual(sType, line.ShapeIdentifier);
        }

        [Test]
        public void MainShapeCreatorFactory_createPolyLine_ReturnsPolyLine()
        {
            ShapeType sType = ShapeType.POLYLINE;
            MainShape polyline = CreateShape(sType);

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
            ShapeType sType = ShapeType.NONE;

            Exception e = Assert.Throws<Exception>(delegate { CreateShape(sType); });
            Assert.That(e.Message, Is.EqualTo("Invalid Object type"));
        }


    }
}
