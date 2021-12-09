/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Whiteboard
{
    /// <summary>
    ///     Base Class for Shapes.
    /// </summary>
    public abstract class MainShape
    {
        /// <summary>
        ///     Angle at which the shape is rotated.
        /// </summary>
        private float _angleOfRotation;

        public List<Coordinate> Points;

        /// <summary>
        ///     Constructor to initialise MainShape with a shape.
        /// </summary>
        /// <param name="s">The type of shape.</param>
        public MainShape(ShapeType s)
        {
            ShapeIdentifier = s;
            Height = 0;
            Width = 0;
            StrokeWidth = 1;
            ShapeFill = new BoardColor(255, 255, 255);
            StrokeColor = new BoardColor(0, 0, 0);
            Start = new Coordinate(0, 0);
            Center = new Coordinate(0, 0);
            Points = null;
            AngleOfRotation = 0;
        }

        /// <summary>
        ///     Constructor of Base Class.
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
            ShapeIdentifier = s;
            Height = height;
            Width = width;
            StrokeWidth = strokeWidth;
            StrokeColor = strokeColor;
            ShapeFill = shapeFill;
            Start = start;
            Center = center;
            Points = points;
            AngleOfRotation = angle;
        }

        /// <summary>
        ///     Type of shape.
        /// </summary>
        public ShapeType ShapeIdentifier { get; set; }

        /// <summary>
        ///     Height of shape.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        ///     Width of shape.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     Stroke Width of shape.
        /// </summary>
        public float StrokeWidth { get; set; }

        /// <summary>
        ///     Stroke color of shape.
        /// </summary>
        public BoardColor StrokeColor { get; set; }

        // 
        /// <summary>
        ///     Fill color of the shape.
        /// </summary>
        public BoardColor ShapeFill { get; set; }

        /// <summary>
        ///     The Coordinate of start of mouse drag while creation.
        /// </summary>
        public Coordinate Start { get; set; }

        /// <summary>
        ///     Coordinate of Center of shape.
        /// </summary>
        public Coordinate Center { get; set; }

        /// <summary>
        ///     Property setter for angle of rotation, ensuring the angle stays between -180 to 180 degrees.
        /// </summary>
        public float AngleOfRotation
        {
            get => _angleOfRotation;
            set
            {
                _angleOfRotation = value;
                while (_angleOfRotation >= 2 * Math.PI) _angleOfRotation -= (float) (2 * Math.PI);
                while (_angleOfRotation <= -2 * Math.PI) _angleOfRotation += (float) (2 * Math.PI);
                if (_angleOfRotation > Math.PI) _angleOfRotation = -(float) (2 * Math.PI) + _angleOfRotation;
                if (_angleOfRotation <= -Math.PI) _angleOfRotation = (float) (2 * Math.PI) + _angleOfRotation;
            }
        }

        /// <summary>
        ///     Add a coordinate to the list of points in the shape.
        /// </summary>
        /// <param name="c">Coordinate to add.</param>
        public void AddToList([NotNull] Coordinate c)
        {
            if (Points == null) Points = new List<Coordinate>();
            Points.Add(c);
        }

        /// <summary>
        ///     Returns the Deep Copy of the list of Points.
        /// </summary>
        /// <returns>Deep copy of the list of points.</returns>
        public List<Coordinate> GetPoints()
        {
            return Points?.ConvertAll(point => new Coordinate(point.R, point.C));
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
        /// <returns>The create shape.</returns>
        public abstract MainShape ShapeMaker(Coordinate start, Coordinate end, MainShape prevShape = null);

        /// <summary>
        ///     Performs rotation on shape.
        /// </summary>
        /// <param name="start">Start coordinate for rotation.</param>
        /// <param name="end">End coordinate for rotation.</param>
        /// <returns>True if rotation successful, else false.</returns>
        public virtual bool Rotate(Coordinate start, Coordinate end)
        {
            var v1 = start - Center;
            var v2 = end - Center;

            // finding angle of rotation from start and end coordinates.
            var rotAngle = (float) (0.01745 * Vector.AngleBetween(new Vector(v1.C, v1.R), new Vector(v2.C, v2.R)));
            AngleOfRotation += rotAngle;

            return true;
        }

        /// <summary>
        ///     Resize Operation on shape about center.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="dragPos">The Latch selected for resize operation.</param>
        /// <returns>True if resizing successful, else false.</returns>
        public virtual bool ResizeAboutCenter([NotNull] Coordinate start, [NotNull] Coordinate end, DragPos dragPos)
        {
            // Unit vector at an angle AngleOfRotation from x - axis (C axis in this case).
            Vector centerVector = new(Math.Cos(AngleOfRotation), Math.Sin(AngleOfRotation));

            // Finding displacement vector.
            var deltaCord = end - start;
            Vector deltaVector = new(deltaCord.C, deltaCord.R);

            var angleBetween = 0.01745 * Vector.AngleBetween(centerVector, deltaVector);

            // Magnitude of displacement.
            var deltaNorm = (float) Math.Sqrt(Math.Pow(deltaCord.R, 2) + Math.Pow(deltaCord.C, 2));

            // Calculating displacements in direction of unit vector
            var xDelta = (float) (deltaNorm * Math.Cos(angleBetween));
            var yDelta = (float) (deltaNorm * Math.Sin(angleBetween));

            // Changing shape attributes after resizing.
            switch (dragPos)
            {
                // the dignonal direction resizing broken into its 2 components.
                case DragPos.TopRight:
                    Height += 2 * yDelta;
                    Width += 2 * xDelta;
                    break;
                case DragPos.BottomLeft:
                    Height -= 2 * yDelta;
                    Width -= 2 * xDelta;
                    break;
                case DragPos.TopLeft:
                    Height += 2 * yDelta;
                    Width -= 2 * xDelta;
                    break;
                case DragPos.BottomRight:
                    Height -= 2 * yDelta;
                    Width += 2 * xDelta;
                    break;
                case DragPos.Left:
                    Width -= 2 * xDelta;
                    break;
                case DragPos.Right:
                    Width += 2 * xDelta;
                    break;
                case DragPos.Top:
                    Height += 2 * yDelta;
                    break;
                case DragPos.Bottom:
                    Height -= 2 * yDelta;
                    break;
                default:
                    return false;
            }

            Height = Height < BoardConstants.MinHeight ? BoardConstants.MinHeight : Height;
            Width = Width < BoardConstants.MinWidth ? BoardConstants.MinWidth : Width;
            return true;
        }
    }
}