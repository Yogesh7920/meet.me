/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 15/22/2021
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
    class EllipseTests
    {
        
        [Test]
        public void Clone__CreatesClone()
        {
            float height = 2;
            float width = 2;
            float strokeWidth = 1;
            BoardColor strokeColor = new BoardColor(34, 5, 6);
            BoardColor fillColor = new BoardColor(34, 5, 64);
            Coordinate start = new Coordinate(3, 4);
            Coordinate center = new Coordinate(4, 5);
            float angleOfRotation = (float)3.00;

            MainShape previousMainShape = new Ellipse(height, width, strokeWidth, strokeColor, fillColor, start, center, null, angleOfRotation);
            MainShape ClonedShape = previousMainShape.Clone();
            Comparators.Compare(ClonedShape, start, center, height, width, strokeWidth, strokeColor, fillColor, angleOfRotation);
            Assert.IsFalse(ReferenceEquals(previousMainShape, ClonedShape));
            Assert.IsFalse(ReferenceEquals(strokeColor, ClonedShape.StrokeColor));
            Assert.IsFalse(ReferenceEquals(fillColor, ClonedShape.ShapeFill));
            Assert.IsFalse(ReferenceEquals(start, ClonedShape.Start));
            Assert.IsFalse(ReferenceEquals(center, ClonedShape.Center));
            Assert.IsNull(ClonedShape.GetPoints());
        }
        
    }
}
