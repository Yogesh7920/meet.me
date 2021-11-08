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
    public class Rectangle : MainShape
    {
        /// <summary>
        /// Constructor setting just the basic attributes of Polyline.
        /// </summary>
        /// <param name="height">Hright of Rectangle.</param>
        /// <param name="width">Width of Rectangle.</param>
        /// <param name="start">The left botton coordinate of the smallest rectangle enclosing the shape.</param>
        public Rectangle(int height, int width, Coordinate start, Coordinate center) : base(ShapeType.RECTANGLE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start.Clone();
            this.Center = center.Clone();
        }

        /// <summary>
        /// Constructor to create Rectangle.
        /// </summary>
        /// <param name="height">Height of Rectangle.</param>
        /// <param name="width">Width of Rectangle.</param>
        /// <param name="strokeWidth">Stroke Width/</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Fill color of the shape.</param>
        /// <param name="start">The left bottom coordinate of the smallest rectangle enclosing the shape.</param>
        /// <param name="points">List of points, if any.</param>
        /// <param name="angle">Angle of Rotation.</param>
        public Rectangle(int height,
                         int width,
                         float strokeWidth,
                         BoardColor strokeColor,
                         BoardColor shapeFill,
                         Coordinate start,
                         Coordinate center,
                         List<Coordinate> points,
                         float angle) :
                         base(ShapeType.RECTANGLE, height, width, strokeWidth, strokeColor, shapeFill, start, center, points, angle)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Rectangle() : base(ShapeType.RECTANGLE)
        {
        }

        /// <summary>
        /// Creates/ modifies the previous shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="prevPolyline">Previous Rectangle object to modify.</param>
        /// <returns>Create/modified Rectangle object.</returns>
        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevRectangle = null)
        {
            if (prevRectangle == null)
            {
                int height = Math.Abs(start.R - end.R);
                int width = Math.Abs(start.C - end.C);
                Coordinate center = (end - start) / 2;
                return new Rectangle(height, width, start, center);
            }
            else
            {
                prevRectangle.Height = Math.Abs(end.R - prevRectangle.Start.R);
                prevRectangle.Width = Math.Abs(end.C - prevRectangle.Start.C);
                Coordinate center = (end - prevRectangle.Start) / 2;
                return prevRectangle;
            }
        }

        /// <summary>
        /// Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            return new Rectangle(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, Center, new List<Coordinate>(), AngleOfRotation);
        }
    }
}
