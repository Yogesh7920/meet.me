/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 11/12/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;

namespace Whiteboard
{
    public class UXShape
    {
        /// <summary>
        /// Operation to be performed by UX.
        /// </summary>
        public UXOperation UxOperation;

        /// <summary>
        /// Windows shape to be rendered.
        /// </summary>
        public Shape WindowsShape;

        /// <summary>
        /// Denotes what shape it is.
        /// </summary>
        public ShapeType ShapeIdentifier;

        /// <summary>
        /// Center of this shape on canvas plane.
        /// </summary>
        public Coordinate TranslationCoordinate;

        /// <summary>
        /// Angle by which the shape is rotated.
        /// </summary>
        public float AngleOfRotation;

        /// <summary>
        /// Count of checkpoints saved on server.
        /// </summary>
        public int CheckPointNumber;

        // Operation performed on the state.
        public Operation OperationType;

        /// <summary>
        /// Constructor for UXShape.
        /// </summary>
        /// <param name="uxOperation">The UXOperation to be performed by UX.</param>
        /// <param name="s">MainShape to be converted into UXShape.</param>
        /// <param name="shapeId">Id to be given to the shape.</param>
        /// <param name="checkPointNumber">The count of checkpoints.</param>
        /// <param name="operationType">The operation performed on state.</param>
        public UXShape([NotNull] UXOperation uxOperation, [NotNull] MainShape s, string shapeId = null, int checkPointNumber = 0, Operation operationType = Operation.NONE)
        {
            // setting params of UXShape
            UxOperation = uxOperation;
            ShapeIdentifier = s.ShapeIdentifier;
            TranslationCoordinate = s.Center.Clone();
            AngleOfRotation = s.AngleOfRotation;
            CheckPointNumber = checkPointNumber;
            OperationType = operationType;

            SolidColorBrush shapeFillBrush = new()
            {
                Color = Color.FromArgb(Convert.ToByte(s.ShapeFill.R), Convert.ToByte(s.ShapeFill.G), Convert.ToByte(s.ShapeFill.B), 0)
            };

            // setting paramaters based on shape
            if (s.ShapeIdentifier == ShapeType.ELLIPSE)
            {
                System.Windows.Shapes.Ellipse EllipseUXElement = new()
                {
                    Width = s.Width,
                    Height = s.Height,
                    Fill = shapeFillBrush
                };
                WindowsShape = EllipseUXElement;

            }
            else if (s.ShapeIdentifier == ShapeType.RECTANGLE)
            {
                System.Windows.Shapes.Rectangle RectangleUXElement = new()
                {
                    Width = s.Width,
                    Height = s.Height,
                    Fill = shapeFillBrush
                };
                WindowsShape = RectangleUXElement;
            }
            else if (s.ShapeIdentifier == ShapeType.LINE)
            {
                System.Windows.Shapes.Line LineUXElement = new()
                {
                    X1 = s.Start.R,
                    Y1 = s.Start.C,
                    X2 = s.Start.R + s.Height,
                    Y2 = s.Start.R + s.Width
                };
                
                WindowsShape = LineUXElement;
            }
            else
            {
                System.Windows.Shapes.Polyline PolylineUXElement = new();
                PointCollection PolyLinePointCollection = new();
                foreach (Coordinate cord in s.GetPoints())
                {
                    PolyLinePointCollection.Add(new System.Windows.Point(cord.R, cord.C));
                }
                PolylineUXElement.Points = PolyLinePointCollection;
                WindowsShape = PolylineUXElement;
            }
            WindowsShape.StrokeThickness = s.StrokeWidth;

            SolidColorBrush StrokeBrush = new()
            {
                Color = Color.FromArgb(Convert.ToByte(s.StrokeColor.R), Convert.ToByte(s.StrokeColor.G), Convert.ToByte(s.StrokeColor.B), 0)
            };

            WindowsShape.Stroke = StrokeBrush;

            // Reassigning the shape id to windows shape.
            if (shapeId != null)
            {
                WindowsShape.Uid = shapeId;
            }
            
        }

        /// <summary>
        /// Constructor for UXShape for FETCH_CHECKPOINT operation.
        /// </summary>
        /// <param name="checkpointNumber">Count of checkpoints.</param>
        /// <param name="operationFlag">FETCH_CHECKPOINT</param>
        public UXShape(int checkpointNumber, Operation operationFlag = Operation.FETCH_CHECKPOINT)
        {
            CheckPointNumber = checkpointNumber;
            OperationType = operationFlag;
            UxOperation = UXOperation.NONE;
            WindowsShape = null;
            ShapeIdentifier = ShapeType.NONE;
            TranslationCoordinate = null;
            AngleOfRotation = 0;
        }

        /// <summary>
        /// Public default constructor.
        /// </summary>
        public UXShape()
        {
        }

    }
}
