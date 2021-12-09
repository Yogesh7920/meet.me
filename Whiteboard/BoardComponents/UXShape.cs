/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Whiteboard
{
    public class UXShape
    {
        /// <summary>
        ///     Angle by which the shape is rotated.
        /// </summary>
        public float AngleOfRotation;

        /// <summary>
        ///     Count of checkpoints saved on server.
        /// </summary>
        public int CheckPointNumber;

        /// <summary>
        ///     Operation to be performed on the state.
        /// </summary>
        public Operation OperationType;

        /// <summary>
        ///     Denotes what shape it is.
        /// </summary>
        public ShapeType ShapeIdentifier;

        /// <summary>
        ///     Center of this shape on canvas plane.
        /// </summary>
        public Coordinate TranslationCoordinate;

        /// <summary>
        ///     Operation to be performed by UX.
        /// </summary>
        public UXOperation UxOperation;

        /// <summary>
        ///     Windows shape to be rendered.
        /// </summary>
        public Shape WindowsShape;

        /// <summary>
        ///     Constructor for UXShape.
        /// </summary>
        /// <param name="uxOperation">The UXOperation to be performed by UX.</param>
        /// <param name="s">MainShape to be converted into UXShape.</param>
        /// <param name="shapeId">Id to be given to the shape.</param>
        /// <param name="checkPointNumber">The count of checkpoints.</param>
        /// <param name="operationType">The operation performed on state.</param>
        public UXShape([NotNull] UXOperation uxOperation, [NotNull] MainShape s, string shapeId = null,
            int checkPointNumber = 0, Operation operationType = Operation.None)
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
                Color = Color.FromArgb(255, Convert.ToByte(s.ShapeFill.R), Convert.ToByte(s.ShapeFill.G),
                    Convert.ToByte(s.ShapeFill.B))
            };
            // setting paramaters based on shape
            if (s.ShapeIdentifier == ShapeType.Ellipse)
            {
                System.Windows.Shapes.Ellipse EllipseUXElement = new()
                {
                    Width = s.Width,
                    Height = s.Height,
                    Fill = shapeFillBrush
                };
                WindowsShape = EllipseUXElement;
            }
            else if (s.ShapeIdentifier == ShapeType.Rectangle)
            {
                System.Windows.Shapes.Rectangle RectangleUXElement = new()
                {
                    Width = s.Width,
                    Height = s.Height,
                    Fill = shapeFillBrush
                };
                WindowsShape = RectangleUXElement;
            }
            else if (s.ShapeIdentifier == ShapeType.Line)
            {
                TranslationCoordinate = new Coordinate(0, 0);
                System.Windows.Shapes.Line LineUXElement = new()
                {
                    Y1 = s.Center.R,
                    X1 = s.Center.C - s.Width / 2,
                    Y2 = s.Center.R,
                    X2 = s.Center.C + s.Width / 2
                };

                WindowsShape = LineUXElement;
            }
            else
            {
                TranslationCoordinate = new Coordinate(0, 0);
                AngleOfRotation = 0;
                System.Windows.Shapes.Polyline PolylineUXElement = new();
                PointCollection PolyLinePointCollection = new();
                foreach (var cord in s.GetPoints()) PolyLinePointCollection.Add(new Point(cord.R, cord.C));
                PolylineUXElement.Points = PolyLinePointCollection;
                WindowsShape = PolylineUXElement;
            }

            WindowsShape.StrokeThickness = s.StrokeWidth;

            SolidColorBrush StrokeBrush = new()
            {
                Color = Color.FromArgb(255, Convert.ToByte(s.StrokeColor.R), Convert.ToByte(s.StrokeColor.G),
                    Convert.ToByte(s.StrokeColor.B))
            };

            WindowsShape.Stroke = StrokeBrush;

            // Reassigning the shape id to windows shape.
            if (shapeId != null)
                WindowsShape.Uid = shapeId;
            else
                WindowsShape.Uid = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Constructor for UXShape for FetchCheckpoint operation.
        /// </summary>
        /// <param name="checkpointNumber">Count of checkpoints.</param>
        /// <param name="operationFlag">FetchCheckpoint</param>
        public UXShape(int checkpointNumber, Operation operationFlag = Operation.FetchCheckpoint)
        {
            CheckPointNumber = checkpointNumber;
            OperationType = operationFlag;
            UxOperation = UXOperation.None;
            WindowsShape = null;
            ShapeIdentifier = ShapeType.None;
            TranslationCoordinate = null;
            AngleOfRotation = 0;
        }

        /// <summary>
        ///     Public default constructor.
        /// </summary>
        public UXShape()
        {
        }

        /// <summary>
        ///     Convert a single UXShapeHelper to UXShape.
        /// </summary>
        /// <param name="uXShapeHelper">The helper used to create.</param>
        /// <returns>Returns UXShape</returns>
        public static UXShape ToUXShape(UXShapeHelper uXShapeHelper)
        {
            return uXShapeHelper.MainShapeDefiner == null
                ? new UXShape(uXShapeHelper.CheckpointNumber, uXShapeHelper.OperationType)
                : new UXShape(uXShapeHelper.UxOperation, uXShapeHelper.MainShapeDefiner, uXShapeHelper.ShapeId,
                    uXShapeHelper.CheckpointNumber, uXShapeHelper.OperationType);
        }

        /// <summary>
        ///     Overloaded method to convert to list of UXShapes.
        /// </summary>
        /// <param name="uXShapeHelpers">List of helpers to create.</param>
        /// <returns>Returns list of UXShape</returns>
        public static List<UXShape> ToUXShape(List<UXShapeHelper> uXShapeHelpers)
        {
            List<UXShape> uXShapes = new();
            for (var i = 0; i < uXShapeHelpers.Count; i++) uXShapes.Add(ToUXShape(uXShapeHelpers[i]));
            return uXShapes;
        }
    }
}