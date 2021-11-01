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
    abstract public class MainShape
    {
        public MainShape(ShapeType s)
        {
            this.ShapeIdentifier = s;
            this.Height = 0;
            this.Width = 0;
            this.StrokeWidth = 0;
            this.ShapeFill = new BoardColor(255, 255, 255);
            this.StrokeColor = new BoardColor(0, 0, 0);
            this.Start = new Coordinate(0, 0);
            this.Points = new List<Coordinate>();
            this.AngleOfRotation = 0;
        }

        public MainShape(ShapeType s, int height, int width, float strokeWidth, BoardColor strokeColor, BoardColor shapeFill, Coordinate start, List<Coordinate> points, float angle)
        {
            this.ShapeIdentifier = s;
            this.Height = height;
            this.Width = width;
            this.StrokeWidth = strokeWidth;
            this.StrokeColor = strokeColor.Clone();
            this.ShapeFill = shapeFill.Clone();
            this.Start = start.Clone();
            this.Points = points.Select(cord => new Coordinate(cord.R, cord.C)).ToList();
            this.AngleOfRotation = angle;
        }

        public ShapeType ShapeIdentifier { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float StrokeWidth { get; set; }
        public BoardColor StrokeColor { get; set; }
        public BoardColor ShapeFill { get; set; }
        public Coordinate Start { get; set; }
        public float AngleOfRotation { get; set; }
        protected List<Coordinate> Points;

        public void AddToList(Coordinate c)
        {
            Points.Add(c);
        } 

        public Coordinate PopLastElementFromList()
        {
            Coordinate LastCord = GetLast();
            int LastIndex = Points.Count()-1;
            Points.RemoveAt(LastIndex);
            return LastCord;
        }

        public Coordinate GetLast()
        {
            int LastIndex = Points.Count() - 1;
            Coordinate c = Points.ElementAt(LastIndex);
            return c.Clone();
        }

        public List<Coordinate> GetPoints()
        {
            return Points.ConvertAll(point => new Coordinate(point.R, point.C));
        }

        public Coordinate GetCenter()
        {
            return new Coordinate(Start.R + Height/2, Start.C + Width/2);
        }

        abstract public MainShape Clone();

        abstract public MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevShape = null);
    }
}
