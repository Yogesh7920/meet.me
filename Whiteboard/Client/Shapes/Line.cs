/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Whiteboard
{
    /// <summary>
    ///     Line Class.
    /// </summary>
    public class Line : MainShape
    {
        /// <summary>
        ///     Constructor for Ellipse Shape.
        /// </summary>
        /// <param name="angle">Angle from C axis at which the line is.</param>
        /// <param name="width">Width of Line.</param>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        /// <param name="center">Center of the line.</param>
        public Line(float angle, float width, Coordinate start, Coordinate center) : base(ShapeType.Line)
        {
            AngleOfRotation = angle;
            Start = start;
            Center = center;
            Width = width;
        }

        /// <summary>
        ///     Constructor to create a Line.
        /// </summary>
        /// <param name="height">Height of line.</param>
        /// <param name="width">Width of line.</param>
        /// <param name="strokeWidth">Stroke Width/</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Fill color of the shape.</param>
        /// <param name="start">The left bottom coordinate of the smallest rectangle enclosing the shape.</param>
        /// <param name="points">List of points, if any.</param>
        /// <param name="angle">Angle of Rotation.</param>
        public Line(float height,
            float width,
            float strokeWidth,
            BoardColor strokeColor,
            BoardColor shapeFill,
            Coordinate start,
            Coordinate center,
            List<Coordinate> points,
            float angle) :
            base(ShapeType.Line, height, width, strokeWidth, strokeColor, shapeFill, start, center, points, angle)
        {
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Line() : base(ShapeType.Line)
        {
            Points = new List<Coordinate>();
        }

        /// <summary>
        ///     Creates/modies prevShape based on start and end coordinate of the mouse.
        /// </summary>
        /// <param name="start">The start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevLine">Previous shape to modify, if any.</param>
        /// <returns>Created/modifies Line object.</returns>
        public override MainShape ShapeMaker([NotNull] Coordinate start, [NotNull] Coordinate end,
            MainShape prevLine = null)
        {
            if (prevLine == null)
            {
                // If previous shape to modify is not provided, a new shape is created.
                var disp = end - start;
                var rotAngle = (float) (0.01745 * Vector.AngleBetween(new Vector(1, 0), new Vector(disp.C, disp.R)));
                var width = (float) Math.Sqrt(Math.Pow(disp.R, 2) + Math.Pow(disp.C, 2));
                var center = (end + start) / 2;
                return new Line(rotAngle, width, start.Clone(), center);
            }
            else
            {
                // Modification of previous shape.
                var disp = end - prevLine.Start;
                prevLine.AngleOfRotation =
                    (float) (0.01745 * Vector.AngleBetween(new Vector(1, 0), new Vector(disp.C, disp.R)));
                prevLine.Width = (float) Math.Sqrt(Math.Pow(disp.R, 2) + Math.Pow(disp.C, 2));
                prevLine.Center = (end + prevLine.Start) / 2;
                return prevLine;
            }
        }

        /// <summary>
        ///     Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            return new Line(Height, Width, StrokeWidth, StrokeColor.Clone(), ShapeFill.Clone(), Start.Clone(),
                Center.Clone(), null, AngleOfRotation);
        }

        /// <summary>
        ///     Resize Operation on shape about center.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="dragPos">The Latch selected for resize operation.</param>
        /// <returns>True if resizing successful, else false.</returns>
        public override bool ResizeAboutCenter([NotNull] Coordinate start, [NotNull] Coordinate end, DragPos dragPos)
        {
            // Unit vector at an angle AngleOfRotation from x - axis.
            Vector centerVector = new(Math.Cos(AngleOfRotation), Math.Sin(AngleOfRotation));

            // Finding displacement vector.
            var deltaCord = end - start;
            Vector deltaVector = new(deltaCord.C, deltaCord.R);

            var angleBetween = 0.01745 * Vector.AngleBetween(centerVector, deltaVector);

            // Magnitude of displacement.
            var deltaNorm = (float) Math.Sqrt(Math.Pow(deltaCord.R, 2) + Math.Pow(deltaCord.C, 2));

            // Calculating displacements in direction of unit vector
            var xDelta = (float) (deltaNorm * Math.Cos(angleBetween));

            // Changing shape attributes after resizing.
            switch (dragPos)
            {
                // the dignonal direction resizing broken into its 2 components.
                case DragPos.TopRight:
                    Width += 2 * xDelta;
                    break;
                case DragPos.BottomLeft:
                    Width -= 2 * xDelta;
                    break;
                case DragPos.TopLeft:
                    Width -= 2 * xDelta;
                    break;
                case DragPos.BottomRight:
                    Width += 2 * xDelta;
                    break;
                case DragPos.Left:
                    Width -= 2 * xDelta;
                    break;
                case DragPos.Right:
                    Width += 2 * xDelta;
                    break;
                default:
                    return false;
            }

            Width = Width < BoardConstants.MinWidth ? BoardConstants.MinWidth : Width;

            return true;
        }
    }
}