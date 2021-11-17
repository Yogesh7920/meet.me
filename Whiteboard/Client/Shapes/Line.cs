/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/02/2021
**/


using System.Collections.Generic;

namespace Whiteboard
{
    public class Line : MainShape
    {
        /// <summary>
        ///     Constructor setting just the basic attributes of Ellipse.
        /// </summary>
        /// <param name="height">Height of Line.</param>
        /// <param name="width">Width of Line.</param>
        /// <param name="start">The left botton coordinate of the smallest rectangle enclosing the shape.</param>
        public Line(int height, int width, Coordinate start) : base(ShapeType.LINE)
        {
            Height = height;
            Width = width;
            Start = start.Clone();
            AddToList(start.Clone());
            AddToList(new Coordinate(start.R + height, start.C + width));
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
        public Line(int height,
            int width,
            float strokeWidth,
            BoardColor strokeColor,
            BoardColor shapeFill,
            Coordinate start,
            List<Coordinate> points,
            float angle) :
            base(ShapeType.LINE, height, width, strokeWidth, strokeColor, shapeFill, start, points, angle)
        {
            AddToList(start.Clone());
            AddToList(new Coordinate(start.R + height, start.C + width));
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Line() : base(ShapeType.LINE)
        {
        }

        /// <summary>
        ///     Creates/modies prevShape based on start and end coordinate of the mouse.
        /// </summary>
        /// <param name="start">The start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevLine">Previous shape to modify, if any.</param>
        /// <returns>Created/modifies Line object.</returns>
        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevLine = null)
        {
            if (prevLine == null)
            {
                return new Polyline(start.R - end.R, start.C - end.C, start);
            }

            prevLine.Height = end.R - prevLine.Start.R;
            prevLine.Width = end.C - prevLine.Start.C;
            PopLastElementFromList();
            AddToList(end.Clone());
            return prevLine;
        }

        /// <summary>
        ///     Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            return new Line(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, new List<Coordinate>(),
                AngleOfRotation);
        }
    }
}