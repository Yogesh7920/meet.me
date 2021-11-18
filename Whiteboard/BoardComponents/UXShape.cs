/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 11/01/2021
**/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Whiteboard
{
    public class UXShape
    {
        public float AngleOfRotation;
        public int CheckPointNumber;
        public Operation OperationType;
        public ShapeType ShapeIdentifier;
        public Coordinate TranslationCoordinate;
        public UXOperation UxOperation;
        public Shape WindowsShape;

        public UXShape(UXOperation uxOperation, MainShape s, string shapeId, int checkPointNumber = 0,
            Operation operationType = Operation.NONE)
        {
            UxOperation = uxOperation;
            ShapeIdentifier = s.ShapeIdentifier;
            TranslationCoordinate = s.GetCenter().Clone();
            AngleOfRotation = s.AngleOfRotation;
            CheckPointNumber = checkPointNumber;
            OperationType = operationType;

            var shapeFillBrush = new SolidColorBrush
            {
                Color = Color.FromArgb(Convert.ToByte(s.ShapeFill.R), Convert.ToByte(s.ShapeFill.G),
                    Convert.ToByte(s.ShapeFill.B), 0)
            };

            if (s.ShapeIdentifier == ShapeType.ELLIPSE)
            {
                var EllipseUXElement = new System.Windows.Shapes.Ellipse();
                EllipseUXElement.Width = s.Width;
                EllipseUXElement.Height = s.Height;
                EllipseUXElement.Fill = shapeFillBrush;
                WindowsShape = EllipseUXElement;
            }
            else if (s.ShapeIdentifier == ShapeType.RECTANGLE)
            {
                var RectangleUXElement = new System.Windows.Shapes.Rectangle();
                RectangleUXElement.Width = s.Width;
                RectangleUXElement.Height = s.Height;
                RectangleUXElement.Fill = shapeFillBrush;
                WindowsShape = RectangleUXElement;
            }
            else if (s.ShapeIdentifier == ShapeType.LINE)
            {
                System.Windows.Shapes.Line LineUXElement = new();
                LineUXElement.X1 = s.Start.R;
                LineUXElement.Y1 = s.Start.C;
                LineUXElement.X2 = s.Start.R + s.Height;
                LineUXElement.Y2 = s.Start.R + s.Width;
                WindowsShape = LineUXElement;
            }
            else
            {
                var PolylineUXElement = new System.Windows.Shapes.Polyline();
                var PolyLinePointCollection = new PointCollection();
                foreach (var cord in s.GetPoints()) PolyLinePointCollection.Add(new Point(cord.R, cord.C));
                PolylineUXElement.Points = PolyLinePointCollection;
                WindowsShape = PolylineUXElement;
            }

            WindowsShape.StrokeThickness = s.StrokeWidth;

            var StrokeBrush = new SolidColorBrush
            {
                Color = Color.FromArgb(Convert.ToByte(s.StrokeColor.R), Convert.ToByte(s.StrokeColor.G),
                    Convert.ToByte(s.StrokeColor.B), 0)
            };

            WindowsShape.Stroke = StrokeBrush;
            if (shapeId != null) WindowsShape.Uid = shapeId;
        }

        public UXShape()
        {
        }
    }
}