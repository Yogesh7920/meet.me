/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/02/2021
**/

namespace Whiteboard
{
    internal static class ShapeFactory
    {
        private static readonly MainShape s;

        static ShapeFactory()
        {
            s = new Ellipse();
        }

        /// <summary>
        ///     Creates/modifies Shape.
        /// </summary>
        /// <param name="shapeType">Defines which shape to create.</param>
        /// <param name="start">Start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevShape">previous shape to modify.</param>
        /// <returns></returns>
        public static MainShape MainShapeCreatorFactory(ShapeType shapeType, Coordinate start, Coordinate end,
            MainShape prevShape)
        {
            if (shapeType == ShapeType.ELLIPSE)
                return s.ShapeMaker(start, end, prevShape);
            if (shapeType == ShapeType.LINE)
                return s.ShapeMaker(start, end, prevShape);
            if (shapeType == ShapeType.POLYLINE)
                return s.ShapeMaker(start, end, prevShape);
            return s.ShapeMaker(start, end, prevShape);
        }
    }
}