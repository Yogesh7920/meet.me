/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/12/2021
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
    /// <summary>
    /// Base Class for Shapes.
    /// </summary>
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
        /// Constructor of Base Class.
        /// </summary>
        /// <param name="s">Type of shape.</param>
        /// <param name="height">Height.</param>
        /// <param name="width">Width.</param>
        /// <param name="strokeWidth">Stroke Width.</param>
        /// <param name="strokeColor">Stroke Color.</param>
        /// <param name="shapeFill">Shape fill.</param>
        /// <param name="start">The Coordinate of start of mouse drag while creation.</param>
        /// <param name="points">Points in case it is a polyline or a line.</param>
        /// <param name="angle">Angle of Rotation of the Shape.</param>
        public MainShape(
            ShapeType s, float height, float width, float strokeWidth, BoardColor strokeColor, BoardColor shapeFill,
            Coordinate start, Coordinate center, List<Coordinate> points, float angle)
        {
            this.ShapeIdentifier = s;
            this.Height = height;
            this.Width = width;
            this.StrokeWidth = strokeWidth;
            this.StrokeColor = strokeColor;
            this.ShapeFill = shapeFill;
            this.Start = start;
            this.Center = center;
            this.Points = points;
            this.AngleOfRotation = angle;
        }

        // Type of shape.
        public ShapeType ShapeIdentifier { get; set; }

        // Height of shape.
        public float Height { get; set; }

        // Width of shape.
        public float Width { get; set; }

        // Stroke Width of shape.
        public float StrokeWidth { get; set; }

        // Stroke color of shape.
        public BoardColor StrokeColor { get; set; }

        // Shape fill of the shape.
        public BoardColor ShapeFill { get; set; }

        // The Coordinate of start of mouse drag while creation.
        public Coordinate Start { get; set; }

        // Coordinate of Center of shape.
        public Coordinate Center { get; set; }

        // Angle at which the shape is rotated.
        public float AngleOfRotation { get; set; }
        protected List<Coordinate> Points;

        /// <summary>
        /// Add a coordinate to the list of points in the shape.
        /// </summary>
        /// <param name="c">Coordinate to add.</param>
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

        /// <summary>
        /// Performs rotation on shape.
        /// </summary>
        /// <param name="start">Start coordinate for rotation.</param>
        /// <param name="end">End coordinate for rotation.</param>
        /// <returns>True if rotation successful, else false.</returns>
        public virtual bool Rotate(Coordinate start, Coordinate end)
        {
            Coordinate v1 = start - Center;
            Coordinate v2 = end - Center;

            // finding angle of rotation from start and end coordinates.
            float rotAngle = (float)(0.01745 * Vector.AngleBetween(new Vector(v1.R, v1.C), new Vector(v2.R, v2.C)));
            AngleOfRotation += rotAngle;

            // Keeping the angle of rotation in range -pi to pi
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

        /// <summary>
        /// Performs resize operation on the shape.
        /// </summary>
        /// <param name="start">Start coordinate for Resize.</param>
        /// <param name="end">End coordinate for Resize.</param>
        /// <param name="dragPos"></param>
        /// <returns>Success of resizing operation.</returns>
        public virtual bool Resize(Coordinate start, Coordinate end, DragPos dragPos)
        {
            // Unit vector at an angle AngleOfRotation from x-axis.
            Vector centerVector = new(Math.Cos(AngleOfRotation), Math.Sin(AngleOfRotation));

            // Finding displacement vector.
            Coordinate deltaCord = end - start;            
            Vector deltaVector = new(deltaCord.C, deltaCord.R);

            double angleBetween = Math.Abs(Vector.AngleBetween(centerVector, deltaVector));

            // Magnitude of displacement.
            float deltaNorm = (float)(Math.Sqrt(Math.Pow(deltaCord.R, 2) + Math.Pow(deltaCord.C, 2)));

            // Calculating displacements in direction of unit vector
            float xDelta = (float)(deltaNorm * Math.Cos(angleBetween));
            float yDelta = (int)(deltaNorm * Math.Sin(angleBetween));

            // Changing shape attributes after resizing.
            switch (dragPos)
            {
                // the dignonal direction resizing broken into its 2 components.
                case DragPos.TOP_RIGHT:
                    return (Resize(start, end, DragPos.TOP) || Resize(start, end, DragPos.RIGHT));
                case DragPos.BOTTOM_LEFT:
                    return (Resize(start, end, DragPos.BOTTOM) || Resize(start, end, DragPos.LEFT));
                case DragPos.TOP_LEFT:
                    return (Resize(start, end, DragPos.TOP) || Resize(start, end, DragPos.LEFT));
                case DragPos.BOTTOM_RIGHT:
                    return (Resize(start, end, DragPos.BOTTOM) || Resize(start, end, DragPos.RIGHT));
                case DragPos.LEFT:
                    Center.Add(new Coordinate((float)((-xDelta/2)*Math.Cos(AngleOfRotation)), (float)((-xDelta/2)*Math.Sin(AngleOfRotation))));
                    Width += xDelta;
                    break;
                case DragPos.RIGHT:
                    Center.Add(new Coordinate((float)((xDelta/2)*Math.Cos(AngleOfRotation)), (float)((xDelta/2)*Math.Sin(AngleOfRotation))));
                    Width += xDelta;
                    break;
                case DragPos.TOP:
                    Center.Add(new Coordinate((float)((-xDelta/2)*Math.Sin(AngleOfRotation)),(float) ((xDelta/2)*Math.Cos(AngleOfRotation))));
                    Height += yDelta;
                    break;
                case DragPos.BOTTOM:
                    Center.Add(new Coordinate((float)((xDelta/2)*Math.Sin(AngleOfRotation)), (float)((-xDelta/2)*Math.Cos(AngleOfRotation))));
                    Height += yDelta;
                    break;
                default:
                    return false;
            } 
            return true;
        }
    }

}
