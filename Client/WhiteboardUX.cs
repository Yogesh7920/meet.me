/**
 * Owned By: Aniket Singh Rajpoot
 * Created By: Aniket Singh Rajpoot
 * Date Created: 25/10/2021
 * Date Modified: 28/11/2021
**/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Whiteboard;
using Ellipse = System.Windows.Shapes.Ellipse;
using Line = System.Windows.Shapes.Line;
using Polyline = System.Windows.Shapes.Polyline;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Client
{
    /// Enum used to pass the info about the dragged corner of the shape in case of Resize operation
    public enum AdornerDragPos
    {
        TopLeft,
        BotRight,
        TopRight,
        BotLeft
    }

    /// <summary>
    ///     Interface which listens to fetched server updates by IWhiteBoardState and local updates by IShapeOperation
    /// </summary>
    internal interface IWhiteBoardUpdater
    {
        /// <summary>
        ///     Fetch updates from IWhiteBoardState for rendering in the view
        /// </summary>
        void processServerUpdateBatch(List<UXShapeHelper> ServerUpdates);
    }

    public class BorderAdorner : Adorner
    {
        private readonly UIElement adornedShape;
        private readonly Thumb bottomLeft;
        private readonly Thumb bottomRight;
        private readonly Canvas cn;
        private readonly ShapeManager shapeManager;

        //use thumb for resizing elements
        private readonly Thumb topLeft;
        private readonly Thumb topRight;

        //visual child collection for adorner
        private readonly VisualCollection visualChilderns;

        private readonly IWhiteBoardOperationHandler WbOp;

        private Point dragStart, dragEnd, permissibleDragEnd;
        private AdornerDragPos lastDraggedCorner;

        private bool testing;

        public BorderAdorner(UIElement element, ShapeManager shapeManager, Canvas cn, IWhiteBoardOperationHandler WbOp,
            bool testing = false) : base(element)
        {
            visualChilderns = new VisualCollection(this);
            adornedShape = element as Shape;
            this.shapeManager = shapeManager;
            this.cn = cn;
            this.WbOp = WbOp;

            dragStart = new Point {X = 0, Y = 0};
            dragEnd = new Point {X = 0, Y = 0};
            permissibleDragEnd = new Point {X = 0, Y = 0};
            this.testing = testing;

            if (element is not Line)
            {
                //adding thumbs for drawing adorner rectangle and setting cursor
                BuildAdornerCorners(ref topLeft, Cursors.SizeNWSE);
                BuildAdornerCorners(ref bottomRight, Cursors.SizeNWSE);
                BuildAdornerCorners(ref topRight, Cursors.No);
                BuildAdornerCorners(ref bottomLeft, Cursors.No);
            }
            else
            {
                //adding thumbs for drawing adorner rectangle and setting cursor
                BuildAdornerCorners(ref topLeft, Cursors.SizeNWSE);
                BuildAdornerCorners(ref bottomRight, Cursors.SizeNWSE);
            }

            //registering drag delta events for thumb drag movement
            /*
            topLeft.PreviewMouseLeftButtonUp += Adorner_MouseUp;
            topLeft.PreviewMouseLeftButtonDown += Adorner_MouseDown;

            
            bottomRight.PreviewMouseLeftButtonUp += Adorner_MouseUp;
            bottomRight.PreviewMouseLeftButtonDown += Adorner_MouseDown;*/

            topLeft.PreviewMouseLeftButtonDown += Adorner_MouseDown;
            bottomRight.PreviewMouseLeftButtonDown += Adorner_MouseDown;

            topLeft.DragDelta += TopLeft_DragDelta;
            bottomRight.DragDelta += BottomRight_DragDelta;

            topLeft.DragCompleted += Adorner_MouseUp;
            bottomRight.DragCompleted += Adorner_MouseUp;
        }

        protected override int VisualChildrenCount => visualChilderns.Count;

        private void Adorner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(cn);
            dragEnd = e.GetPosition(cn);
        }

        private void Adorner_MouseUp(object sender, DragCompletedEventArgs e)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var corner = sender as Thumb;

            /*if (corner.Equals(topLeft)) MessageBox.Show("Start = " + dragStart.ToString() + " ,End = " + dragEnd.ToString() + " on topLeft");
            else if (corner.Equals(bottomRight)) MessageBox.Show("Start = " + dragStart.ToString() + " ,End = " + dragEnd.ToString() + " on bottomRight");

            Transform rt = adornedElement.RenderTransform;

            var mat = new Matrix();
            //mat.Rotate(rt.Angle);
            mat.Translate(Canvas.GetLeft(adornedElement) + adornedElement.Width / 2, Canvas.GetTop(adornedElement) + adornedElement.Height / 2);
            mat.Invert();

            Point transDragStart = mat.Transform(dragStart);
            Point transDragEnd = mat.Transform(dragEnd);
            switch (lastDraggedCorner)
            {
                case AdornerDragPos.BotRight:
                    this.permissibleDragEnd = e.GetPosition(bottomRight);
                    break;
                case AdornerDragPos.TopLeft:
                    this.permissibleDragEnd = e.GetPosition(topLeft);
                    break;
            }

            transDragStart = rt.Inverse.Transform(transDragStart);
            transDragEnd = rt.Inverse.Transform(transDragEnd);

            transDragStart.X = (int)transDragStart.X;
            transDragStart.Y = (int)transDragStart.Y;
            transDragEnd.X = (int)transDragEnd.X;
            transDragEnd.Y = (int)transDragEnd.Y;

            if (corner.Equals(topLeft)) MessageBox.Show("Transformed Start = " + transDragStart.ToString() + " ,End = " + transDragEnd.ToString() + " on topLeft");
            else if (corner.Equals(bottomRight)) MessageBox.Show("Transformed Start = " + transDragStart.ToString() + " ,End = " + transDragEnd.ToString() + " on bottomRight");
            */

            if (corner.Equals(topLeft))
                shapeManager.ResizeShape(cn, WbOp, (Shape) adornedElement, dragStart, dragEnd, AdornerDragPos.TopLeft);
            else if (corner.Equals(bottomRight))
                shapeManager.ResizeShape(cn, WbOp, (Shape) adornedElement, dragStart, dragEnd, AdornerDragPos.BotRight);
        }

        private void BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            dragEnd.X = dragStart.X + e.HorizontalChange;
            dragEnd.Y = dragStart.Y + e.VerticalChange;

            var adornedElement = AdornedElement as FrameworkElement;
            var bottomRightCorner = sender as Thumb;
            //setting new height and width after drag
            if (adornedElement != null && bottomRightCorner != null)
            {
                EnforceSize(adornedElement);
                //this.shapeManager.ResizeAdorner(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, bottomRightCorner, AdornerDragPos.BotRight);
                lastDraggedCorner = AdornerDragPos.BotRight;
            }
        }

        private void TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            dragEnd.X = dragStart.X + e.HorizontalChange;
            dragEnd.Y = dragStart.Y + e.VerticalChange;

            var adornedElement = AdornedElement as FrameworkElement;
            var topLeftCorner = sender as Thumb;
            //setting new height, width and canvas top, left after drag
            if (adornedElement != null && topLeftCorner != null)
            {
                EnforceSize(adornedElement);
                //this.shapeManager.ResizeAdorner(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, topLeftCorner, AdornerDragPos.TopLeft);
                lastDraggedCorner = AdornerDragPos.TopLeft;
            }
        }

        public void BuildAdornerCorners(ref Thumb cornerThumb, Cursor customizedCursors)
        {
            //adding new thumbs for adorner to visual childern collection
            if (cornerThumb != null) return;
            cornerThumb = new Thumb
            {
                Cursor = customizedCursors, Height = 10, Width = 10, Opacity = 0.5,
                Background = new SolidColorBrush(Colors.Purple)
            };
            visualChilderns.Add(cornerThumb);
        }

        public void EnforceSize(FrameworkElement element)
        {
            if (element.Width.Equals(double.NaN))
                element.Width = element.DesiredSize.Width;
            if (element.Height.Equals(double.NaN))
                element.Height = element.DesiredSize.Height;

            //enforce size of element not exceeding to it's parent element size
            var parent = element.Parent as FrameworkElement;

            if (parent != null)
            {
                element.MaxHeight = parent.ActualHeight;
                element.MaxWidth = parent.ActualWidth;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);

            var desireWidth = AdornedElement.DesiredSize.Width;
            var desireHeight = AdornedElement.DesiredSize.Height;

            var adornerWidth = DesiredSize.Width;
            var adornerHeight = DesiredSize.Height;

            if (adornedShape is not Line)
            {
                //arranging thumbs
                topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                topRight.Arrange(new Rect(desireWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth,
                    adornerHeight));
                bottomLeft.Arrange(new Rect(-adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth,
                    adornerHeight));
                bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2,
                    adornerWidth, adornerHeight));
            }
            else
            {
                //arranging thumbs
                topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2,
                    adornerWidth, adornerHeight));
            }

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChilderns[index];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }

    /// <summary>
    ///     Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module
    /// </summary>
    public class ShapeManager
    {
        //THIS IS SUPPOSED TO BE A UNIQUE UID THAT IS NOT USED BY THE CLIENT MODULE
        //for assigning temporary UID to the shape being created
        private readonly int counter = 0;
        private readonly bool testing;
        public AdornerLayer adornerLayer;

        public List<string> selectedShapes = new();
        public Point selectMouseDownPos;
        public Point selectMouseStuck;

        //Variable to keep track of the Uid of the new shape that is currently under creation
        private string uidShapeCreate;
        public Shape underCreation;

        public ShapeManager(bool testing = false)
        {
            this.testing = testing;
        }

        public Canvas CreateSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp)
        {
            adornerLayer = AdornerLayer.GetAdornerLayer(sh);
            var adr = new BorderAdorner(sh, this, cn, WBOp, testing);

            if (!testing)
            {
                adr.IsClipEnabled = true;
                adornerLayer.Add(adr);
            }

            return cn;
        }


        public Canvas DeleteSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp)
        {
            adornerLayer.Remove(adornerLayer.GetAdorners(sh)[0]);
            return cn;
        }

        public Canvas UnselectAllBB(Canvas cn, IWhiteBoardOperationHandler WBOp)
        {
            foreach (var item in selectedShapes)
            {
                var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);

                //Check Condition 
                var cnt = iterat.Count();
                //if (testing) Debug.Assert(cnt == 1);
                var sh = iterat.ToList()[0] as Shape;
                if (!testing) cn = DeleteSelectionBB(cn, sh, WBOp);
            }

            selectedShapes.Clear();

            return cn;
        }

        /// <summary>
        ///     Deletes Selected Shapes : Used for Testing purposes only
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> void, upon altering the 'selectedShapes' of this class instane accordingly </returns>
        public Canvas DeleteSelectedShapes(Canvas cn, IWhiteBoardOperationHandler WBOp)
        {
            //remove shapes 
            foreach (var item in selectedShapes)
            {
                var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);

                //Check Condition 
                //if(testing) Debug.Assert(iterat.Count() == 1);

                var sh = iterat.ToList()[0] as Shape;
                cn.Children.Remove(sh);
            }

            selectedShapes.Clear();
            return cn;
        }

        /// <summary>
        ///     Handle input events for selection : this includes evnents for single shape selection and multiple shape selection
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="sh"> System.Windows.Shape instance to be selected </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="mode"> mode=0 if shape selected without Ctrl pressed, else mode=1 </param>
        /// <returns> void, upon altering the 'selectedShapes' of this class instane accordingly </returns>
        public Canvas SelectShape(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp, int mode = 0)
        {
            switch (mode)
            {
                //selectedShapes.ToString()
                //single shape selection case
                case 0:
                    Trace.WriteLine("Selection of shape with Uid = " + sh.Uid +
                                    "requested by user with Ctrl NOT pressed");

                    //If selected shape is already selected or we select a different shape  
                    if (selectedShapes.Count > 0)
                    {
                        if (selectedShapes.Contains(sh.Uid))
                        {
                            cn = UnselectAllBB(cn, WBOp);
                        }
                        else
                        {
                            cn = UnselectAllBB(cn, WBOp);
                            selectedShapes.Add(sh.Uid);
                            cn = CreateSelectionBB(cn, sh, WBOp);
                        }
                    }
                    else
                    {
                        selectedShapes.Add(sh.Uid);
                        cn = CreateSelectionBB(cn, sh, WBOp);
                    }

                    break;
                //multiple shape selection case
                case 1:
                    Trace.WriteLine("Selection of shape with Uid = " + sh.Uid + "requested by user with Ctrl pressed");
                    if (selectedShapes.Contains(sh.Uid))
                    {
                        cn = DeleteSelectionBB(cn, sh, WBOp);
                        selectedShapes.Remove(sh.Uid);
                    }
                    else
                    {
                        selectedShapes.Add(sh.Uid);
                        cn = CreateSelectionBB(cn, sh, WBOp);
                    }

                    break;
            }

            Trace.WriteLine("List of Uids of selected shapes at the end of Client.ShapeManager.SelectShape is: " +
                            selectedShapes);
            return cn;
        }


        /// <summary>
        ///     Handles Shape creation requests by the user
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="strokeWidth">
        ///     Float determining the thickness of border of drawn shape (OR) thickness of the stroke in
        ///     freehand drawing
        /// </param>
        /// <param name="strokeColor"> Hex code representing the color of the drawn shape </param>
        /// <param name="shapeId">
        ///     Attribute to recursively keep track of the drawn shape visually by the user, initialised as null
        ///     and equal to the UID assigned by the WB module for the remaining iterations
        /// </param>
        /// <param name="shapeComp">
        ///     Attribute to keep track of temporary/final operations of Client in order to send only the
        ///     final queries to the Server by the WB module
        /// </param>
        /// <returns> Final Canvas instance with the newly rendered/deleted shapes </returns>
        public Canvas CreateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, WhiteBoardViewModel.WBTools activeTool,
            Point strt, Point end, float strokeWidth = 1, string fillColor = "#FFFFFF", bool shapeComp = false)
        {
            //List of server render request, used only when shapeComp is true
            List<UXShape> toRender;

            //Brush for Fill 
            var strokeColorBrush = (SolidColorBrush) new BrushConverter().ConvertFrom(fillColor);

            //Brush With Opacity 
            var strokeOpacityBrush = new SolidColorBrush(Colors.Aqua);
            strokeOpacityBrush.Opacity = .25d;

            //Brush for Border 
            var blackBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#000000");

            var strk_clr = new BoardColor(strokeColorBrush.Color.R, strokeColorBrush.Color.G, strokeColorBrush.Color.B);

            //if (end.X < 0 || end.Y < 0 || end.X > cn.Width || end.Y > cn.Height) MessageBox.Show("Cursor went out of screen");

            switch (activeTool)
            {
                case WhiteBoardViewModel.WBTools.NewLine:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of a line with start = " + strt + "end = " + end);
                        //toRender = WBOps.CreateLine(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp); //return is of form List of UXShape
                        //cn = this.RenderUXElement(toRender, cn);

                        //TEMP: Logic to simulate continuous deletion & addition while drawing new shapes
                        if (uidShapeCreate == null)
                        {
                            underCreation = new Line();

                            ((Line) underCreation).X1 = 0;
                            ((Line) underCreation).X2 = end.X - strt.X;
                            ((Line) underCreation).Y1 = 0;
                            ((Line) underCreation).Y2 = end.Y - strt.Y;

                            underCreation.Stroke = blackBrush;
                            underCreation.StrokeThickness = 1;

                            Canvas.SetLeft(underCreation, strt.X);
                            Canvas.SetTop(underCreation, strt.Y);

                            underCreation.Uid = counter.ToString();
                            uidShapeCreate = counter.ToString();
                            cn.Children.Add(underCreation);
                        }
                        else
                        {
                            //Updating the underCreation Shape Accordingly 
                            ((Line) underCreation).X1 = 0;
                            ((Line) underCreation).X2 = end.X - strt.X;
                            ((Line) underCreation).Y1 = 0;
                            ((Line) underCreation).Y2 = end.Y - strt.Y;

                            Canvas.SetLeft(underCreation, strt.X);
                            Canvas.SetTop(underCreation, strt.Y);
                        }
                    }

                    break;
                case WhiteBoardViewModel.WBTools.NewRectangle:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of a rectangle with start = " + strt + "end = " + end);
                        //toRender = WBOps.CreateRectangle(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        //cn = this.RenderUXElement(toRender, cn);

                        if (uidShapeCreate == null)
                        {
                            underCreation = new Rectangle();

                            underCreation.Stroke = blackBrush;
                            underCreation.Fill = strokeColorBrush;
                            underCreation.StrokeThickness = 1;

                            var x = Math.Min(end.X, strt.X);
                            var y = Math.Min(end.Y, strt.Y);

                            var w = Math.Max(end.X, strt.X) - x;
                            var h = Math.Max(end.Y, strt.Y) - y;

                            underCreation.Width = w;
                            underCreation.Height = h;

                            Canvas.SetLeft(underCreation, x);
                            Canvas.SetTop(underCreation, y);

                            //set the uid of shape 
                            underCreation.Uid = counter.ToString();
                            uidShapeCreate = counter.ToString();

                            //Add to canvas
                            cn.Children.Add(underCreation);
                        }
                        else
                        {
                            //Updating the underCreation Shape Accordingly 
                            var x = Math.Min(end.X, strt.X);
                            var y = Math.Min(end.Y, strt.Y);

                            var w = Math.Max(end.X, strt.X) - x;
                            var h = Math.Max(end.Y, strt.Y) - y;

                            underCreation.Width = w;
                            underCreation.Height = h;

                            Canvas.SetLeft(underCreation, x);
                            Canvas.SetTop(underCreation, y);
                        }
                    }

                    break;
                case WhiteBoardViewModel.WBTools.NewEllipse:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of an ellipse with start = " + strt + "end = " + end);
                        //toRender = WBOps.CreateEllipse(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        //cn = this.RenderUXElement(toRender, cn);

                        if (uidShapeCreate == null)
                        {
                            underCreation = new Ellipse();

                            underCreation.Stroke = blackBrush;
                            underCreation.Fill = strokeColorBrush;
                            underCreation.StrokeThickness = 1;

                            var x = Math.Min(end.X, strt.X);
                            var y = Math.Min(end.Y, strt.Y);

                            var w = Math.Max(end.X, strt.X) - x;
                            var h = Math.Max(end.Y, strt.Y) - y;

                            underCreation.Width = w;
                            underCreation.Height = h;

                            Canvas.SetLeft(underCreation, x);
                            Canvas.SetTop(underCreation, y);

                            //set the uid of shape 
                            underCreation.Uid = counter.ToString();
                            uidShapeCreate = counter.ToString();

                            //Add to canvas
                            cn.Children.Add(underCreation);
                        }
                        else
                        {
                            //Updating the underCreation Shape Accordingly 
                            var x = Math.Min(end.X, strt.X);
                            var y = Math.Min(end.Y, strt.Y);

                            var w = Math.Max(end.X, strt.X) - x;
                            var h = Math.Max(end.Y, strt.Y) - y;

                            underCreation.Width = w;
                            underCreation.Height = h;

                            Canvas.SetLeft(underCreation, x);
                            Canvas.SetTop(underCreation, y);
                        }
                    }

                    break;
            }

            if (shapeComp)
            {
                if (testing)
                {
                    switch (activeTool)
                    {
                        case WhiteBoardViewModel.WBTools.NewLine:
                            lock (this)
                            {
                                Trace.WriteLine("User requested creation of a line with start = " + strt + "end = " +
                                                end);

                                //Updating the underCreation Shape Accordingly 
                                ((Line) underCreation).X1 = 0;
                                ((Line) underCreation).X2 = end.X - strt.X;
                                ((Line) underCreation).Y1 = 0;
                                ((Line) underCreation).Y2 = end.Y - strt.Y;

                                Canvas.SetLeft(underCreation, strt.X);
                                Canvas.SetTop(underCreation, strt.Y);
                            }

                            break;
                        case WhiteBoardViewModel.WBTools.NewRectangle:
                            lock (this)
                            {
                                Trace.WriteLine("User requested creation of a rectangle with start = " + strt +
                                                "end = " + end);

                                var x = Math.Min(end.X, strt.X);
                                var y = Math.Min(end.Y, strt.Y);

                                var w = Math.Max(end.X, strt.X) - x;
                                var h = Math.Max(end.Y, strt.Y) - y;

                                underCreation.Width = w;
                                underCreation.Height = h;

                                Canvas.SetLeft(underCreation, x);
                                Canvas.SetTop(underCreation, y);
                            }

                            break;
                        case WhiteBoardViewModel.WBTools.NewEllipse:
                            lock (this)
                            {
                                Trace.WriteLine("User requested creation of an ellipse with start = " + strt +
                                                "end = " + end);

                                var x = Math.Min(end.X, strt.X);
                                var y = Math.Min(end.Y, strt.Y);

                                var w = Math.Max(end.X, strt.X) - x;
                                var h = Math.Max(end.Y, strt.Y) - y;

                                underCreation.Width = w;
                                underCreation.Height = h;

                                Canvas.SetLeft(underCreation, x);
                                Canvas.SetTop(underCreation, y);
                            }

                            break;
                    }

                    //select the created shape
                    SelectShape(cn, underCreation, WBOps);
                }
                else
                {
                    underCreation.StrokeThickness = 2;
                    uidShapeCreate = null;

                    //Coordinate C_strt = new Coordinate((float)strt.X, (float)strt.Y);
                    //Coordinate C_end = new Coordinate((float)end.X, (float)end.Y);

                    var C_strt = new Coordinate((int) (cn.Height - strt.Y), (int) strt.X);
                    var C_end = new Coordinate((int) (cn.Height - end.Y), (int) end.X);

                    var stroke = new BoardColor(blackBrush.Color.R, blackBrush.Color.G, blackBrush.Color.B);

                    //SERVER REQUEST TO CREATE FINAL SHAPE
                    toRender = new List<UXShape>();
                    switch (activeTool)
                    {
                        case WhiteBoardViewModel.WBTools.NewLine:
                            toRender = WBOps.CreateLine(C_strt, C_end, 2, stroke, null, true);
                            break;
                        case WhiteBoardViewModel.WBTools.NewEllipse:
                            toRender = WBOps.CreateEllipse(C_strt, C_end, 2, stroke, null, true);
                            //setting the rendered-to-be shape fill according to the Canvas background
                            break;
                        case WhiteBoardViewModel.WBTools.NewRectangle:
                            toRender = WBOps.CreateRectangle(C_strt, C_end, 2, stroke, null, true);
                            //setting the rendered-to-be shape fill according to the Canvas background
                            break;
                    }

                    //Removing temporary render from Canvas as Whiteboard module sends only CREATE operation, so we need to clean up temporary render
                    cn.Children.Remove(underCreation);

                    if (!(toRender == null || toRender.Count() == 0))
                    {
                        cn = RenderUXElement(toRender, cn, WBOps);

                        //select the shape 
                        SelectShape(cn, toRender.ElementAt(0).WindowsShape, WBOps);
                    }
                }
            }

            return cn;
        }


        /// <summary>
        ///     Translate the shape according to input events
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="shapeComp">
        ///     Attribute to keep track of temporary/final operations of Client in order to send only the
        ///     final queries to the Server by the WB module
        /// </param>
        /// <returns> The updated Canvas </returns>
        public Canvas MoveShape(Canvas cn, IWhiteBoardOperationHandler WBOps, Point strt, Point end, Shape mouseDownSh,
            bool shapeComp)
        {
            if (mouseDownSh == null) return cn;

            if (mouseDownSh != null && !selectedShapes.Contains(mouseDownSh.Uid))
            {
                /*if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    
                }*/
                cn = UnselectAllBB(cn, WBOps);
                cn = SelectShape(cn, mouseDownSh, WBOps, 1);
            }

            Trace.WriteLine("Beginning moving shape with Uid" + mouseDownSh.Uid + "from start point" + strt +
                            "to end point " + end);
            Trace.WriteLine("List of Uids of selected shapes affected by move:" + selectedShapes);

            List<UXShape> toRender;

            if (shapeComp != true || testing)
            {
                //if(!testing) Debug.Assert(selectedShapes.Count == 1);
                var shUID = selectedShapes[0];

                /* Temporary WB Module code to test functionality */
                var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                //Check Condition 
                //if (!testing) Debug.Assert(iterat.Count() == 1);

                var sh = (Shape) cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

                var topleft_x = Canvas.GetLeft(iterat.ToList()[0]);
                var topleft_y = Canvas.GetTop(iterat.ToList()[0]);

                //MessageBox.Show("Entered MoveShape event");
                //MessageBox.Show(topleft_x.ToString(), topleft_y.ToString());

                var diff_topleft_x = strt.X - end.X;
                var diff_topleft_y = strt.Y - end.Y;
                double center_x, center_y;

                if (sh is not Line)
                {
                    center_x = topleft_x - diff_topleft_x + sh.Width / 2;
                    center_y = topleft_y - diff_topleft_y + sh.Height / 2;


                    if (center_x > 0 && center_x < cn.Width)
                    {
                        selectMouseStuck.X = end.X;
                        Canvas.SetLeft(sh, topleft_x - diff_topleft_x);
                    }
                    else
                    {
                        Canvas.SetLeft(sh, Canvas.GetLeft(sh));
                    }

                    if (center_y > 0 && center_y < cn.Height)
                    {
                        selectMouseStuck.Y = end.Y;
                        Canvas.SetTop(sh, topleft_y - diff_topleft_y);
                    }
                    else
                    {
                        Canvas.SetTop(sh, Canvas.GetTop(sh));
                    }
                }
                else
                {
                    center_x = Canvas.GetLeft(sh) - diff_topleft_x + +((Line) sh).X2 / 2;
                    center_y = Canvas.GetTop(sh) - diff_topleft_y + ((Line) sh).Y2 / 2;

                    if (center_x > 0 && center_x < cn.Width) Canvas.SetLeft(sh, topleft_x - diff_topleft_x);
                    else Canvas.SetLeft(sh, Canvas.GetLeft(sh));

                    if (center_y > 0 && center_y < cn.Height) Canvas.SetTop(sh, topleft_y - diff_topleft_y);
                    else Canvas.SetTop(sh, Canvas.GetTop(sh));
                }

                //else if (center_x > cn.Width) Canvas.SetLeft(newEl, Canvas.GetLeft(sh) - 2);
                //else Canvas.SetLeft(newEl, Canvas.GetLeft(sh) + 2);
            }
            else
            {
                //if(!testing) Debug.Assert(selectedShapes.Count == 1);
                var shUID = selectedShapes[0];

                /* Temporary WB Module code to test functionality */
                var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                //Check Condition 
                //if (!testing) Debug.Assert(iterat.Count() == 1);

                var sh = (Shape) cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

                var topleft_x = Canvas.GetLeft(iterat.ToList()[0]);
                var topleft_y = Canvas.GetTop(iterat.ToList()[0]);

                //MessageBox.Show("Entered MoveShape event");
                //MessageBox.Show(topleft_x.ToString(), topleft_y.ToString());

                var diff_topleft_x = selectMouseStuck.X - end.X;
                var diff_topleft_y = selectMouseStuck.Y - end.Y;
                double center_x, center_y;

                if (sh is not Line)
                {
                    center_x = topleft_x - diff_topleft_x + sh.Width / 2;
                    center_y = topleft_y - diff_topleft_y + sh.Height / 2;


                    if (center_x > 0 && center_x < cn.Width) selectMouseStuck.X = end.X;

                    if (center_y > 0 && center_y < cn.Height) selectMouseStuck.Y = end.Y;
                }
                else
                {
                    center_x = Canvas.GetLeft(sh) - diff_topleft_x + +((Line) sh).X2 / 2;
                    center_y = Canvas.GetTop(sh) - diff_topleft_y + ((Line) sh).Y2 / 2;

                    if (center_x > 0 && center_x < cn.Width) selectMouseStuck.X = end.X;

                    if (center_y > 0 && center_y < cn.Height) selectMouseStuck.Y = end.Y;
                }


                var C_strt = new Coordinate((int) (cn.Height - selectMouseDownPos.Y), (int) selectMouseDownPos.X);
                //Coordinate C_end = new Coordinate(((int)(cn.Height - end.Y)), ((int)end.X));
                var C_end = new Coordinate((int) (cn.Height - selectMouseStuck.Y), (int) selectMouseStuck.X);


                toRender = new List<UXShape>();
                toRender = WBOps.TranslateShape(C_strt, C_end, mouseDownSh.Uid, true);

                //removing the local temporary render and only acknowledging the CREATE UXShape request as we cleaned up temporary render
                cn = UnselectAllBB(cn, WBOps);
                //cn.Children.Remove(sh);
                //Since we are removing rendered temporary shape above and toRender[1] corresponds to CREATE operation
                cn = RenderUXElement(toRender, cn, WBOps);
                if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps);

                //Bugged, adr.ClipEnabled gives NullException??
                //cn = SelectShape(cn, toRender[0].WindowsShape, WBOps);

                Trace.WriteLine("Sent move request to the client for the shape with Uid:" + mouseDownSh.Uid +
                                "from start point" + strt +
                                "to end point " + end + ", where list of Uids of selected shapes are:" +
                                selectedShapes + "with shapeComp = ", shapeComp.ToString());
            }


            return cn;
        }

        /// <summary>
        ///     Rotate the selected shape by input degrees
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="shapeComp">
        ///     Attribute to keep track of temporary/final operations of Client in order to send only the
        ///     final queries to the Server by the WB module
        /// </param>
        /// <returns> The updated Canvas </returns>
        public Canvas RotateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, Point strt, Point end,
            Shape mouseDownSh, bool shapeComp)
        {
            if (mouseDownSh == null) return cn;

            if (mouseDownSh != null && !selectedShapes.Contains(mouseDownSh.Uid))
            {
                /*if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    
                }*/
                cn = UnselectAllBB(cn, WBOps);
                cn = SelectShape(cn, mouseDownSh, WBOps, 1);
            }

            Trace.WriteLine("Beginning rotating shape with Uid" + mouseDownSh.Uid + "from start point" + strt +
                            "to end point " + end);
            Trace.WriteLine("List of Uids of selected shapes affected by rotate:" + selectedShapes);

            List<UXShape> toRender;

            //UNCOMMENT LATER
            /*lock (this)
            {
                toRender = WBOps.RotateShape(C_strt, C_end, shpUID, shapeComp);
                cn = this.RenderUXElement(toRender, cn);
            }*/
            /* Temporary WB Module code to test functionality */
            var shUID = selectedShapes[0];

            var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);
            var sh = (Shape) cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

            //Check Condition 
            //if (!testing) Debug.Assert(iterat.Count() == 1);

            if (sh is Line)
            {
                var lin = (Line) sh;
                var topleft_x = (int) Canvas.GetLeft(lin);
                var topleft_y = (int) Canvas.GetTop(lin);
                var center_x = (int) (topleft_x + lin.X2 / 2);
                var center_y = (int) (topleft_y + lin.Y2 / 2);

                var strt_shifted = new Point(strt.X - center_x, strt.Y - center_y);
                var end_shifted = new Point(end.X - center_x, end.Y - center_y);

                var radians_strt = Math.Atan2(strt_shifted.Y, strt_shifted.X);
                var angle_strt = radians_strt * (180 / Math.PI);

                var radians_end = Math.Atan2(end_shifted.Y, end_shifted.X);
                var angle_end = radians_end * (180 / Math.PI);

                var rt_prev = (RotateTransform) sh.RenderTransform;

                var ang = (float) (angle_end - angle_strt);
                /*Code to find the angle made by start & end point on the center of the shape*/

                var rotateTransform = new RotateTransform
                {
                    Angle = ang + (float) rt_prev.Angle,
                    CenterX = lin.X2 / 2, //topleft_x,
                    CenterY = lin.Y2 / 2 //topleft_y
                };
                sh.RenderTransform = rotateTransform;
                //sh = (Shape)lin;
            }
            else
            {
                /*Code to find the angle made by start & end point on the center of the shape*/
                var topleft_x = (int) Canvas.GetLeft(sh);
                var topleft_y = (int) Canvas.GetTop(sh);
                var center_x = (int) (topleft_x + sh.Width / 2);
                var center_y = (int) (topleft_y + sh.Height / 2);

                var strt_shifted = new Point(strt.X - center_x, strt.Y - center_y);
                var end_shifted = new Point(end.X - center_x, end.Y - center_y);

                var radians_strt = Math.Atan2(strt_shifted.Y, strt_shifted.X);
                var angle_strt = radians_strt * (180 / Math.PI);

                var radians_end = Math.Atan2(end_shifted.Y, end_shifted.X);
                var angle_end = radians_end * (180 / Math.PI);


                var rt_prev = (RotateTransform) sh.RenderTransform;

                var ang = (float) (angle_end - angle_strt);
                /*Code to find the angle made by start & end point on the center of the shape*/

                var rotateTransform = new RotateTransform
                {
                    Angle = ang + (float) rt_prev.Angle,
                    CenterX = sh.Width / 2, //topleft_x,
                    CenterY = sh.Height / 2 //topleft_y
                };
                sh.RenderTransform = rotateTransform;
                //MessageBox.Show(ang.ToString(), ((int)rt_prev.Angle).ToString());
                /* Temporary WB Module code to test functionality */
            }

            //Necessary step to synchronize borders on rotation of selected shapes
            //cn = SyncBorders(cn, WBOps, sh);


            if (shapeComp)
            {
                cn = UnselectAllBB(cn, WBOps);
                //cn.Children.Remove(sh);

                var C_strt = new Coordinate((int) (cn.Height - selectMouseDownPos.Y), (int) selectMouseDownPos.X);
                var C_end = new Coordinate((int) (cn.Height - end.Y), (int) end.X);

                toRender = new List<UXShape>();
                toRender = WBOps.RotateShape(C_strt, C_end, mouseDownSh.Uid, true);

                //Since we already removed our side of temporary render, DELETE operation by WB module is not acknowledged, whereas toRender[1] refers to necessary CREATE operation with the updated shape
                cn = RenderUXElement(toRender, cn, WBOps);
                if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps);

                //Bugged as adr.isClipEnabled gives Null Exception
                //cn = SelectShape(cn, toRender[0].WindowsShape, WBOps);

                Trace.WriteLine("Sent rotate request to the client for the shape with Uid:" + mouseDownSh.Uid +
                                "from start point" + strt +
                                "to end point " + end + ", where the list of Uids of selected shapes are:" +
                                selectedShapes + "with shapeComp = ", shapeComp.ToString());
            }


            return cn;
        }

        /// <summary>
        ///     Duplicate the selected shape by input degrees
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the duplicated shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="strokeWidth">
        ///     Float determining the thickness of border of drawn shape (OR) thickness of the stroke in
        ///     freehand drawing
        /// </param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// <param name="offs_x"> Float determining the fill color of the drawn shape </param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// <returns> The Updated Canvas </returns>
        public Canvas DuplicateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, int offs_x = 10, int offs_y = 10)
        {
            Point strt, end;

            if (selectedShapes == null || selectedShapes.Count == 0) return cn;

            var shUID = selectedShapes[0];

            var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);
            var sh = (Shape) cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

            //Check Condition 
            //if (!testing) Debug.Assert(iterat.Count() == 1);

            cn = UnselectAllBB(cn, WBOps);

            switch (sh)
            {
                case Line:

                    strt.X = Canvas.GetLeft(sh) + 20;
                    strt.Y = Canvas.GetTop(sh);

                    end.X = Canvas.GetLeft(sh) + ((Line) sh).X2 + 20;
                    end.Y = Canvas.GetTop(sh) + ((Line) sh).Y2;

                    cn = CreateShape(cn, WBOps, WhiteBoardViewModel.WBTools.NewLine, strt, end, shapeComp: true);
                    break;
                case Rectangle:
                    strt.X = Canvas.GetLeft(sh) + offs_x;
                    strt.Y = Canvas.GetTop(sh) + offs_y;

                    end.X = Canvas.GetLeft(sh) + sh.Width + offs_x;
                    end.Y = Canvas.GetTop(sh) + sh.Height + offs_y;

                    cn = CreateShape(cn, WBOps, WhiteBoardViewModel.WBTools.NewRectangle, strt, end, shapeComp: true);
                    break;
                case Ellipse:
                    strt.X = Canvas.GetLeft(sh) + offs_x;
                    strt.Y = Canvas.GetTop(sh) + offs_y;

                    end.X = Canvas.GetLeft(sh) + sh.Width + offs_x;
                    end.Y = Canvas.GetTop(sh) + sh.Height + offs_y;

                    cn = CreateShape(cn, WBOps, WhiteBoardViewModel.WBTools.NewEllipse, strt, end, shapeComp: true);
                    break;
            }

            return cn;
        }

        /// <summary>
        ///     Render fetched shape updates on canvas
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            if (shps == null || shps.Count() == 0) return cn;

            //UXShape has attribute
            foreach (var shp in shps)
                switch (shp.UxOperation)
                {
                    case UXOperation.CREATE:

                        //Convert Radians from WB to Degrres for Render Transform 
                        var degrees = (int) (shp.AngleOfRotation * (180 / Math.PI));

                        //Setting the rendering such that bottom-right and top-left adorners adjust appropriately
                        if (degrees > 90) degrees -= 180;
                        else if (degrees < -90) degrees += 180;
                        shp.AngleOfRotation = degrees;

                        shp.AngleOfRotation = -1 * degrees;

                        if (shp.WindowsShape is not Line)
                        {
                            Canvas.SetLeft(shp.WindowsShape, shp.TranslationCoordinate.C - shp.WindowsShape.Width / 2);
                            Canvas.SetTop(shp.WindowsShape,
                                cn.Height - shp.TranslationCoordinate.R - shp.WindowsShape.Height / 2);

                            //Since WB module stores the System.Windows.Shape.Width as height & System.Windows.Shape.Height as width?
                            /*double temp = shp.WindowsShape.Height;
                            shp.WindowsShape.Height = shp.WindowsShape.Width;
                            shp.WindowsShape.Width = temp;*/

                            //Setting the angular orientation of bounding box to be same as updated shape
                            var rotateTransform = new RotateTransform
                            {
                                Angle = shp.AngleOfRotation,
                                CenterX = shp.WindowsShape.Width / 2, //topleft_x,
                                CenterY = shp.WindowsShape.Height / 2 //topleft_y
                            };
                            shp.WindowsShape.RenderTransform = rotateTransform;
                        }
                        else
                        {
                            var recv_Line = (Line) shp.WindowsShape;

                            Shape ParsedLineUXElement = new Line();
                            ((Line) ParsedLineUXElement).X1 = 0;
                            ((Line) ParsedLineUXElement).Y1 = 0;
                            ((Line) ParsedLineUXElement).X2 = recv_Line.X2 - recv_Line.X1;
                            ((Line) ParsedLineUXElement).Y2 = recv_Line.Y1 - recv_Line.Y2;
                            //((System.Windows.Shapes.Line)ParsedLineUXElement).RenderTransform = rotateTransform;
                            ParsedLineUXElement.Stroke = recv_Line.Stroke;
                            ParsedLineUXElement.StrokeThickness = recv_Line.StrokeThickness;
                            ParsedLineUXElement.Uid = recv_Line.Uid;
                            //Height = recv_Line.Width,
                            //Width = recv_Line.Height,

                            //Setting the angular orientation of bounding box to be same as updated shape
                            var rotateTransform = new RotateTransform
                            {
                                Angle = shp.AngleOfRotation,
                                CenterX = ((Line) ParsedLineUXElement).X2 / 2, //topleft_x,
                                CenterY = ((Line) ParsedLineUXElement).Y2 / 2 //topleft_y
                            };
                            ((Line) ParsedLineUXElement).RenderTransform = rotateTransform;

                            var transf_Line = (Line) ParsedLineUXElement;

                            shp.WindowsShape = ParsedLineUXElement;

                            //NOT WORKING
                            //shp.WindowsShape.RenderTransform = rotateTransform;
                            Canvas.SetLeft(shp.WindowsShape, recv_Line.X1);
                            Canvas.SetTop(shp.WindowsShape, cn.Height - recv_Line.Y1);
                        }

                        shp.WindowsShape.ToolTip = "User : " + getUserId(shp.WindowsShape, WBOps);
                        cn.Children.Add(shp.WindowsShape);
                        break;
                    case UXOperation.DELETE:

                        var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shp.WindowsShape.Uid);

                        //Check Condition that the shape to be deleted actually exists within the Canvas and has unique Uid
                        //if (!testing) Debug.Assert(iterat.Count() == 1);

                        cn.Children.Remove(iterat.ToList()[0]);
                        break;
                }

            return cn;
        }

        /// <summary>
        ///     Rotate the selected shape by input degrees
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> The updated Canvas </returns>
        public Canvas DeleteShape(Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            Trace.WriteLine("List of Uids of selected shapes that are supposed to be deleted:",
                selectedShapes.ToString());
            List<UXShape> toRender;
            foreach (var shp in selectedShapes)
                lock (this)
                {
                    toRender = WBOps.DeleteShape(shp);
                    cn = RenderUXElement(toRender, cn, WBOps);
                }

            selectedShapes.Clear();

            Trace.WriteLine("Sent delete requests to the Client for the selected shapes with Uids:",
                selectedShapes.ToString());
            return cn;
        }

        /// <summary>
        ///     Adjusts the finer attributes of selected shape, like Fill color, border thickness, border color etc
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="property"> Property of the selected shape that is to be changed, represented by string </param>
        /// <param name="hexCode"> Hexcode of the new fill/border color, used only if property = "Stroke" or "Fill" </param>
        /// <param name="thickness">
        ///     Floating point value representing the new thickness of the selected shape's border, used only
        ///     if property = "StrokeThickness"
        /// </param>
        /// <returns> The updated Canvas </returns>
        public Canvas CustomizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, string property, string hexCode,
            float thickness = 0)
        {
            var toRender = new List<UXShape>();
            var color = (SolidColorBrush) new BrushConverter().ConvertFrom(hexCode);
            var color_b = new BoardColor(color.Color.R, color.Color.G, color.Color.B);

            //If no shape is selected while changing fill property, do not update Canvas
            if (selectedShapes.Count() == 0) return cn;

            var shUID = selectedShapes[0];

            var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

            //Check Condition 
            //if (!testing) Debug.Assert(iterat.Count() == 1);

            //Convert the UI element to Shape type 
            var sh = (Shape) iterat.ToList()[0];

            switch (property)
            {
                case "Stroke":
                    if (testing)
                        sh.Stroke = color;
                    else
                        toRender = WBOps.ChangeStrokeColor(color_b, sh.Uid);
                    break;
                case "StrokeThickness":
                    if (testing)
                        sh.StrokeThickness = thickness;
                    else
                        toRender = WBOps.ChangeStrokeWidth(thickness, sh.Uid);
                    break;
                case "Fill":

                    if (testing)
                        sh.Fill = color;
                    else
                        toRender = WBOps.ChangeShapeFill(color_b, sh.Uid);
                    break;
            }

            if (!testing)
            {
                cn = UnselectAllBB(cn, WBOps);
                //cn.Children.Remove(sh);

                //Add 
                cn = RenderUXElement(toRender, cn, WBOps);
                if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps);
            }

            return cn;
        }

        /// <summary>
        ///     Resizes the Adorner of the Shape adornedElement locally in the Visual Layer along with the shape, without sending
        ///     update to the server of this change
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="adornedElement"> Shape instance that is to be resized </param>
        /// <param name="horizontalDrag"> Displacement of the cursor on drag in horizontal direction </param>
        /// <param name="verticalDrag"> Displacement of the cursor on drag in horizontal direction </param>
        /// <param name="corner"> Thumb primitive control representing the corner of the Shape that is dragged for resizing </param>
        /// <param name="pos"> AdornerDragPos enum type that represents the positive of the dragged corner of the Shape </param>
        /// <returns> The permissible Point that can be considered as `end` by the WBOps.ResizeShape function </returns>
        public Canvas ResizeAdorner(Canvas cn, IWhiteBoardOperationHandler WBOp, Shape adornedElement,
            double horizontalDrag, double verticalDrag, Thumb corner, AdornerDragPos pos)
        {
            double oldWidth, newWidth, oldHeight, newHeight;
            double oldLeft, newLeft;
            double oldTop, newTop;
            var perm = new Point {X = 0, Y = 0};

            var rt = adornedElement.RenderTransform;

            var mat = new Matrix();
            //mat.Rotate(rt.Angle);
            mat.Translate(Canvas.GetLeft(adornedElement) + adornedElement.Width / 2,
                Canvas.GetTop(adornedElement) + adornedElement.Height / 2);
            mat.Invert();

            var drag = new Point {X = horizontalDrag, Y = verticalDrag};
            var strt = new Point
            {
                X = Canvas.GetLeft(adornedElement) + adornedElement.Width,
                Y = Canvas.GetTop(adornedElement) + adornedElement.Height
            };
            var end = Point.Add(strt, (Vector) drag);

            var transStrt = mat.Transform(strt);
            var transEnd = mat.Transform(end);

            transStrt = rt.Inverse.Transform(transStrt);
            transEnd = rt.Inverse.Transform(transEnd);

            var deltaWidth = transEnd.X - transStrt.X;
            var deltaHeight = transEnd.Y - transStrt.Y;

            var rt_angie = (RotateTransform) adornedElement.RenderTransform;
            var shapeAtAngle = (float) (rt_angie.Angle * (180 / Math.PI));

            if (adornedElement is Line)
            {
                var adornedLine = (Line) adornedElement;
                switch (pos)
                {
                    //Bottom Right Corner 
                    case AdornerDragPos.BotRight:
                        oldWidth = adornedLine.Width;
                        oldHeight = adornedLine.Height;

                        newWidth = Math.Max(adornedLine.Width + deltaWidth * 2, corner.DesiredSize.Width);
                        newHeight = Math.Max(adornedLine.Height + deltaHeight * 2, corner.DesiredSize.Height);

                        adornedLine.Width = newWidth;
                        adornedLine.Height = newHeight;

                        adornedLine.X2 = newWidth;
                        adornedLine.Y2 = newHeight;

                        Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) - verticalDrag);
                        Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) - horizontalDrag);

                        break;
                    //Top Left Corner 
                    case AdornerDragPos.TopLeft:
                        oldWidth = adornedLine.Width;
                        oldHeight = adornedLine.Height;

                        newWidth = Math.Max(adornedLine.Width - deltaWidth * 2, corner.DesiredSize.Width);
                        newHeight = Math.Max(adornedLine.Height - deltaHeight * 2, corner.DesiredSize.Height);

                        adornedLine.Width = newWidth;
                        adornedLine.Height = newHeight;

                        adornedLine.X2 = newWidth;
                        adornedLine.Y2 = newHeight;

                        adornedLine.X2 = newWidth;
                        adornedLine.Y2 = newHeight;

                        Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) + verticalDrag);
                        Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) + horizontalDrag);

                        break;
                }
            }
            else
            {
                switch (pos)
                {
                    //Bottom Right Corner 
                    case AdornerDragPos.BotRight:
                        oldWidth = adornedElement.Width;
                        oldHeight = adornedElement.Height;

                        oldWidth = adornedElement.Width;
                        oldHeight = adornedElement.Height;

                        var temp1 = Math.Max(adornedElement.Width + deltaWidth * 2, corner.DesiredSize.Width);
                        var temp2 = Math.Max(adornedElement.Height + deltaHeight * 2, corner.DesiredSize.Height);

                        if (10 > Math.Max(adornedElement.Width + deltaWidth * 2, corner.DesiredSize.Width))
                        {
                            newWidth = 10;
                            adornedElement.Width = 10;

                            //set canvas top 
                        }
                        else
                        {
                            newWidth = Math.Max(adornedElement.Width + deltaWidth * 2, corner.DesiredSize.Width);
                            adornedElement.Width = newWidth;
                        }


                        if (10 > Math.Max(adornedElement.Height + deltaHeight * 2, corner.DesiredSize.Height))
                        {
                            newHeight = 10;
                            adornedElement.Height = 10;
                            //set canvas left
                        }
                        else
                        {
                            newHeight = Math.Max(adornedElement.Height + deltaHeight * 2, corner.DesiredSize.Height);
                            adornedElement.Height = newHeight;
                        }


                        if (newHeight == 10 && newWidth == 10)
                        {
                            return cn;
                        }
                        else if (newHeight == 10)
                        {
                            //set canvas top taking component of drag along width
                            var newTp = Canvas.GetTop(adornedElement) - deltaWidth * Math.Sin(shapeAtAngle);
                            Canvas.SetTop(adornedElement, newTp);
                            //set canvas left taking component of drag along width
                            var newLef = Canvas.GetLeft(adornedElement) - deltaWidth * Math.Cos(shapeAtAngle);
                            Canvas.SetLeft(adornedElement, newLef);
                        }
                        else if (newWidth == 10)
                        {
                            //set canvas top taking component of drag along height
                            var newTp = Canvas.GetTop(adornedElement) - deltaHeight * Math.Cos(shapeAtAngle);
                            Canvas.SetTop(adornedElement, newTp);
                            //set canvas left taking component of drag along height
                            var newLef = Canvas.GetLeft(adornedElement) - deltaHeight * Math.Sin(shapeAtAngle);
                            Canvas.SetLeft(adornedElement, newLef);
                        }
                        else
                        {
                            //set canvas top 
                            var newTp = Canvas.GetTop(adornedElement) - verticalDrag;
                            Canvas.SetTop(adornedElement, newTp);
                            //set canvas left
                            var newLef = Canvas.GetLeft(adornedElement) - horizontalDrag;
                            Canvas.SetLeft(adornedElement, newLef);
                        }

                        break;

                    //Top Left Corner 
                    case AdornerDragPos.TopLeft:
                        oldWidth = adornedElement.Width;
                        oldHeight = adornedElement.Height;

                        newWidth = Math.Max(Math.Max(adornedElement.Width - deltaWidth * 2, corner.DesiredSize.Width),
                            0);
                        newHeight = Math.Max(
                            Math.Max(adornedElement.Height - deltaHeight * 2, corner.DesiredSize.Height), 0);

                        adornedElement.Width = newWidth;
                        adornedElement.Height = newHeight;

                        Canvas.SetTop(adornedElement,
                            Canvas.GetTop(adornedElement) + Math.Min(verticalDrag, adornedElement.Height / 2));
                        Canvas.SetLeft(adornedElement,
                            Canvas.GetLeft(adornedElement) + Math.Min(horizontalDrag, adornedElement.Width / 2));
                        break;
                }
            }

            return cn;
        }

        /// <summary>
        ///     Request the server to resize the Shape shp when mouse is dragged from `strt` to `end` on the corner of `shp`
        ///     represented by `pos`, only called when Resize operation is finalised, i.e, `shapeComp == true`, else ResizeAdorners
        ///     is called.
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="pos"> AdornerDragPos enum type that represents the positive of the dragged corner of the Shape </param>
        /// <returns> The updated Canvas </returns>
        public Canvas ResizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, Shape shp, Point strt, Point end,
            AdornerDragPos pos)
        {
            var C_strt = new Coordinate((int) (cn.Height - strt.Y), (int) strt.X);
            var C_end = new Coordinate((int) (cn.Height - end.Y), (int) end.X);

            List<UXShape> toRender;

            var drgPos = DragPos.NONE;
            switch (pos)
            {
                case AdornerDragPos.BotRight:
                    drgPos = DragPos.BOTTOM_RIGHT;
                    break;
                case AdornerDragPos.TopLeft:
                    drgPos = DragPos.TOP_LEFT;
                    break;
            }

            cn = UnselectAllBB(cn, WBOps);
            //cn.Children.Remove(shp);

            toRender = WBOps.ResizeShape(C_strt, C_end, shp.Uid, drgPos, true);
            cn = RenderUXElement(toRender, cn, WBOps);
            if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps);

            return cn;
        }


        public string getUserId(Shape sh, IWhiteBoardOperationHandler WBOps)
        {
            return WBOps.GetUserName(sh.Uid);
        }
    }

    /// <summary>
    ///     Class to manage existing and new FreeHand instances by providing various methods by aggregating WhiteBoard Module
    /// </summary>
    public class FreeHand
    {
        private readonly bool testing;
        private string assgn_uid;
        private Coordinate C_end;
        private Polyline poly;
        private SolidColorBrush polyLineColor;
        private float polyLineThickness;
        private Coordinate prev;

        //Consructor for the class 
        public FreeHand(bool testing = false)
        {
            polyLineColor = new SolidColorBrush(Colors.Black);
            polyLineThickness = 5;
            assgn_uid = "-1";
            prev = new Coordinate(0, 0);
            C_end = new Coordinate(0, 0);
            this.testing = testing;
        }

        public SolidColorBrush GetColor()
        {
            return polyLineColor;
        }

        public void SetColor(string hexCode)
        {
            polyLineColor = (SolidColorBrush) new BrushConverter().ConvertFrom(hexCode);
        }

        public float GetThickness()
        {
            return polyLineThickness;
        }

        public void SetThickness(float thick)
        {
            polyLineThickness = thick;
        }


        /// <summary>
        ///     Render FreeHand instances shape updates on canvas
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            if (shps == null || shps.Count() == 0) return cn;

            //UXShape has attribute
            foreach (var shp in shps)
                switch (shp.UxOperation)
                {
                    case UXOperation.CREATE:
                        cn.Children.Add(shp.WindowsShape);
                        break;
                    case UXOperation.DELETE:
                        var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shp.WindowsShape.Uid);

                        cn.Children.Remove(iterat.ToList()[0]);
                        break;
                }

            return cn;
        }

        /// <summary>
        ///     ViewModel method to draw PolyLine/Eraser requested by the user
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="pt"> New point to be added into the PolyLine PointCollection </param>
        /// <param name="creation">
        ///     Boolean which is true if the first MouseDown event occured, i.e, user has just started to draw
        ///     the new polyline
        /// </param>
        /// <param name="strokeColor"> Represents the hexcode of the color of polyline to be drawn </param>
        /// <param name="isEraser">
        ///     Boolean which is true if the drawn polyline is supposed to be an Eraser instance, used to set
        ///     the Windows.Shapes.Tag property which is used by 'ChangeWbBackground' method locally
        /// </param>
        /// <returns> The updated Canvas </returns>
        public Canvas DrawPolyline(Canvas cn, IWhiteBoardOperationHandler WBOps, Point pt, bool creation = false,
            bool isEraser = false, bool shapeComp = false)
        {
            var stroke = new BoardColor(polyLineColor.Color.R, polyLineColor.Color.G, polyLineColor.Color.B);

            //SERVER REQUEST TO CREATE FINAL SHAPE
            var toRender = new List<UXShape>();

            if (creation)
            {
                poly = new Polyline();

                var iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == "-1");
                Console.Write(assgn_uid);

                //Check Condition that previous temporary polylines have been cleaned up
                //Debug.Assert(iterat.Count() <= 1);

                //assigning special UID of -1 to temporary shapes
                poly.Uid = "-1";

                poly.Stroke = polyLineColor;
                poly.StrokeThickness = polyLineThickness;
                poly.StrokeLineJoin = PenLineJoin.Round;
                poly.StrokeDashCap = PenLineCap.Round;
                poly.Points.Add(pt);

                if (isEraser)
                {
                    poly.Tag = "ERASER";
                }
                else
                {
                    poly.Tag = "FREEHAND";

                    if (!testing)
                    {
                        C_end = new Coordinate((float) pt.X, (float) pt.Y);
                        toRender = WBOps.CreatePolyline(C_end, C_end, polyLineThickness, stroke);
                        prev = C_end;
                        if (!(toRender == null || toRender.Count == 0)) assgn_uid = toRender[0].WindowsShape.Uid;
                    }
                    else
                    {
                        assgn_uid = "auniquepoly";
                        poly.Uid = assgn_uid;
                    }
                }

                cn.Children.Add(poly);
            }
            else
            {
                if (pt.X <= 0 || pt.Y <= 0 || pt.X >= cn.Width || pt.Y >= cn.Height)
                {
                    //MessageBox.Show("Cursor went out of screen");
                    shapeComp = true;
                }
                else
                {
                    if (isEraser)
                    {
                        poly.Points.Add(pt);

                        if (poly.Points.Count > 40) poly.Points.RemoveAt(0);
                    }
                    else
                    {
                        if (!poly.Points.Contains(pt))
                        {
                            if (assgn_uid != "-1")
                            {
                                poly.Points.Add(pt);

                                if (!testing)
                                {
                                    C_end = new Coordinate((float) pt.X, (float) pt.Y);
                                    toRender = WBOps.CreatePolyline(prev, C_end, polyLineThickness, stroke, assgn_uid);
                                    prev = C_end;
                                }
                            }
                            else
                            {
                                cn.Children.Remove(poly);
                            }
                        }
                    }
                }
            }

            if (shapeComp)
            {
                if (isEraser)
                {
                    cn.Children.Remove(poly);
                }
                else
                {
                    //Sending final point with shapeComp=true
                    if (assgn_uid != "-1")
                    {
                        if (!testing)
                        {
                            C_end = new Coordinate((float) poly.Points.Last().X, (float) poly.Points.Last().Y);
                            toRender = WBOps.CreatePolyline(C_end, C_end, polyLineThickness, stroke, assgn_uid, true);

                            //Since the WBOps.CreatePolyline sends render requests of form DELETE then CREATE,
                            //we choose to ignore DELETE as we are doing temporary rendering
                            var pl = (Polyline) toRender[1].WindowsShape;

                            //Removing temporary render from Canvas
                            cn.Children.Remove(poly);

                            if (!(toRender == null || toRender.Count() == 0))
                            {
                                //Adjusting the polyline render request to the user preference during Create Polyline operation
                                ((Polyline) toRender.ElementAt(1).WindowsShape).StrokeLineJoin = PenLineJoin.Round;
                                ((Polyline) toRender.ElementAt(1).WindowsShape).StrokeDashCap = PenLineCap.Round;

                                //Rendering the Polyline onto the Canvas
                                cn = RenderUXElement(new List<UXShape> {toRender[1]}, cn, WBOps);
                            }

                            assgn_uid = "-1";
                        }
                    }
                    else
                    {
                        cn.Children.Remove(poly);
                    }
                }
            }

            return cn;
        }

        public Canvas DeletePolyline(Canvas cn, IWhiteBoardOperationHandler WBOps, Polyline selectedLine)
        {
            //Call : Render UX element to delete the polyline 
            if (!testing)
            {
                var toRender = WBOps.DeleteShape(selectedLine.Uid);
                cn = RenderUXElement(toRender, cn, WBOps);
            }
            else
            {
                cn.Children.Remove(selectedLine);
            }

            return cn;
        }
    }


    /// <summary>
    ///     View Model of Whiteboard in MVVM design pattern
    /// </summary>
    public class WhiteBoardViewModel : IClientBoardStateListener, INotifyPropertyChanged, IClientSessionNotifications,
        IWhiteBoardUpdater
    {
        /// UX sets this enum to different options when user clicks on the appropriate tool icon
        public enum WBTools
        {
            Initial,

            /// Initialised value, never to be used again
            Selection,
            NewLine,
            NewRectangle,
            NewEllipse,
            Rotate,
            Move,
            Eraser,
            FreeHand
        }

        private readonly IUXClientSessionManager _modelDb;
        private readonly IClientBoardStateManager manager;
        private readonly bool testing;

        //private List<string> ckptList;
        public ObservableCollection<string> _chk;

        //To be bound to the number of "Checkpoint #n" in Restore Checkpoint dropdown in Whiteboard.xaml
        private int _numCheckpoints;
        private WBTools activeTool;
        public Point end;
        public FreeHand freeHand;
        public Canvas GlobCanvas;

        private bool isSubscribedToWBState;
        public ShapeManager shapeManager;

        public Point start;

        public IWhiteBoardOperationHandler WBOps;

        /// <summary>
        ///     Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module
        /// </summary>
        public WhiteBoardViewModel(Canvas GlobCanvas, bool testing = false)
        {
            activeTool = WBTools.Initial;
            this.GlobCanvas = GlobCanvas;

            this.testing = testing;
            if (!testing)
            {
                WBOps = new WhiteBoardOperationHandler(new Coordinate((int) GlobCanvas.Height, (int) GlobCanvas.Width));
                manager = ClientBoardStateManager.Instance;
                _modelDb = SessionManagerFactory.GetClientSessionManager();
                _modelDb.SubscribeSession(this);
            }

            shapeManager = new ShapeManager(this.testing);
            freeHand = new FreeHand(this.testing);

            _numCheckpoints = 0;
            _chk = new ObservableCollection<string>();

            //Initially not subscribed to WBStateManager
            if (!testing)
            {
                isSubscribedToWBState = false;

                //Canvas initialised as non-responsive until FETCH_STATE requests are fully completed
                this.GlobCanvas.IsEnabled = false;
            }
        }

        public ObservableCollection<string> chckList
        {
            get => _chk;
            set
            {
                var n = NumCheckpoints;
                var temp = new ObservableCollection<string>();
                for (var i = 0; i < n; i++) temp.Add("Checkpoint #" + (i + 1));
                _chk = temp;
                OnPropertyChanged(nameof(chckList));
            }
        }

        public int NumCheckpoints
        {
            get => _numCheckpoints;
            set
            {
                if (_numCheckpoints != value)
                {
                    _numCheckpoints = value;
                    OnPropertyChanged(nameof(_numCheckpoints));

                    var n = _numCheckpoints;

                    var temp = new ObservableCollection<string>();
                    for (var i = 0; i < n; i++) chckList.Add("Checkpoint #" + (i + 1));
                    chckList = temp;
                }
            }
        }

        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

        /// <summary>
        //// Listens to the updates from the board state manager and serves the requests accordingly
        /// </summary>
        public void OnUpdateFromStateManager(List<UXShapeHelper> ServerUpdate)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<List<UXShapeHelper>>(ServerUpdate =>
                    {
                        lock (shapeManager)
                        lock (freeHand)
                        {
                            processServerUpdateBatch(ServerUpdate);
                        }
                    }
                ),
                ServerUpdate);
        }

        /// <summary>
        ///     Listenes to the updates to the sesssion, used as a trigger event to subscribe to the Board State Manager
        /// </summary>
        public void OnClientSessionChanged(SessionData session)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<SessionData>(session =>
                {
                    lock (this)
                    {
                        if (!isSubscribedToWBState)
                        {
                            //When this notification occurs, Dashboard has initialised the WhiteboardStateManager properly and we can finally subscribe to it
                            //It is essential that this .Subscribe() method is only called once for a Whiteboard UX ViewModel
                            manager.Subscribe(this, "whiteboard");
                            GlobCanvas.IsEnabled = false;
                            isSubscribedToWBState = true;
                        }
                    }
                }),
                session);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void processServerUpdateBatch(List<UXShapeHelper> receivedHelper)
        {
            var received = UXShape.ToUXShape(receivedHelper);

            //WE ASSUME that an update batch can only have a single Clear Canvas request 
            var iterat = received.OfType<UXShape>().Where(x => x.OperationType == Operation.CLEAR_STATE);
            //if (testing) Debug.Assert(iterat.Count() == 1);

            //WE ASSUME that an update batch can only have either no FETCH_STATE requests, or all FETCH_STATE requests
            var iterat2 = received.OfType<UXShape>().Where(x => x.OperationType == Operation.FETCH_STATE);
            //if (testing) Debug.Assert(iterat2.Count() == 0 || iterat2.Count() == received.Count());
            if (received[0].OperationType == Operation.FETCH_STATE)
            {
                //New user has joined, the 'numCheckpoints' was last updated in the ViewModel Constructor as 0
                //if (testing) Debug.Assert(_numCheckpoints == 0);

                //ASSUMING that the user sending the Clear Board request has already been SHOWN THE WARNING, we clear the Canvas
                GlobCanvas.Children.Clear();
                //if (testing) Debug.Assert(GlobCanvas.IsEnabled == false);
                //Supposed to update the "Restore Checkpoint" dropdown with CheckPointNumber number of dropdown tiles
                increaseCheckpointNum(received[0].CheckPointNumber);
            }

            //WE ASSUME that an update batch can only have either no FETCH_CHECKPOINT requests, or all FETCH_CHECKPOINT requests
            var iterat3 = received.OfType<UXShape>().Where(x => x.OperationType == Operation.FETCH_CHECKPOINT);
            //if (testing) Debug.Assert(iterat3.Count() == 0 || iterat3.Count() == received.Count());
            if (received[0].OperationType == Operation.FETCH_CHECKPOINT)
                //ASSUMING THAT THE USER HAS ACCEPTED THE WARNING TO SAVE CHECKPOINT, SINCE ALL THE CHANGES MADE SINCE LAST CHECKPOINT WOULD BE LOST FOREVER
                GlobCanvas.Children.Clear();

            for (var i = 0; i < received.Count(); i++)
                switch (received[i].OperationType)
                {
                    //Case when new user joins and the whole state of server is sent to user
                    case Operation.FETCH_STATE:

                        ///
                        /// ASSUMING that this batch of server update contains ONLY AND ONLY Operation.FETCH_STATE requests
                        /// VERIFY THE ABOVE ASSUMPTION FROM ASHISH
                        ///
                        if (received[i].WindowsShape == null)
                        {
                            GlobCanvas.IsEnabled = true;
                            continue;
                        }

                        if (received[i].WindowsShape is Polyline)
                            GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);
                        else
                            GlobCanvas =
                                shapeManager.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);

                        //Enabling the Canvas as all of the FETCH_STATE render requests in current batch have been rendered and the new user can now use Canvas
                        if (i == received.Count() - 1) GlobCanvas.IsEnabled = true;

                        break;

                    case Operation.FETCH_CHECKPOINT:

                        if (received[i].WindowsShape is Polyline)
                            GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);
                        else
                            GlobCanvas =
                                shapeManager.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);

                        //Enabling the Canvas as all of the FETCH_STATE render requests in current batch have been rendered and the new user can now use Canvas
                        if (i == received.Count() - 1) GlobCanvas.IsEnabled = true;
                        break;
                    case Operation.CLEAR_STATE:
                        //ASSUMING THAT THE USER HAS ACCEPTED THE WARNING TO CLEAR FRAME, SINCE ALL THE CHANGES MADE SINCE LAST CHECKPOINT WOULD BE LOST FOREVER
                        //based on above assumption that current server update batch can only have one CLEAR_STATE request

                        GlobCanvas.Children.Clear();
                        //Enabling Canvas as it is now consistent with the server state
                        GlobCanvas.IsEnabled = true;

                        break;

                    case Operation.CREATE_CHECKPOINT:
                        increaseCheckpointNum(received[i].CheckPointNumber);
                        break;

                    case Operation.CREATE:
                        //If the operation is CREATE, directly render it onto the Canvas
                        if (received[i].WindowsShape == null) Trace.WriteLine("RenderUXElement received null");
                        else if (received[i].WindowsShape is Polyline)
                            GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);
                        else
                            GlobCanvas =
                                shapeManager.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);

                        break;
                    case Operation.DELETE:
                        //If a selected shape is to be delete by server request, unselect the shape first
                        if (shapeManager.selectedShapes.Count > 0 &&
                            shapeManager.selectedShapes.Contains(received[i].WindowsShape.Uid))
                            GlobCanvas = shapeManager.UnselectAllBB(GlobCanvas, WBOps);
                        //If the operation is DELETE, directly render it onto the Canvas
                        if (received[i].WindowsShape == null) Trace.WriteLine("RenderUXElement received null");
                        else if (received[i].WindowsShape is Polyline)
                            GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);
                        else
                            GlobCanvas =
                                shapeManager.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);

                        break;
                    case Operation.MODIFY:
                        //If the operation is MODIFY, directly render it onto the Canvas
                        if (received[i].WindowsShape == null) Trace.WriteLine("RenderUXElement received null");
                        else if (received[i].WindowsShape is Polyline)
                            GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);
                        else
                            GlobCanvas =
                                shapeManager.RenderUXElement(new List<UXShape> {received[i]}, GlobCanvas, WBOps);

                        break;
                }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


        /// <summary>
        ///     Changes the Background color of Canvas in View
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="hexCode"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> The updated Canvas </returns>
        public void ChangeWbBackground(string WbhexCode)
        {
            GlobCanvas.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(WbhexCode);
        }

        /// <summary>
        ///     Update the activeTool based on selected function on Toolbar
        /// </summary>
        /// <param name="clickedTool">
        ///     Defines the tool on which the user clicked to be used to set the 'activeTool' enum
        ///     accordingly
        /// </param>
        /// <returns> void, upon altering the 'activeTool' of this class instance accordingly </returns>
        public void ChangeActiveTool(string clickedTool)
        {
            switch (clickedTool)
            {
                case "PenTool":
                    activeTool = WBTools.FreeHand;
                    break;
                case "LineTool":
                    activeTool = WBTools.NewLine;
                    break;
                case "RectTool":
                    activeTool = WBTools.NewRectangle;
                    break;
                case "EllipseTool":
                    activeTool = WBTools.NewEllipse;
                    break;
                case "SelectTool":
                    activeTool = WBTools.Selection;
                    break;
                case "EraseTool":
                    activeTool = WBTools.Eraser;
                    break;
            }
        }


        /// <summary>
        ///     Sets the `selectMouseDownPos` variable necessary for sending final Create/Move/Rotate requests to the server
        /// </summary>
        public void setSelectMouseDownPos(Point pnt)
        {
            shapeManager.selectMouseDownPos = pnt;
        }

        /// <summary>
        ///     Returns the Active Tool enum that is currently selected
        /// </summary>
        public WBTools GetActiveTool()
        {
            return activeTool;
        }

        /// <summary>
        ///     Changes the Privilege level of the current user
        /// </summary>
        public void ChangeActivityState()
        {
            WBOps.SwitchState();
        }

        /// <summary>
        ///     Function that should be called only when user accepts warning of Clear Whiteboard
        /// </summary>
        public Canvas ClearCanvas(Canvas cn)
        {
            if (shapeManager.selectedShapes.Count() > 0) cn = shapeManager.UnselectAllBB(cn, WBOps);
            //cn.Children.Clear();

            //Only calling state manager's clear whiteboard here, whiteboard would be cleared when request received in the listener thread
            var success = manager.ClearWhiteBoard();
            if (success) cn.IsEnabled = false;
            return cn;
        }

        /// <summary>
        ///     Checkpoints the drawn shapes on canvas
        /// </summary>
        public void SaveFrame()
        {
            manager.SaveCheckpoint();
        }

        public Canvas DeleteShape(Canvas cn)
        {
            cn = shapeManager.DeleteShape(cn, WBOps);
            return cn;
        }

        /// <summary>
        ///     Restores the selected checkpoint, should be called only when user accepts warning of Restore Whiteboard
        /// </summary>
        public void RestoreFrame(int CheckNum, Canvas GlobCanvas)
        {
            //Unselecting all before clearing canvas 
            if (shapeManager.selectedShapes.Count > 0)
                this.GlobCanvas = shapeManager.UnselectAllBB(this.GlobCanvas, WBOps);

            //The user should not be able to draw anything on the Canvas while the checkpoint is being fetched
            GlobCanvas.IsEnabled = false;

            manager.FetchCheckpoint(CheckNum);
        }

        /// <summary>
        //// Unselects the selected shapes and sends an Undo Request to the board state manager, who sends the appropriate series of render requests received through the `OnUpdateFromStateManager`
        /// </summary>
        public void sendUndoRequest()
        {
            if (shapeManager.selectedShapes.Count > 0) GlobCanvas = shapeManager.UnselectAllBB(GlobCanvas, WBOps);

            var renderUndo = WBOps.Undo();
            for (var i = 0; i < renderUndo.Count(); i++)
                if (renderUndo[i].WindowsShape is Polyline)
                    GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {renderUndo[i]}, GlobCanvas, WBOps);
                else
                    GlobCanvas = shapeManager.RenderUXElement(new List<UXShape> {renderUndo[i]}, GlobCanvas, WBOps);
        }

        /// <summary>
        //// Unselects the selected shapes and sends a Redo Request to the board state manager, who sends the appropriate series of render requests received through the `OnUpdateFromStateManager`
        /// </summary>
        public void sendRedoRequest()
        {
            if (shapeManager.selectedShapes.Count > 0) GlobCanvas = shapeManager.UnselectAllBB(GlobCanvas, WBOps);

            var renderUndo = WBOps.Redo();
            for (var i = 0; i < renderUndo.Count(); i++)
                if (renderUndo[i].WindowsShape is Polyline)
                    GlobCanvas = freeHand.RenderUXElement(new List<UXShape> {renderUndo[i]}, GlobCanvas, WBOps);
                else
                    GlobCanvas = shapeManager.RenderUXElement(new List<UXShape> {renderUndo[i]}, GlobCanvas, WBOps);
        }


        /// <summary>
        //// Performs the appropriate update to the UI when new checkpoint count from server is received
        /// </summary>
        public void increaseCheckpointNum(int latestNumCheckpoints)
        {
            if (latestNumCheckpoints == _numCheckpoints) return;

            //Changing the bound element to the 'Load Checkpoint' dropdown
            NumCheckpoints = latestNumCheckpoints;
        }
    }
}