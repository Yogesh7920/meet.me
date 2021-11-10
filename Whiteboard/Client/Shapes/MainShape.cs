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
using System.Diagnostics;
using System.Windows;

namespace Whiteboard
{
    abstract public class MainShape
    {

        /// <summary>
        /// Constructor to initialise MainShape with a shape.
        /// </summary>
        /// <param name="s">The type of shape.</param>
        public MainShape(ShapeType s)
        {
            this.ShapeIdentifier = s;
            this.Height = 0;
            this.Width = 0;
            this.StrokeWidth = 0;
            this.ShapeFill = new BoardColor(255, 255, 255);
            this.StrokeColor = new BoardColor(0, 0, 0);
            this.Start = new Coordinate(0, 0);
            this.Center = new Coordinate(0, 0);
            this.Points = new List<Coordinate>();
            this.AngleOfRotation = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">Type of shape.</param>
        /// <param name="height">Height of the Shape.</param>
        /// <param name="width">Width of the Shape.</param>
        /// <param name="strokeWidth">Width of shape outline stroke.</param>
        /// <param name="strokeColor">Color of shape outline stroke.</param>
        /// <param name="shapeFill">Shape fill of the shape.</param>
        /// <param name="start">The left bottom coordinate of the smallest rectangle surrounding the shape.</param>
        /// <param name="points">Points in case it is a polyline or a line.</param>
        /// <param name="angle">Angle of Rotation of the Shape</param>
        public MainShape(
            ShapeType s, int height, int width, float strokeWidth, BoardColor strokeColor, BoardColor shapeFill,
            Coordinate start, Coordinate center, List<Coordinate> points, float angle)
        {
            this.ShapeIdentifier = s;
            this.Height = height;
            this.Width = width;
            this.StrokeWidth = strokeWidth;
            this.StrokeColor = strokeColor.Clone();
            this.ShapeFill = shapeFill.Clone();
            this.Start = start.Clone();
            this.Center = center.Clone();
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
        public Coordinate Center { get; set; }
        public float AngleOfRotation { get; set; }
        protected List<Coordinate> Points;

        /// <summary>
        /// Add a coordinate to the list of points in the shape.
        /// Useful for Line and Polyline.
        /// </summary>
        /// <param name="c">Coordinate.</param>
        public void AddToList(Coordinate c)
        {
            Points.Add(c);
        } 

        /// <summary>
        /// Pops and returns the last element of the list of coordinates.
        /// </summary>
        /// <returns>The last element in the list of Coordinates.</returns>
        public Coordinate PopLastElementFromList()
        {
            Coordinate lastCord = GetLast();
            int lastIndex = Points.Count()-1;
            Points.RemoveAt(lastIndex);
            return lastCord;
        }

        /// <summary>
        /// Gets the last element of the list of Coordinates.
        /// </summary>
        /// <returns>The last element of the list of Coordinates.</returns>
        public Coordinate GetLast()
        {
            int lastIndex = Points.Count() - 1;
            Coordinate c = Points.ElementAt(lastIndex);
            return c.Clone();
        }

        /// <summary>
        /// Returns the Deep Copy of the list of Points.
        /// </summary>
        /// <returns></returns>
        public List<Coordinate> GetPoints()
        {
            return Points.ConvertAll(point => new Coordinate(point.R, point.C));
        }

        /// <summary>
        /// Deep Copy for the Shape.
        /// </summary>
        /// <returns>Deep Copy for the Shape.</returns>
        abstract public MainShape Clone();

        /// <summary>
        /// Creates/modies prevShape based on start and end coordinate of the mouse. 
        /// </summary>
        /// <param name="start">The start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevShape">Previous shape to modify, if any.</param>
        /// <returns></returns>
        abstract public MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevShape = null);

        public virtual bool Rotate(Coordinate start, Coordinate end)
        {
            Coordinate v1 = start - Center;
            Coordinate v2 = end - Center;
            float rotAngle = (float)(0.01745 * Vector.AngleBetween(new Vector(v1.R, v1.C), new Vector(v2.R, v2.C)));
            AngleOfRotation += rotAngle;
            if (AngleOfRotation > Math.PI) 
            {
                AngleOfRotation -= (float)Math.PI;
            }
            if (AngleOfRotation < 0) 
            {
                AngleOfRotation = (float)(2*Math.PI -AngleOfRotation);
            }
            return true;
        }

        public virtual bool Resize(Coordinate start, Coordinate end, DragPos dragPos)
        {
            Vector centerVector = new Vector(Math.Cos(AngleOfRotation), Math.Sin(AngleOfRotation));
            Coordinate deltaCord = end - start;
            Vector deltaVector = new Vector(deltaCord.C, deltaCord.R);

            double angleBetween = Vector.AngleBetween(centerVector, deltaVector);

            int deltaNorm = (int)(Math.Sqrt(Math.Pow(deltaCord.R, 2) + Math.Pow(deltaCord.C, 2)));
            int xDelta = (int)(deltaNorm * Math.Cos(angleBetween));
            int yDelta = (int)(deltaNorm * Math.Sin(angleBetween));


            switch (dragPos)
            {
                case DragPos.TOP_RIGHT:
                    return (Resize(start, end, DragPos.TOP) || Resize(start, end, DragPos.RIGHT));
                case DragPos.BOTTOM_LEFT:
                    return (Resize(start, end, DragPos.BOTTOM) || Resize(start, end, DragPos.LEFT));
                case DragPos.TOP_LEFT:
                    return (Resize(start, end, DragPos.TOP) || Resize(start, end, DragPos.LEFT));
                case DragPos.BOTTOM_RIGHT:
                    return (Resize(start, end, DragPos.BOTTOM) || Resize(start, end, DragPos.RIGHT));
                case DragPos.LEFT:
                    Center.Add(new Coordinate((int)((-xDelta/2)*Math.Cos(angleBetween)), (int)((-xDelta/2)*Math.Sin(angleBetween))));
                    Width += xDelta;
                    break;
                case DragPos.RIGHT:
                    Center.Add(new Coordinate((int)((xDelta/2)*Math.Cos(angleBetween)), (int)((xDelta/2)*Math.Sin(angleBetween))));
                    Width += xDelta;
                    break;
                case DragPos.TOP:
                    Center.Add(new Coordinate((int)((-xDelta/2)*Math.Sin(angleBetween)),(int) ((xDelta/2)*Math.Cos(angleBetween))));
                    Height += yDelta;
                    break;
                case DragPos.BOTTOM:
                    Center.Add(new Coordinate((int)((xDelta/2)*Math.Sin(angleBetween)), (int)((-xDelta/2)*Math.Cos(angleBetween))));
                    Height += yDelta;
                    break;
                default:
                    return false;
            }
            
            return true;
        }
    }

}
