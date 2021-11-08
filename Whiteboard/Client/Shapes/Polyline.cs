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
    public class Polyline : MainShape
    {
        /// <summary>
        /// Constructor setting just the basic attributes of Polyline.
        /// </summary>
        /// <param name="height">Hright of Polyline.</param>
        /// <param name="width">Width of Polyline.</param>
        /// <param name="start">The left botton coordinate of the smallest rectangle enclosing the shape.</param>
        public Polyline(Coordinate start) : base(ShapeType.POLYLINE)
        {
            this.Start = start.Clone();
            Points.Add(Start.Clone());
        }

        /// <summary>
        /// Constructor to create Polyline.
        /// </summary>
        /// <param name="height">Height of Polyline.</param>
        /// <param name="width">Width of Polyline.</param>
        /// <param name="strokeWidth">Stroke Width/</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Fill color of the shape.</param>
        /// <param name="start">The left bottom coordinate of the smallest rectangle enclosing the shape.</param>
        /// <param name="points">List of points, if any.</param>
        /// <param name="angle">Angle of Rotation.</param>
        public Polyline(int height,
                         int width,
                         float strokeWidth,
                         BoardColor strokeColor,
                         BoardColor shapeFill,
                         Coordinate start,
                         Coordinate center,
                         List<Coordinate> points,
                         float angle) :
                         base(ShapeType.POLYLINE, height, width, strokeWidth, strokeColor, shapeFill, start, center, points, angle)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Polyline() : base(ShapeType.POLYLINE)
        {
        }

        /// <summary>
        /// Creates/ modifies the previous shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="prevPolyline">Previous Polyline object to modify.</param>
        /// <returns>Create/modified Polyline object.</returns>
        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevPolyline = null)
        {
            if (prevPolyline == null)
            {
                Polyline polyline = new Polyline(start);
                AddToList(end.Clone());
            }
            AddToList(end.Clone());
            return prevPolyline;
        }

        /// <summary>
        /// Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            return new Polyline(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, Center, Points, AngleOfRotation);
        }
    }
}
