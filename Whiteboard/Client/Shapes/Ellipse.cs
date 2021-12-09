/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Whiteboard
{
    /// <summary>
    ///     Ellipse Class.
    /// </summary>
    public class Ellipse : MainShape
    {
        /// <summary>
        ///     Constructor for Ellipse.
        /// </summary>
        /// <param name="height">Height.</param>
        /// <param name="width">Width.</param>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        /// <param name="center"></param>
        public Ellipse(float height, float width, Coordinate start, Coordinate center) : base(ShapeType.Ellipse)
        {
            Height = height;
            Width = width;
            Center = center;
            Start = start;
        }

        /// <summary>
        ///     Constructor to create an ellipse.
        /// </summary>
        /// <param name="height">Height.</param>
        /// <param name="width">Width.</param>
        /// <param name="strokeWidth">Stroke Width/</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Fill color.</param>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        /// <param name="points">List of points, if any.</param>
        /// <param name="angle">Angle of Rotation.</param>
        public Ellipse(float height,
            float width,
            float strokeWidth,
            BoardColor strokeColor,
            BoardColor shapeFill,
            Coordinate start,
            Coordinate center,
            List<Coordinate> points,
            float angle) :
            base(ShapeType.Ellipse, height, width, strokeWidth, strokeColor, shapeFill, start, center, points, angle)
        {
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Ellipse() : base(ShapeType.Ellipse)
        {
        }

        /// <summary>
        ///     Creates/ modifies the previous shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="prevEllipse">Previous ellipse object to modify.</param>
        /// <returns>Create/modified Ellipse object.</returns>
        public override MainShape ShapeMaker([NotNull] Coordinate start, [NotNull] Coordinate end,
            MainShape prevEllipse = null)
        {
            // If previous shape to modify is not provided, a new shape is created.
            if (prevEllipse == null)
            {
                var height = Math.Abs(start.R - end.R);
                var width = Math.Abs(start.C - end.C);
                var center = (end + start) / 2;
                return new Ellipse(height, width, start.Clone(), center);
            }
            // Modification of previous shape.

            prevEllipse.Height = Math.Abs(end.R - prevEllipse.Start.R);
            prevEllipse.Width = Math.Abs(end.C - prevEllipse.Start.C);
            prevEllipse.Center = (end + prevEllipse.Start) / 2;
            return prevEllipse;
        }

        /// <summary>
        ///     Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of Ellipse.</returns>
        public override MainShape Clone()
        {
            return new Ellipse(Height, Width, StrokeWidth, StrokeColor.Clone(), ShapeFill.Clone(), Start.Clone(),
                Center.Clone(), null, AngleOfRotation);
        }
    }
}