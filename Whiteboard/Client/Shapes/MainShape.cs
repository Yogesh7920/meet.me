/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/02/2021
**/

using System.Collections.Generic;
using System.Linq;

namespace Whiteboard
{
    public abstract class MainShape
    {
        protected List<Coordinate> Points;

        /// <summary>
        ///     Constructor to initialise MainShape with a shape.
        /// </summary>
        /// <param name="s">The type of shape.</param>
        public MainShape(ShapeType s)
        {
            ShapeIdentifier = s;
            Height = 0;
            Width = 0;
            StrokeWidth = 0;
            ShapeFill = new BoardColor(255, 255, 255);
            StrokeColor = new BoardColor(0, 0, 0);
            Start = new Coordinate(0, 0);
            Points = new List<Coordinate>();
            AngleOfRotation = 0;
        }

        /// <summary>
        ///     Constructor
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
        public MainShape(ShapeType s, int height, int width, float strokeWidth, BoardColor strokeColor,
            BoardColor shapeFill, Coordinate start, List<Coordinate> points, float angle)
        {
            ShapeIdentifier = s;
            Height = height;
            Width = width;
            StrokeWidth = strokeWidth;
            StrokeColor = strokeColor.Clone();
            ShapeFill = shapeFill.Clone();
            Start = start.Clone();
            Points = points.Select(cord => new Coordinate(cord.R, cord.C)).ToList();
            AngleOfRotation = angle;
        }

        public ShapeType ShapeIdentifier { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float StrokeWidth { get; set; }
        public BoardColor StrokeColor { get; set; }
        public BoardColor ShapeFill { get; set; }
        public Coordinate Start { get; set; }
        public float AngleOfRotation { get; set; }

        /// <summary>
        ///     Add a coordinate to the list of points in the shape.
        ///     Useful for Line and Polyline.
        /// </summary>
        /// <param name="c">Coordinate.</param>
        public void AddToList(Coordinate c)
        {
            Points.Add(c);
        }

        /// <summary>
        ///     Pops and returns the last element of the list of coordinates.
        /// </summary>
        /// <returns>The last element in the list of Coordinates.</returns>
        public Coordinate PopLastElementFromList()
        {
            var lastCord = GetLast();
            var lastIndex = Points.Count() - 1;
            Points.RemoveAt(lastIndex);
            return lastCord;
        }

        /// <summary>
        ///     Gets the last element of the list of Coordinates.
        /// </summary>
        /// <returns>The last element of the list of Coordinates.</returns>
        public Coordinate GetLast()
        {
            var lastIndex = Points.Count() - 1;
            var c = Points.ElementAt(lastIndex);
            return c.Clone();
        }

        /// <summary>
        ///     Returns the Deep Copy of the list of Points.
        /// </summary>
        /// <returns></returns>
        public List<Coordinate> GetPoints()
        {
            return Points.ConvertAll(point => new Coordinate(point.R, point.C));
        }

        /// <summary>
        ///     Gets the Coordinate of the Center of the Shape.
        /// </summary>
        /// <returns></returns>
        public Coordinate GetCenter()
        {
            return new Coordinate(Start.R + Height / 2, Start.C + Width / 2);
        }

        /// <summary>
        ///     Deep Copy for the Shape.
        /// </summary>
        /// <returns>Deep Copy for the Shape.</returns>
        public abstract MainShape Clone();

        /// <summary>
        ///     Creates/modies prevShape based on start and end coordinate of the mouse.
        /// </summary>
        /// <param name="start">The start coordinate of mouse drag.</param>
        /// <param name="end">End coordinate of mouse drag.</param>
        /// <param name="prevShape">Previous shape to modify, if any.</param>
        /// <returns></returns>
        public abstract MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevShape = null);
    }
}