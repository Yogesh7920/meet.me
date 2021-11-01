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
    class Line: MainShape
    {
        public Line(int height, int width, Coordinate start) : base(ShapeType.LINE)
        {
            this.Height = height;
            this.Width = width;
            this.Start = start.Clone();
            this.AddToList(start.Clone());
            this.AddToList(new Coordinate(start.R + height, start.C + width));
        }

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
            this.AddToList(start.Clone());
            this.AddToList(new Coordinate(start.R + height, start.C + width));
        }

        public Line() : base(ShapeType.LINE)
        {
        }

        public override MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevLine = null)
        {
            if (prevLine == null)
            {
                return new Polyline(start.R - end.R, start.C - end.C, start);
            }
            else
            {
                prevLine.Height = end.R - prevLine.Start.R;
                prevLine.Width = end.C - prevLine.Start.C;
                PopLastElementFromList();
                AddToList(end.Clone());
                return prevLine;
            }
        }

        public override MainShape Clone()
        {
            return new Line(Height, Width, StrokeWidth, StrokeColor, ShapeFill, Start, new List<Coordinate>(), AngleOfRotation);
        }
    }
}
