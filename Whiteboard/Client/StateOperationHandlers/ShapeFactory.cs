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
    /// <summary>
    /// Factory for creating mainshape.
    /// </summary>
    static class ShapeFactory
    {
        /// <summary>
        /// MainShape object to use the functionalites provided by the class.
        /// </summary>
        private static MainShape _ellipse;
        private static MainShape _rectangle;
        private static MainShape _line;
        private static MainShape _polyline;

        /// <summary>
        /// Constructor of ShapeFactory
        /// </summary>
        static ShapeFactory()
        {
            _ellipse = new Ellipse();
            _rectangle = new Rectangle();
            _line = new Line();
            _polyline = new Polyline();
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
                return _ellipse.ShapeMaker(start, end, prevShape);
            }
            else if (shapeType == ShapeType.LINE)
            {
                return _line.ShapeMaker(start, end, prevShape);
            }
            else if (shapeType == ShapeType.POLYLINE)
            {
                return _polyline.ShapeMaker(start, end, prevShape);
            }
            else
            {
                return _rectangle.ShapeMaker(start, end, prevShape);
            }
        }

    }
}
