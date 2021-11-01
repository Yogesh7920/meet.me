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
    class Rectangle : MainShape
    {

        public Rectangle(int height, int width, Coordinate start) : base(ShapeType.RECTANGLE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start.Clone();
        }

        public Rectangle(int height,
                         int width,
                         float strokeWidth,
                         BoardColor strokeColor,
                         BoardColor shapeFill,
                         Coordinate start,
                         List<Coordinate> points,
                         float angle) :
                         base(ShapeType.RECTANGLE, height, width, strokeWidth, strokeColor, shapeFill, start, points, angle)
        {
        }

        public Rectangle() : base(ShapeType.RECTANGLE)
        {
        }

        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevRectangle = null)
        {
            if (prevRectangle == null)
            {
                return new Rectangle(start.R - end.R, start.C - end.C, start);
            }
            else
            {
                prevRectangle.Height = end.R - prevRectangle.Start.R;
                prevRectangle.Width = end.C - prevRectangle.Start.C;
                return prevRectangle;
            }
        }

        public override MainShape Clone()
        {
            return new Rectangle(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, new List<Coordinate>(), AngleOfRotation);
        }
    }
}
