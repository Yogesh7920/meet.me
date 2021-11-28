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
    public static class Comparators
    {

        public const float _floatDiff = (float)0.02;

        /// <summary>
        /// Compare the params of shape provided.
        /// </summary>
        /// <param name="newMainShape">Shape to be checked</param>
        /// <param name="start">Expected start of shape.</param>
        /// <param name="center">Expected center of shape.</param>
        /// <param name="height">Expected height of the shape.</param>
        /// <param name="width">Expected width of the shape.</param>
        /// <param name="strokeWidth">Expected strokeWidth of the shape.</param>
        /// <param name="strokeColor">Expected strokecolor of the shape.</param>
        /// <param name="shapeFill">Expected shapefill of the shape.</param>
        /// <param name="angleOfRotation">Expected angle of rotation of the shape.</param>
        public static void Compare(MainShape newMainShape, Coordinate start, Coordinate center, float height, float width,
                            float strokeWidth, BoardColor strokeColor, BoardColor shapeFill, float angleOfRotation)
        {
            // comparing all the parameters inside newMainShape, with the params, and checking their correctness
            Assert.IsNotNull(newMainShape);
            Assert.IsTrue(newMainShape.Start.Equals(start));
            Assert.IsTrue(newMainShape.Center.Equals(center));

            // Compare two float numbers, direct comparison not possible because of quantisation errors.
            CompareFloats(width, newMainShape.Width);
            CompareFloats(height, newMainShape.Height);
            Assert.AreEqual(strokeWidth, newMainShape.StrokeWidth);
            Assert.IsTrue(newMainShape.StrokeColor.Equals(strokeColor));
            Assert.IsTrue(newMainShape.ShapeFill.Equals(shapeFill));

            CompareFloats(angleOfRotation, newMainShape.AngleOfRotation);
            //Assert.AreEqual(angleOfRotation, newMainShape.AngleOfRotation);
        }

        /// <summary>
        /// Comparing floats.
        /// </summary>
        /// <param name="a">Float</param>
        /// <param name="b">Float</param>
        public static void CompareFloats(float a, float b)
        {
            // Asserting that the floats are equal
            Assert.IsTrue(Math.Abs(a - b) < _floatDiff);
        }


        /// <summary>
        /// Comparing rounded floats.
        /// </summary>
        /// <param name="a">float.</param>
        /// <param name="b">float.</param>
        /// <returns></returns>
        public static bool AreDecimalEqual(float a, float b)
        {
            return (Math.Abs(Math.Round(a, 2) - Math.Round(b, 2)) < _floatDiff);
        }

        /// <summary>
        /// Checking UX shape with desired params.
        /// </summary>
        /// <param name="uxshape">Shape to be checked.</param>
        /// <param name="uXOperation">uxoperation.</param>
        /// <param name="shapetype">shapetype</param>
        /// <param name="translationCord">translation Coordinate.</param>
        /// <param name="angle">angle of rotation.</param>
        public static void CheckUXShape(UXShape uxshape, UXOperation uXOperation, ShapeType shapetype, Coordinate translationCord, float angle)
        {
            Assert.AreEqual(shapetype, uxshape.ShapeIdentifier);
            Assert.AreEqual(uXOperation, uxshape.UxOperation);
            Assert.IsTrue(uxshape.TranslationCoordinate.Equals(translationCord));
            Comparators.CompareFloats(angle, uxshape.AngleOfRotation);
        }
    }
}