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
    class Polyline : MainShape
    {
        public Polyline(int height, int width, Coordinate start) : base(ShapeType.POLYLINE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start.Clone();
        }

        public Polyline(int height,
                         int width,
                         float strokeWidth,
                         BoardColor strokeColor,
                         BoardColor shapeFill,
                         Coordinate start,
                         List<Coordinate> points,
                         float angle) :
                         base(ShapeType.POLYLINE, height, width, strokeWidth, strokeColor, shapeFill, start, points, angle)
        {
            this.AddToList(start.Clone());
            this.AddToList(new Coordinate(start.R+ height, start.C + width));
        }

        public Polyline() : base(ShapeType.POLYLINE)
        {
        }

        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevPolyline = null)
        {
            if (prevPolyline == null)
            {
                return new Polyline(start.R - end.R, start.C - end.C, start);
            }
            else
            {
                prevPolyline.Height = end.R - prevPolyline.Start.R;
                prevPolyline.Width = end.C - prevPolyline.Start.C;
                AddToList(end.Clone());
                return prevPolyline;
            }
        }

        public override MainShape Clone()
        {
            return new Polyline(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, new List<Coordinate>(), AngleOfRotation);
        }
    }
}
