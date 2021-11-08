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
    public class Line: MainShape
    {

        /// <summary>
        /// Constructor setting just the basic attributes of Ellipse.
        /// </summary>
        /// <param name="height">Height of Line.</param>
        /// <param name="width">Width of Line.</param>
        /// <param name="start">The left botton coordinate of the smallest rectangle enclosing the shape.</param>
        public Line(int height, int width, Coordinate start, Coordinate end, Coordinate center) : base(ShapeType.LINE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start.Clone();
            this.Center = center.Clone();
            this.AddToList(start.Clone());
            this.AddToList(end.Clone());
        }

        /// <summary>
        /// Constructor to create a Line.
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
                    Coordinate center,
                    List<Coordinate> points,
                    float angle) :
                    base(ShapeType.LINE, height, width, strokeWidth, strokeColor, shapeFill, start, center, points, angle)
        {
            this.AddToList(start.Clone());
            this.AddToList(new Coordinate(start.R + height, start.C + width));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Line() : base(ShapeType.LINE)
        {
        }

        /// <summary>
        /// Creates/modies prevShape based on start and end coordinate of the mouse. 
        /// </summary>
        /// <param name="start">The start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevLine">Previous shape to modify, if any.</param>
        /// <returns>Created/modifies Line object.</returns>
        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevLine = null)
        {
            if (prevLine == null)
            {
                int height = Math.Abs(start.R - end.R);
                int width = Math.Abs(start.C - end.C);
                Coordinate center = (end - start) / 2;
                return new Line(height, width, start, end, center);
            }
            else
            {
                prevLine.Height = end.R - prevLine.Start.R;
                prevLine.Width = end.C - prevLine.Start.C;
                prevLine.Center = (end - prevLine.Start) / 2;
                PopLastElementFromList();
                AddToList(end.Clone());
                return prevLine;
            }
        }

        /// <summary>
        /// Creating clone object of this class.
        /// </summary>
        /// <returns>Clone of shape.</returns>
        public override MainShape Clone()
        {
            return new Line(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, Center, new List<Coordinate>(), AngleOfRotation);
        }
    }
}
