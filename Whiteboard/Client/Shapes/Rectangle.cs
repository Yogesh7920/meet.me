/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Rectangle class.
    /// </summary>
    public class Rectangle : MainShape
    {
        /// <summary>
        /// Constructor of Polyline.
        /// </summary>
        /// <param name="height">Height.</param>
        /// <param name="width">Width.</param>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        /// <param name="center">Center of the shape.</param>
        public Rectangle(float height, float width, Coordinate start, Coordinate center) : base(ShapeType.RECTANGLE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start;
            this.Center = center;
        }

        /// <summary>
        /// Constructor to create Rectangle.
        /// </summary>
        /// <param name="height">Height.</param>
        /// <param name="width">Width.</param>
        /// <param name="strokeWidth">Stroke Width/</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Fill color of the shape.</param>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        /// <param name="points">List of points, if any.</param>
        /// <param name="angle">Angle of Rotation.</param>
        public Rectangle(float height,
                         float width,
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
        /// <param name="prevRectangle">Previous Rectangle object to modify.</param>
        /// <returns>Create/modified Rectangle object.</returns>
        public override MainShape ShapeMaker([NotNull] Coordinate start, [NotNull] Coordinate end, MainShape prevRectangle = null)
        {
            // If previous shape to modify is not provided, a new shape is created.
            if (prevRectangle == null)
            {
                float height = Math.Abs(start.R - end.R);
                float width = Math.Abs(start.C - end.C);
                Coordinate center = (end + start) / 2;
                return new Rectangle(height, width, start.Clone(), center);
            }
            // Modification of previous shape.
            else
            {
                prevRectangle.Height = Math.Abs(end.R - prevRectangle.Start.R);
                prevRectangle.Width = Math.Abs(end.C - prevRectangle.Start.C);
                prevRectangle.Center = (end + prevRectangle.Start) / 2;
                return prevRectangle;
            }
        }

        /// <summary>
        /// Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            return new Rectangle(Height, Width, StrokeWidth, StrokeColor.Clone(), ShapeFill.Clone(), Start.Clone(), Center.Clone(), null, AngleOfRotation);
        }
    }
}
