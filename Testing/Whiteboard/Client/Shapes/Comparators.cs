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
    public static class Comparators
    {
        public static void Compare(MainShape newMainShape, Coordinate start, Coordinate center, float height, float width,
                            float strokeWidth, BoardColor strokeColor, BoardColor shapeFill, float angleOfRotation)
        {
            Assert.IsNotNull(newMainShape);
            Assert.IsTrue(newMainShape.Start.Equals(start));
            Assert.IsTrue(newMainShape.Center.Equals(center));
            Console.WriteLine("exoecetd" + height.ToString() + " got " + newMainShape.Height.ToString() + "\n");
            Console.WriteLine("exoecetd" + width.ToString() + " got " + newMainShape.Width.ToString() + "\n");
            Assert.IsTrue(Math.Abs(newMainShape.Height - height) < 0.02);
            //Assert.AreEqual(newMainShape.Height, height);
            //Assert.AreEqual(newMainShape.Width, width);

            Assert.IsTrue(Math.Abs(newMainShape.Width - width) < 0.02);
            Assert.AreEqual(newMainShape.StrokeWidth, strokeWidth);
            Assert.IsTrue(newMainShape.StrokeColor.Equals(strokeColor));
            Assert.IsTrue(newMainShape.ShapeFill.Equals(shapeFill));
            Assert.AreEqual(angleOfRotation, newMainShape.AngleOfRotation);
        }
    }
}
