/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/01/2021
**/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    class Ellipse : MainShape
    {

        public Ellipse(int height, int width, Coordinate start) : base(ShapeType.ELLIPSE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start.Clone();
        }

        public Ellipse(int height, 
                       int width, 
                       float strokeWidth, 
                       BoardColor strokeColor, 
                       BoardColor shapeFill, 
                       Coordinate start,
                       List<Coordinate> points,
                       float angle) :
                       base(ShapeType.ELLIPSE, height, width, strokeWidth, strokeColor, shapeFill, start, points, angle)
        {
        }

        public Ellipse() : base(ShapeType.ELLIPSE)
        {
        }

        public override MainShape ShapeMaker (Coordinate start, Coordinate end, MainShape prevEllipse = null)
        {
            if (prevEllipse == null)
            {
                return new Ellipse(start.R - end.R, start.C - end.C, start);
            }
            else
            {
                prevEllipse.Height = end.R - prevEllipse.Start.R;
                prevEllipse.Width = end.C - prevEllipse.Start.C;
                return prevEllipse;
            }
        }

        public override MainShape Clone()
        {
            return new Ellipse(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, new List<Coordinate> (), AngleOfRotation);
        }

    }
}
