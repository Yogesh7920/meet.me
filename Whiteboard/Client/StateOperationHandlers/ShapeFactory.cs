/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/02/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    static class ShapeFactory
    {
        private static MainShape s;

        static ShapeFactory()
        {
            s = new Ellipse();
        }

        /// <summary>
        /// Creates/modifies Shape.
        /// </summary>
        /// <param name="shapeType">Defines which shape to create.</param>
        /// <param name="start">Start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevShape">previous shape to modify.</param>
        /// <returns></returns>
        public static MainShape MainShapeCreatorFactory(ShapeType shapeType, Coordinate start, Coordinate end, MainShape prevShape)
        {

            if (shapeType == ShapeType.ELLIPSE)
            {
                return s.ShapeMaker(start, end, prevShape);
            }
            else if (shapeType == ShapeType.LINE)
            {
                return s.ShapeMaker(start, end, prevShape);
            }
            else if (shapeType == ShapeType.POLYLINE)
            {
                return s.ShapeMaker(start, end, prevShape);
            }
            else
            {
                return s.ShapeMaker(start, end, prevShape);
            }
        }

    }
}
