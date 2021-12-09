/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Whiteboard
{
    /// <summary>
    ///     Polyline Class.
    /// </summary>
    public class Polyline : MainShape
    {
        /// <summary>
        ///     Constructor setting just the basic attributes of Polyline.
        /// </summary>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        public Polyline(Coordinate start) : base(ShapeType.Polyline)
        {
            Start = start;
            AddToList(Start.Clone());
        }

        /// <summary>
        ///     Constructor to create Polyline.
        /// </summary>
        /// <param name="height">Height of Polyline.</param>
        /// <param name="width">Width of Polyline.</param>
        /// <param name="strokeWidth">Stroke Width/</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Fill color of the shape.</param>
        /// <param name="start">The left bottom coordinate of the smallest rectangle enclosing the shape.</param>
        /// <param name="points">List of points, if any.</param>
        /// <param name="angle">Angle of Rotation.</param>
        public Polyline(float height,
            float width,
            float strokeWidth,
            BoardColor strokeColor,
            BoardColor shapeFill,
            Coordinate start,
            Coordinate center,
            List<Coordinate> points,
            float angle) :
            base(ShapeType.Polyline, height, width, strokeWidth, strokeColor, shapeFill, start, center, points, angle)
        {
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Polyline() : base(ShapeType.Polyline)
        {
            Points = new List<Coordinate>();
        }

        /// <summary>
        ///     Creates/ modifies the previous shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="prevPolyline">Previous Polyline object to modify.</param>
        /// <returns>Create/modified Polyline object.</returns>
        public override MainShape ShapeMaker([NotNull] Coordinate start, [NotNull] Coordinate end,
            MainShape prevPolyline = null)
        {
            // Create new shape if prevPolyLine is null.
            if (prevPolyline == null) prevPolyline = new Polyline(start.Clone());
            prevPolyline.AddToList(end.Clone());
            return prevPolyline;
        }

        /// <summary>
        ///     Creating clone object for this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            var pointClone = Points?.Select(cord => new Coordinate(cord.R, cord.C)).ToList();
            return new Polyline(Height, Width, StrokeWidth, StrokeColor.Clone(), ShapeFill.Clone(), Start.Clone(),
                Center.Clone(), pointClone, AngleOfRotation);
        }

        /// <summary>
        ///     Resizing about center for polyline.
        /// </summary>
        /// <param name="start">start of mouse drag.</param>
        /// <param name="end">end of mouse drag.</param>
        /// <param name="dragPos">The latch to resize about.</param>
        /// <returns>Success flag for this operation.</returns>
        public override bool ResizeAboutCenter([NotNull] Coordinate start, [NotNull] Coordinate end, DragPos dragPos)
        {
            return false;
        }

        /// <summary>
        ///     Rotating the shape about center.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <returns>Success of operation.</returns>
        public override bool Rotate(Coordinate start, Coordinate end)
        {
            return false;
        }
    }
}