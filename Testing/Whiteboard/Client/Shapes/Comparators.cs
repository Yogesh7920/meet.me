/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/23/2021
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

        public static float _floatDiff = (float)0.2;

        public static void Compare(MainShape newMainShape, Coordinate start, Coordinate center, float height, float width,
                            float strokeWidth, BoardColor strokeColor, BoardColor shapeFill, float angleOfRotation)
        {
            // comparing all the parameters inside newMainShape, with the params, and checking their correctness
            Assert.IsNotNull(newMainShape);
            Assert.IsTrue(newMainShape.Start.Equals(start));
            Assert.IsTrue(newMainShape.Center.Equals(center));

            // Compare two float numbers, direct comparison not possible because of quantisation errors.
            CompareFloats(width, newMainShape.Width);
            CompareFloats(width, newMainShape.Width);
            Assert.AreEqual(strokeWidth, newMainShape.StrokeWidth);
            Assert.IsTrue(newMainShape.StrokeColor.Equals(strokeColor));
            Assert.IsTrue(newMainShape.ShapeFill.Equals(shapeFill));
            Assert.AreEqual(angleOfRotation, newMainShape.AngleOfRotation);
        }

        public static void CompareFloats(float a, float b)
        {
            // Asserting that the floats are equal
            Assert.IsTrue(Math.Abs(a - b) < _floatDiff);
        }
    }
}
