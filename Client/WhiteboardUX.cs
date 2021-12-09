/**
 * Owned By: Aniket Singh Rajpoot
 * Created By: Aniket Singh Rajpoot
 * Date Created: 25/10/2021
 * Date Modified: 28/11/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;
using Whiteboard;
using System.Windows.Documents;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Dashboard.Client.SessionManagement;
using Dashboard;

namespace Client
{
    /// Enum used to pass the info about the dragged corner of the shape in case of Resize operation
    public enum AdornerDragPos
    {
        TopLeft,
        BotRight,
        TopRight,
        BotLeft
    };

    /// <summary>
    /// Interface which listens to fetched server updates by IWhiteBoardState and local updates by IShapeOperation
    /// </summary>
    interface IWhiteBoardUpdater
    {
        /// <summary>
        /// Fetch updates from IWhiteBoardState for rendering in the view  
        /// </summary>
        abstract void processServerUpdateBatch(List <UXShapeHelper> ServerUpdates);
    }

    public class BorderAdorner : Adorner
    {
        //use thumb for resizing elements
        Thumb topLeft, bottomRight, topRight, bottomLeft;
        //visual child collection for adorner
        VisualCollection visualChilderns;
        ShapeManager shapeManager;
        Canvas cn;
        private UIElement adornedShape;
        private bool testing;

        Point dragStart, dragEnd, permissibleDragEnd;
        AdornerDragPos lastDraggedCorner;

        IWhiteBoardOperationHandler WbOp;

        public BorderAdorner(UIElement element, ShapeManager shapeManager, Canvas cn, IWhiteBoardOperationHandler WbOp, bool testing = false) : base(element)
        {
            visualChilderns = new VisualCollection(this);
            adornedShape = element as Shape;
            this.shapeManager = shapeManager;
            this.cn = cn;
            this.WbOp = WbOp;

            this.dragStart = new Point { X = 0, Y = 0 };
            this.dragEnd = new Point { X = 0, Y = 0 };
            this.permissibleDragEnd = new Point { X = 0, Y = 0 };
            this.testing = testing;

            if (element is not System.Windows.Shapes.Line)
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

        private void Adorner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(cn);
            dragEnd = e.GetPosition(cn);
            return;
        }

        private void Adorner_MouseUp(object sender, DragCompletedEventArgs e)
        {

            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb corner = sender as Thumb;
            
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
            {
                shapeManager.ResizeShape(cn, WbOp, (Shape)adornedElement, dragStart, dragEnd, AdornerDragPos.TopLeft);
            }
            else if (corner.Equals(bottomRight))
            {
                shapeManager.ResizeShape(cn, WbOp, (Shape)adornedElement, dragStart, dragEnd, AdornerDragPos.BotRight);
            }

            return;
        }

        private void BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            dragEnd.X = dragStart.X + e.HorizontalChange;
            dragEnd.Y = dragStart.Y + e.VerticalChange;

            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb bottomRightCorner = sender as Thumb;
            //setting new height and width after drag
            if (adornedElement != null && bottomRightCorner != null)
            {
                EnforceSize(adornedElement);
                //this.shapeManager.ResizeAdorner(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, bottomRightCorner, AdornerDragPos.BotRight);
                this.lastDraggedCorner = AdornerDragPos.BotRight;
            }
        }

        private void TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            dragEnd.X = dragStart.X + e.HorizontalChange;
            dragEnd.Y = dragStart.Y + e.VerticalChange;

            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb topLeftCorner = sender as Thumb;
            //setting new height, width and canvas top, left after drag
            if (adornedElement != null && topLeftCorner != null)
            {
                EnforceSize(adornedElement);
                //this.shapeManager.ResizeAdorner(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, topLeftCorner, AdornerDragPos.TopLeft);
                this.lastDraggedCorner = AdornerDragPos.TopLeft;
            }
        }

        public void BuildAdornerCorners(ref Thumb cornerThumb, Cursor customizedCursors)
        {
            //adding new thumbs for adorner to visual childern collection
            if (cornerThumb != null) return;
            cornerThumb = new Thumb() { Cursor = customizedCursors, Height = 10, Width = 10, Opacity = 0.5, Background = new SolidColorBrush(Colors.Purple) };
            visualChilderns.Add(cornerThumb);
        }

        public void EnforceSize(FrameworkElement element)
        {
            if (element.Width.Equals(Double.NaN))
                element.Width = element.DesiredSize.Width;
            if (element.Height.Equals(Double.NaN))
                element.Height = element.DesiredSize.Height;

            //enforce size of element not exceeding to it's parent element size
            FrameworkElement parent = element.Parent as FrameworkElement;

            if (parent != null)
            {
                element.MaxHeight = parent.ActualHeight;
                element.MaxWidth = parent.ActualWidth;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);

            double desireWidth = AdornedElement.DesiredSize.Width;
            double desireHeight = AdornedElement.DesiredSize.Height;

            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            if (adornedShape is not System.Windows.Shapes.Line)
            {
                //arranging thumbs
                topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                topRight.Arrange(new Rect(desireWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                bottomLeft.Arrange(new Rect(-adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));
                bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            }
            else
            {
                //arranging thumbs
                topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
                bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            }
            return finalSize;
        }
        protected override int VisualChildrenCount { get { return visualChilderns.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChilderns[index]; }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }

    /// <summary>
    /// Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module  
    /// </summary>
    public class ShapeManager
    {

        public List<string> selectedShapes = new List<string>();
        public AdornerLayer adornerLayer;
        public Shape underCreation;
        public System.Windows.Point selectMouseDownPos;
        public System.Windows.Point selectMouseStuck;

        //Variable to keep track of the Uid of the new shape that is currently under creation
        private string uidShapeCreate = null;
        private bool testing;


        //THIS IS SUPPOSED TO BE A UNIQUE UID THAT IS NOT USED BY THE CLIENT MODULE
        //for assigning temporary UID to the shape being created
        int counter = 0;
        public ShapeManager(bool testing = false)
        {
            this.testing = testing;
        }

        public Canvas CreateSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp)
        {
            adornerLayer = AdornerLayer.GetAdornerLayer(sh);
            BorderAdorner adr = new BorderAdorner(sh, this, cn, WBOp, testing: this.testing);

            if (!this.testing)
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
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);

                //Check Condition 
                int cnt = iterat.Count();
                //if (testing) Debug.Assert(cnt == 1);
                Shape sh = (iterat.ToList()[0]) as Shape;
                if (!testing) cn = DeleteSelectionBB(cn, sh, WBOp);
            }

            selectedShapes.Clear();

            return cn;
        }

        /// <summary>
        /// Deletes Selected Shapes : Used for Testing purposes only  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> void, upon altering the 'selectedShapes' of this class instane accordingly </returns>
        public Canvas DeleteSelectedShapes(Canvas cn, IWhiteBoardOperationHandler WBOp)
        {
            //remove shapes 
            foreach (var item in selectedShapes)
            {
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);

                //Check Condition 
                //if(testing) Debug.Assert(iterat.Count() == 1);

                Shape sh = (iterat.ToList()[0]) as Shape;
                cn.Children.Remove(sh);
            }
            selectedShapes.Clear();
            return cn;
        }

        /// <summary>
        /// Handle input events for selection : this includes evnents for single shape selection and multiple shape selection  
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
                    Trace.WriteLine("Selection of shape with Uid = " + sh.Uid.ToString() + "requested by user with Ctrl NOT pressed");

                    //If selected shape is already selected or we select a different shape  
                    if (selectedShapes.Count > 0)
                    {
                        if (selectedShapes.Contains(sh.Uid.ToString()))
                        {
                            cn = UnselectAllBB(cn, WBOp);
                        }
                        else
                        {
                            cn = UnselectAllBB(cn, WBOp);
                            selectedShapes.Add(sh.Uid.ToString());
                            cn = CreateSelectionBB(cn, sh, WBOp);
                        }
                    }
                    else
                    {
                        selectedShapes.Add(sh.Uid.ToString());
                        cn = CreateSelectionBB(cn, sh, WBOp);
                    }
                    break;
                //multiple shape selection case
                case 1:
                    Trace.WriteLine("Selection of shape with Uid = " + sh.Uid.ToString() + "requested by user with Ctrl pressed");
                    if (selectedShapes.Contains(sh.Uid.ToString()))
                    {
                        cn = DeleteSelectionBB(cn, sh, WBOp);
                        selectedShapes.Remove(sh.Uid.ToString());
                    }
                    else
                    {
                        selectedShapes.Add(sh.Uid.ToString());
                        cn = CreateSelectionBB(cn, sh, WBOp);
                    }

                    break;

            }
            Trace.WriteLine("List of Uids of selected shapes at the end of Client.ShapeManager.SelectShape is: " + selectedShapes.ToString());
            return cn;
        }


        /// <summary>
        /// Handles Shape creation requests by the user 
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="strokeWidth"> Float determining the thickness of border of drawn shape (OR) thickness of the stroke in freehand drawing</param>
        /// <param name="strokeColor"> Hex code representing the color of the drawn shape </param>
        /// <param name="shapeId"> Attribute to recursively keep track of the drawn shape visually by the user, initialised as null and equal to the UID assigned by the WB module for the remaining iterations </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> Final Canvas instance with the newly rendered/deleted shapes </returns>
        public Canvas CreateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, WhiteBoardViewModel.WBTools activeTool, Point strt, Point end, float strokeWidth = 1, string fillColor = "#FFFFFF", bool shapeComp = false)
        {
            //List of server render request, used only when shapeComp is true
            List<UXShape> toRender;

            //Brush for Fill 
            SolidColorBrush strokeColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(fillColor));

            //Brush With Opacity 
            SolidColorBrush strokeOpacityBrush = new SolidColorBrush(Colors.Aqua);
            strokeOpacityBrush.Opacity = .25d;

            //Brush for Border 
            SolidColorBrush blackBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));

            BoardColor strk_clr = new BoardColor(strokeColorBrush.Color.R, strokeColorBrush.Color.G, strokeColorBrush.Color.B);

            //if (end.X < 0 || end.Y < 0 || end.X > cn.Width || end.Y > cn.Height) MessageBox.Show("Cursor went out of screen");

            switch (activeTool)
            {
                case WhiteBoardViewModel.WBTools.NewLine:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of a line with start = " + strt.ToString() + "end = " + end.ToString());
                        //toRender = WBOps.CreateLine(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp); //return is of form List of UXShape
                        //cn = this.RenderUXElement(toRender, cn);

                        //TEMP: Logic to simulate continuous deletion & addition while drawing new shapes
                        if (this.uidShapeCreate == null)
                        {
                            this.underCreation = new System.Windows.Shapes.Line();

                            ((System.Windows.Shapes.Line)underCreation).X1 = 0;
                            ((System.Windows.Shapes.Line)underCreation).X2 = end.X - strt.X;
                            ((System.Windows.Shapes.Line)underCreation).Y1 = 0;
                            ((System.Windows.Shapes.Line)underCreation).Y2 = end.Y - strt.Y;

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
                            ((System.Windows.Shapes.Line)underCreation).X1 = 0;
                            ((System.Windows.Shapes.Line)underCreation).X2 = end.X - strt.X;
                            ((System.Windows.Shapes.Line)underCreation).Y1 = 0;
                            ((System.Windows.Shapes.Line)underCreation).Y2 = end.Y - strt.Y;

                            Canvas.SetLeft(underCreation, strt.X);
                            Canvas.SetTop(underCreation, strt.Y);
                        }

                    }
                    break;
                case WhiteBoardViewModel.WBTools.NewRectangle:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of a rectangle with start = " + strt.ToString() + "end = " + end.ToString());
                        //toRender = WBOps.CreateRectangle(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        //cn = this.RenderUXElement(toRender, cn);

                        if (this.uidShapeCreate == null)
                        {
                            this.underCreation = new System.Windows.Shapes.Rectangle();

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
                        Trace.WriteLine("User requested creation of an ellipse with start = " + strt.ToString() + "end = " + end.ToString());
                        //toRender = WBOps.CreateEllipse(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        //cn = this.RenderUXElement(toRender, cn);

                        if (this.uidShapeCreate == null)
                        {
                            this.underCreation = new System.Windows.Shapes.Ellipse();

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

            if (shapeComp == true)
            {

                if (testing)
                {
                    switch (activeTool)
                    {
                        case WhiteBoardViewModel.WBTools.NewLine:
                            lock (this)
                            {
                                Trace.WriteLine("User requested creation of a line with start = " + strt.ToString() + "end = " + end.ToString());

                                //Updating the underCreation Shape Accordingly 
                                ((System.Windows.Shapes.Line)underCreation).X1 = 0;
                                ((System.Windows.Shapes.Line)underCreation).X2 = end.X - strt.X;
                                ((System.Windows.Shapes.Line)underCreation).Y1 = 0;
                                ((System.Windows.Shapes.Line)underCreation).Y2 = end.Y - strt.Y;

                                Canvas.SetLeft(underCreation, strt.X);
                                Canvas.SetTop(underCreation, strt.Y);

                            }
                            break;
                        case WhiteBoardViewModel.WBTools.NewRectangle:
                            lock (this)
                            {
                                Trace.WriteLine("User requested creation of a rectangle with start = " + strt.ToString() + "end = " + end.ToString());

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
                                Trace.WriteLine("User requested creation of an ellipse with start = " + strt.ToString() + "end = " + end.ToString());

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
                    SelectShape(cn, underCreation, WBOps, 0);
                }
                else
                {
                    underCreation.StrokeThickness = 2;
                    uidShapeCreate = null;

                    //Coordinate C_strt = new Coordinate((float)strt.X, (float)strt.Y);
                    //Coordinate C_end = new Coordinate((float)end.X, (float)end.Y);

                    Coordinate C_strt = new Coordinate(((int)(cn.Height - strt.Y)), ((int)strt.X));
                    Coordinate C_end = new Coordinate(((int)(cn.Height - end.Y)), ((int)end.X));

                    BoardColor stroke = new BoardColor(blackBrush.Color.R, blackBrush.Color.G, blackBrush.Color.B);

                    //SERVER REQUEST TO Create FINAL SHAPE
                    toRender = new List<UXShape>();
                    switch (activeTool)
                    {
                        case WhiteBoardViewModel.WBTools.NewLine:
                            toRender = WBOps.CreateLine(C_strt, C_end, 2, stroke, shapeId: null, shapeComp: true);
                            break;
                        case WhiteBoardViewModel.WBTools.NewEllipse:
                            toRender = WBOps.CreateEllipse(C_strt, C_end, 2, stroke, shapeId: null, shapeComp: true);
                            //setting the rendered-to-be shape fill according to the Canvas background
                            break;
                        case WhiteBoardViewModel.WBTools.NewRectangle:
                            toRender = WBOps.CreateRectangle(C_strt, C_end, 2, stroke, shapeId: null, shapeComp: true);
                            //setting the rendered-to-be shape fill according to the Canvas background
                            break;
                    }
                    //Removing temporary render from Canvas as Whiteboard module sends only Create operation, so we need to clean up temporary render
                    cn.Children.Remove(underCreation);

                    if (!(toRender == null || toRender.Count() == 0))
                    {
                        cn = RenderUXElement(toRender, cn, WBOps);

                        //select the shape 
                        SelectShape(cn, toRender.ElementAt(0).WindowsShape, WBOps, 0);
                    }
                }
            }

            return cn;
        }


        /// <summary>
        /// Translate the shape according to input events  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> The updated Canvas </returns>
        public Canvas MoveShape(Canvas cn, IWhiteBoardOperationHandler WBOps, Point strt, Point end, Shape mouseDownSh, bool shapeComp)
        {

            if (mouseDownSh == null || selectedShapes.Count() == 0)
            {
                return cn;
            }
            else if (mouseDownSh != null && !selectedShapes.Contains(mouseDownSh.Uid))
            {
                /*if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    
                }*/
                cn = UnselectAllBB(cn, WBOps);
                cn = SelectShape(cn, mouseDownSh, WBOps, 1);
            }
            Trace.WriteLine("Beginning moving shape with Uid" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() + "to end point " + end.ToString());
            Trace.WriteLine("List of Uids of selected shapes affected by move:" + selectedShapes.ToString());

            List<UXShape> toRender;

            if (shapeComp != true || testing == true)
            {


                //if(!testing) Debug.Assert(selectedShapes.Count == 1);
                string shUID = selectedShapes[0];

                /* Temporary WB Module code to test functionality */
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                //Check Condition 
                //if (!testing) Debug.Assert(iterat.Count() == 1);

                Shape sh = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

                double topleft_x = (double)Canvas.GetLeft(iterat.ToList()[0]);
                double topleft_y = (double)Canvas.GetTop(iterat.ToList()[0]);

                //MessageBox.Show("Entered MoveShape event");
                //MessageBox.Show(topleft_x.ToString(), topleft_y.ToString());

                double diff_topleft_x = (double)strt.X - (double)end.X;
                double diff_topleft_y = (double)strt.Y - (double)end.Y;
                double center_x, center_y;

                if (sh is not System.Windows.Shapes.Line)
                {
                    center_x = (double)(topleft_x - diff_topleft_x + sh.Width / 2);
                    center_y = (double)(topleft_y - diff_topleft_y + sh.Height / 2);


                    if (center_x > 0 && center_x < cn.Width)
                    {
                        selectMouseStuck.X = end.X;
                        Canvas.SetLeft(sh, topleft_x - diff_topleft_x);
                    }
                    else Canvas.SetLeft(sh, Canvas.GetLeft(sh));

                    if (center_y > 0 && center_y < cn.Height)
                    {
                        selectMouseStuck.Y = end.Y;
                        Canvas.SetTop(sh, topleft_y - diff_topleft_y);
                    }
                    else Canvas.SetTop(sh, Canvas.GetTop(sh));
                }
                else
                {
                    center_x = (double)(Canvas.GetLeft(sh) - diff_topleft_x + +((System.Windows.Shapes.Line)sh).X2 / 2);
                    center_y = (double)(Canvas.GetTop(sh) - diff_topleft_y + ((System.Windows.Shapes.Line)sh).Y2 / 2);

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
                string shUID = selectedShapes[0];

                /* Temporary WB Module code to test functionality */
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                //Check Condition 
                //if (!testing) Debug.Assert(iterat.Count() == 1);

                Shape sh = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

                double topleft_x = (double)Canvas.GetLeft(iterat.ToList()[0]);
                double topleft_y = (double)Canvas.GetTop(iterat.ToList()[0]);

                //MessageBox.Show("Entered MoveShape event");
                //MessageBox.Show(topleft_x.ToString(), topleft_y.ToString());

                double diff_topleft_x = (double)selectMouseStuck.X - (double)end.X;
                double diff_topleft_y = (double)selectMouseStuck.Y - (double)end.Y;
                double center_x, center_y;

                if (sh is not System.Windows.Shapes.Line)
                {
                    center_x = (double)(topleft_x - diff_topleft_x + sh.Width / 2);
                    center_y = (double)(topleft_y - diff_topleft_y + sh.Height / 2);


                    if (center_x > 0 && center_x < cn.Width)
                    {
                        selectMouseStuck.X = end.X;
                    }

                    if (center_y > 0 && center_y < cn.Height)
                    {
                        selectMouseStuck.Y = end.Y;
                    }
                }
                else
                {
                    center_x = (double)(Canvas.GetLeft(sh) - diff_topleft_x + +((System.Windows.Shapes.Line)sh).X2 / 2);
                    center_y = (double)(Canvas.GetTop(sh) - diff_topleft_y + ((System.Windows.Shapes.Line)sh).Y2 / 2);

                    if (center_x > 0 && center_x < cn.Width)
                    {
                        selectMouseStuck.X = end.X;
                    }

                    if (center_y > 0 && center_y < cn.Height)
                    {
                        selectMouseStuck.Y = end.Y;
                    }

                }


                Coordinate C_strt = new Coordinate(((int)(cn.Height - selectMouseDownPos.Y)), ((int)selectMouseDownPos.X));
                //Coordinate C_end = new Coordinate(((int)(cn.Height - end.Y)), ((int)end.X));
                Coordinate C_end = new Coordinate(((int)(cn.Height - selectMouseStuck.Y)), ((int)selectMouseStuck.X));
                

                toRender = new List<UXShape>();
                toRender = WBOps.TranslateShape(C_strt, C_end, mouseDownSh.Uid, shapeComp: true);

                //removing the local temporary render and only acknowledging the Create UXShape request as we cleaned up temporary render
                cn = UnselectAllBB(cn, WBOps);
                //cn.Children.Remove(sh);
                //Since we are removing rendered temporary shape above and toRender[1] corresponds to Create operation
                cn = RenderUXElement(toRender, cn, WBOps);
                if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps, 0);

                //Bugged, adr.ClipEnabled gives NullException??
                //cn = SelectShape(cn, toRender[0].WindowsShape, WBOps);

                Trace.WriteLine("Sent move request to the client for the shape with Uid:" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() +
                "to end point " + end.ToString() + ", where list of Uids of selected shapes are:" + selectedShapes.ToString() + "with shapeComp = ", shapeComp.ToString());
            }


            return cn;
        }

        /// <summary>
        /// Rotate the selected shape by input degrees  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> The updated Canvas </returns>

        public Canvas RotateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, Point strt, Point end, Shape mouseDownSh, bool shapeComp)
        {

            if (mouseDownSh == null || selectedShapes.Count() == 0)
            {
                return cn;
            }
            else if (mouseDownSh != null && !selectedShapes.Contains(mouseDownSh.Uid))
            {
                /*if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    
                }*/
                cn = UnselectAllBB(cn, WBOps);
                cn = SelectShape(cn, mouseDownSh, WBOps, 1);
            }
            Trace.WriteLine("Beginning rotating shape with Uid" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() + "to end point " + end.ToString());
            Trace.WriteLine("List of Uids of selected shapes affected by rotate:" + selectedShapes.ToString());

            List<UXShape> toRender;

            //UNCOMMENT LATER
            /*lock (this)
            {
                toRender = WBOps.RotateShape(C_strt, C_end, shpUID, shapeComp);
                cn = this.RenderUXElement(toRender, cn);
            }*/
            /* Temporary WB Module code to test functionality */
            string shUID = selectedShapes[0];

            IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);
            Shape sh = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

            //Check Condition 
            //if (!testing) Debug.Assert(iterat.Count() == 1);

            if (sh is System.Windows.Shapes.Line)
            {
                System.Windows.Shapes.Line lin = (System.Windows.Shapes.Line)sh;
                int topleft_x = (int)Canvas.GetLeft(lin);
                int topleft_y = (int)Canvas.GetTop(lin);
                int center_x = (int)(topleft_x + lin.X2 / 2);
                int center_y = (int)(topleft_y + lin.Y2 / 2);

                Point strt_shifted = new Point(strt.X - center_x, strt.Y - center_y);
                Point end_shifted = new Point(end.X - center_x, end.Y - center_y);

                double radians_strt = Math.Atan2(strt_shifted.Y, strt_shifted.X);
                double angle_strt = radians_strt * (180 / Math.PI);

                double radians_end = Math.Atan2(end_shifted.Y, end_shifted.X);
                double angle_end = radians_end * (180 / Math.PI);

                RotateTransform rt_prev = (RotateTransform)(sh.RenderTransform);

                float ang = (float)(angle_end - angle_strt);
                /*Code to find the angle made by start & end point on the center of the shape*/

                RotateTransform rotateTransform = new RotateTransform
                {
                    Angle = ang + (float)rt_prev.Angle,
                    CenterX = (lin.X2 / 2), //topleft_x,
                    CenterY = (lin.Y2 / 2) //topleft_y
                };
                sh.RenderTransform = rotateTransform;
                //sh = (Shape)lin;

            }
            else
            {

                /*Code to find the angle made by start & end point on the center of the shape*/
                int topleft_x = (int)Canvas.GetLeft(sh);
                int topleft_y = (int)Canvas.GetTop(sh);
                int center_x = (int)(topleft_x + sh.Width / 2);
                int center_y = (int)(topleft_y + sh.Height / 2);

                Point strt_shifted = new Point(strt.X - center_x, strt.Y - center_y);
                Point end_shifted = new Point(end.X - center_x, end.Y - center_y);

                double radians_strt = Math.Atan2(strt_shifted.Y, strt_shifted.X);
                double angle_strt = radians_strt * (180 / Math.PI);

                double radians_end = Math.Atan2(end_shifted.Y, end_shifted.X);
                double angle_end = radians_end * (180 / Math.PI);


                RotateTransform rt_prev = (RotateTransform)(sh.RenderTransform);

                float ang = (float)(angle_end - angle_strt);
                /*Code to find the angle made by start & end point on the center of the shape*/

                RotateTransform rotateTransform = new RotateTransform
                {
                    Angle = ang + (float)rt_prev.Angle,
                    CenterX = (sh.Width / 2), //topleft_x,
                    CenterY = (sh.Height / 2) //topleft_y
                };
                sh.RenderTransform = rotateTransform;
                //MessageBox.Show(ang.ToString(), ((int)rt_prev.Angle).ToString());
                /* Temporary WB Module code to test functionality */

            }

            //Necessary step to synchronize borders on rotation of selected shapes
            //cn = SyncBorders(cn, WBOps, sh);


            if (shapeComp == true)
            {
                cn = UnselectAllBB(cn, WBOps);
                //cn.Children.Remove(sh);

                Coordinate C_strt = new Coordinate(((int)(cn.Height - selectMouseDownPos.Y)), ((int)selectMouseDownPos.X));
                Coordinate C_end = new Coordinate(((int)(cn.Height - end.Y)), ((int)end.X));

                toRender = new List<UXShape>();
                toRender = WBOps.RotateShape(C_strt, C_end, mouseDownSh.Uid, shapeComp: true);

                //Since we already removed our side of temporary render, Delete operation by WB module is not acknowledged, whereas toRender[1] refers to necessary Create operation with the updated shape
                cn = RenderUXElement(toRender, cn, WBOps);
                if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps, 0);

                //Bugged as adr.isClipEnabled gives Null Exception
                //cn = SelectShape(cn, toRender[0].WindowsShape, WBOps);

                Trace.WriteLine("Sent rotate request to the client for the shape with Uid:" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() +
                "to end point " + end.ToString() + ", where the list of Uids of selected shapes are:" + selectedShapes.ToString() + "with shapeComp = ", shapeComp.ToString());
            }


            return cn;
        }

        /// <summary>
        /// Duplicate the selected shape by input degrees  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the duplicated shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="strokeWidth"> Float determining the thickness of border of drawn shape (OR) thickness of the stroke in freehand drawing </param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// <param name="offs_x"> Float determining the fill color of the drawn shape </param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// 
        /// <returns> The Updated Canvas </returns>
        public Canvas DuplicateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, int offs_x = 10, int offs_y = 10)
        {
            Point strt, end;

            if(selectedShapes == null || selectedShapes.Count == 0)
            {
                return cn;
            }

            string shUID = selectedShapes[0];

            IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);
            Shape sh = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

            //Check Condition 
            //if (!testing) Debug.Assert(iterat.Count() == 1);

            cn = UnselectAllBB(cn, WBOps);

            switch (sh)
            {
                case System.Windows.Shapes.Line:

                    strt.X = Canvas.GetLeft(sh) + 20;
                    strt.Y = Canvas.GetTop(sh);

                    end.X = Canvas.GetLeft(sh) + ((System.Windows.Shapes.Line)sh).X2 + 20;
                    end.Y = Canvas.GetTop(sh) + ((System.Windows.Shapes.Line)sh).Y2;

                    cn = CreateShape(cn, WBOps, WhiteBoardViewModel.WBTools.NewLine, strt, end, shapeComp: true);
                    break;
                case System.Windows.Shapes.Rectangle:
                    strt.X = Canvas.GetLeft(sh) + offs_x;
                    strt.Y = Canvas.GetTop(sh) + offs_y;

                    end.X = Canvas.GetLeft(sh) + sh.Width + offs_x;
                    end.Y = Canvas.GetTop(sh) + sh.Height + offs_y;

                    cn = CreateShape(cn, WBOps, WhiteBoardViewModel.WBTools.NewRectangle, strt, end, shapeComp: true);
                    break;
                case System.Windows.Shapes.Ellipse:
                    strt.X = Canvas.GetLeft(sh) + offs_x;
                    strt.Y = Canvas.GetTop(sh) + offs_y;

                    end.X = Canvas.GetLeft(sh) + sh.Width + offs_x;
                    end.Y = Canvas.GetTop(sh) + sh.Height + offs_y;

                    cn = CreateShape(cn, WBOps, WhiteBoardViewModel.WBTools.NewEllipse, strt, end, shapeComp: true);
                    break;
                default:
                    break;
            }

            return cn;
        }

        /// <summary>
        /// Render fetched shape updates on canvas  
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            if (shps == null || shps.Count() == 0) return cn;

            //UXShape has attribute
            foreach (UXShape shp in shps)
            {
                switch (shp.UxOperation)
                {
                    case (UXOperation.Create):

                        //Convert Radians from WB to Degrres for Render Transform 
                        int degrees = (int)(shp.AngleOfRotation * (180 / Math.PI));

                        //Setting the rendering such that bottom-right and top-left adorners adjust appropriately
                        if (degrees > 90) degrees -= 180;
                        else if (degrees < -90) degrees += 180;
                        shp.AngleOfRotation = degrees;

                        shp.AngleOfRotation = (-1) * degrees;

                        if (shp.WindowsShape is not System.Windows.Shapes.Line)
                        {

                            Canvas.SetLeft(shp.WindowsShape, (shp.TranslationCoordinate.C - shp.WindowsShape.Width / 2));
                            Canvas.SetTop(shp.WindowsShape, ((cn.Height - shp.TranslationCoordinate.R) - shp.WindowsShape.Height / 2));

                            //Since WB module stores the System.Windows.Shape.Width as height & System.Windows.Shape.Height as width?
                            /*double temp = shp.WindowsShape.Height;
                            shp.WindowsShape.Height = shp.WindowsShape.Width;
                            shp.WindowsShape.Width = temp;*/

                            //Setting the angular orientation of bounding box to be same as updated shape
                            RotateTransform rotateTransform = new RotateTransform
                            {
                                Angle = shp.AngleOfRotation,
                                CenterX = (shp.WindowsShape.Width / 2), //topleft_x,
                                CenterY = (shp.WindowsShape.Height / 2) //topleft_y
                            };
                            shp.WindowsShape.RenderTransform = rotateTransform;

                        }
                        else
                        {
                            System.Windows.Shapes.Line recv_Line = (System.Windows.Shapes.Line)shp.WindowsShape;

                            Shape ParsedLineUXElement = new System.Windows.Shapes.Line();
                            ((System.Windows.Shapes.Line)ParsedLineUXElement).X1 = 0;
                            ((System.Windows.Shapes.Line)ParsedLineUXElement).Y1 = 0;
                            ((System.Windows.Shapes.Line)ParsedLineUXElement).X2 = recv_Line.X2 - recv_Line.X1;
                            ((System.Windows.Shapes.Line)ParsedLineUXElement).Y2 = recv_Line.Y1 - recv_Line.Y2;
                            //((System.Windows.Shapes.Line)ParsedLineUXElement).RenderTransform = rotateTransform;
                            ParsedLineUXElement.Stroke = recv_Line.Stroke;
                            ParsedLineUXElement.StrokeThickness = recv_Line.StrokeThickness;
                            ParsedLineUXElement.Uid = recv_Line.Uid;
                            //Height = recv_Line.Width,
                            //Width = recv_Line.Height,

                            //Setting the angular orientation of bounding box to be same as updated shape
                            RotateTransform rotateTransform = new RotateTransform
                            {
                                Angle = shp.AngleOfRotation,
                                CenterX = (((System.Windows.Shapes.Line)ParsedLineUXElement).X2 / 2), //topleft_x,
                                CenterY = (((System.Windows.Shapes.Line)ParsedLineUXElement).Y2 / 2) //topleft_y
                            };
                            ((System.Windows.Shapes.Line)ParsedLineUXElement).RenderTransform = rotateTransform;

                            System.Windows.Shapes.Line transf_Line = (System.Windows.Shapes.Line)ParsedLineUXElement;

                            shp.WindowsShape = ParsedLineUXElement;

                            //NOT WORKING
                            //shp.WindowsShape.RenderTransform = rotateTransform;
                            Canvas.SetLeft(shp.WindowsShape, recv_Line.X1);
                            Canvas.SetTop(shp.WindowsShape, (cn.Height - recv_Line.Y1));
                        }

                        shp.WindowsShape.ToolTip = "User : " + getUserId(shp.WindowsShape, WBOps);
                        cn.Children.Add(shp.WindowsShape);
                        break;
                    case (UXOperation.Delete):

                        IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shp.WindowsShape.Uid);

                        //Check Condition that the shape to be deleted actually exists within the Canvas and has unique Uid
                        //if (!testing) Debug.Assert(iterat.Count() == 1);

                        cn.Children.Remove(iterat.ToList()[0]);
                        break;
                }

            }

            return cn;
        }

        /// <summary>
        /// Rotate the selected shape by input degrees  
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> The updated Canvas </returns>
        public Canvas DeleteShape(Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            Trace.WriteLine("List of Uids of selected shapes that are supposed to be deleted:", selectedShapes.ToString());
            List<UXShape> toRender;
            foreach (string shp in selectedShapes)
            {
                lock (this)
                {
                    toRender = WBOps.DeleteShape(shp);
                    cn = this.RenderUXElement(toRender, cn, WBOps);
                }
            }

            selectedShapes.Clear();

            Trace.WriteLine("Sent delete requests to the Client for the selected shapes with Uids:", selectedShapes.ToString());
            return cn;
        }

        /// <summary>
        /// Adjusts the finer attributes of selected shape, like Fill color, border thickness, border color etc
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="property"> Property of the selected shape that is to be changed, represented by string </param>
        /// <param name="hexCode"> Hexcode of the new fill/border color, used only if property = "Stroke" or "Fill" </param>
        /// <param name="thickness"> Floating point value representing the new thickness of the selected shape's border, used only if property = "StrokeThickness" </param>
        /// <returns> The updated Canvas </returns>
        public Canvas CustomizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, string property, string hexCode, float thickness = 0)
        {
            List<UXShape> toRender = new List<UXShape>();
            SolidColorBrush color = (SolidColorBrush)(new BrushConverter().ConvertFrom(hexCode));
            BoardColor color_b = new BoardColor(color.Color.R, color.Color.G, color.Color.B);

            //If no shape is selected while changing fill property, do not update Canvas
            if (selectedShapes.Count() == 0) return cn;

            string shUID = selectedShapes[0];

            IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

            //Check Condition 
            //if (!testing) Debug.Assert(iterat.Count() == 1);

            //Convert the UI element to Shape type 
            Shape sh = (Shape)iterat.ToList()[0];

            switch (property)
            {
                case "Stroke":
                    if (testing)
                    {
                        sh.Stroke = color;
                    }
                    else
                    {
                        toRender = WBOps.ChangeStrokeColor(color_b, sh.Uid);
                    }
                    break;
                case "StrokeThickness":
                    if (testing)
                    {
                        sh.StrokeThickness = thickness;
                    }
                    else
                    {
                        toRender = WBOps.ChangeStrokeWidth(thickness, sh.Uid);
                    }
                    break;
                case "Fill":

                    if (testing)
                    {
                        sh.Fill = color;
                    }
                    else
                    {
                        toRender = WBOps.ChangeShapeFill(color_b, sh.Uid);
                    }
                    break;
            }

            if (!testing)
            {
                cn = UnselectAllBB(cn, WBOps);
                //cn.Children.Remove(sh);

                //Add 
                cn = RenderUXElement(toRender, cn, WBOps);
                if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps, 0);
            }

            return cn;
        }

        /// <summary>
        /// Resizes the Adorner of the Shape adornedElement locally in the Visual Layer along with the shape, without sending update to the server of this change
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="adornedElement"> Shape instance that is to be resized </param>
        /// <param name="horizontalDrag"> Displacement of the cursor on drag in horizontal direction </param>
        /// <param name="verticalDrag"> Displacement of the cursor on drag in horizontal direction </param>
        /// <param name="corner"> Thumb primitive control representing the corner of the Shape that is dragged for resizing </param>
        /// <param name="pos"> AdornerDragPos enum type that represents the positive of the dragged corner of the Shape </param>
        /// <returns> The permissible Point that can be considered as `end` by the WBOps.ResizeShape function </returns>
        public Canvas ResizeAdorner(Canvas cn, IWhiteBoardOperationHandler WBOp, Shape adornedElement, double horizontalDrag, double verticalDrag, Thumb corner, AdornerDragPos pos)
        {

            double oldWidth, newWidth, oldHeight, newHeight;
            double oldLeft, newLeft;
            double oldTop, newTop;
            Point perm = new Point { X = 0, Y = 0 };

            Transform rt = adornedElement.RenderTransform;

            var mat = new Matrix();
            //mat.Rotate(rt.Angle);
            mat.Translate(Canvas.GetLeft(adornedElement) + adornedElement.Width / 2, Canvas.GetTop(adornedElement) + adornedElement.Height / 2);
            mat.Invert();

            Point drag = new Point { X = horizontalDrag, Y = verticalDrag };
            Point strt = new Point { X = (Canvas.GetLeft(adornedElement) + adornedElement.Width), Y = (Canvas.GetTop(adornedElement) + adornedElement.Height) };
            Point end = Point.Add(strt, (Vector)drag);

            Point transStrt = mat.Transform(strt);
            Point transEnd = mat.Transform(end);

            transStrt = rt.Inverse.Transform(transStrt);
            transEnd = rt.Inverse.Transform(transEnd);

            double deltaWidth = transEnd.X - transStrt.X;
            double deltaHeight = transEnd.Y - transStrt.Y;

            RotateTransform rt_angie = (RotateTransform)adornedElement.RenderTransform;
            float shapeAtAngle = (float)(rt_angie.Angle * (180 / Math.PI));

            if (adornedElement is System.Windows.Shapes.Line)
            {
                System.Windows.Shapes.Line adornedLine = (System.Windows.Shapes.Line)adornedElement;
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

                        double temp1 = Math.Max(adornedElement.Width + deltaWidth * 2, corner.DesiredSize.Width);
                        double temp2 = Math.Max(adornedElement.Height + deltaHeight * 2, corner.DesiredSize.Height);

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
                            double newTp = Canvas.GetTop(adornedElement) - deltaWidth * Math.Sin(shapeAtAngle);
                            Canvas.SetTop(adornedElement, newTp);
                            //set canvas left taking component of drag along width
                            double newLef = Canvas.GetLeft(adornedElement) - deltaWidth * Math.Cos(shapeAtAngle);
                            Canvas.SetLeft(adornedElement, newLef);
                        }
                        else if (newWidth == 10)
                        {
                            //set canvas top taking component of drag along height
                            double newTp = Canvas.GetTop(adornedElement) - deltaHeight * Math.Cos(shapeAtAngle);
                            Canvas.SetTop(adornedElement, newTp);
                            //set canvas left taking component of drag along height
                            double newLef = Canvas.GetLeft(adornedElement) - deltaHeight * Math.Sin(shapeAtAngle);
                            Canvas.SetLeft(adornedElement, newLef);
                        }
                        else
                        {
                            //set canvas top 
                            double newTp = Canvas.GetTop(adornedElement) - verticalDrag;
                            Canvas.SetTop(adornedElement, newTp);
                            //set canvas left
                            double newLef = Canvas.GetLeft(adornedElement) - horizontalDrag;
                            Canvas.SetLeft(adornedElement, newLef);
                        }

                        break;

                    //Top Left Corner 
                    case AdornerDragPos.TopLeft:
                        oldWidth = adornedElement.Width;
                        oldHeight = adornedElement.Height;

                        newWidth = Math.Max(Math.Max(adornedElement.Width - deltaWidth * 2, corner.DesiredSize.Width), 0);
                        newHeight = Math.Max(Math.Max(adornedElement.Height - deltaHeight * 2, corner.DesiredSize.Height), 0);

                        adornedElement.Width = newWidth;
                        adornedElement.Height = newHeight;

                        Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) + Math.Min(verticalDrag, adornedElement.Height / 2));
                        Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) + Math.Min(horizontalDrag, adornedElement.Width / 2));
                        break;
                }
            }
            return cn;
        }

        /// <summary>
        /// Request the server to resize the Shape shp when mouse is dragged from `strt` to `end` on the corner of `shp` represented by `pos`, only called when Resize operation is finalised, i.e, `shapeComp == true`, else ResizeAdorners is called.
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="pos"> AdornerDragPos enum type that represents the positive of the dragged corner of the Shape </param>
        /// <returns> The updated Canvas </returns>
        public Canvas ResizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, Shape shp, Point strt, Point end, AdornerDragPos pos)
        {
            if (selectedShapes.Count() == 0) return cn;

            Coordinate C_strt = new Coordinate(((int)(cn.Height - strt.Y)), ((int)strt.X));
            Coordinate C_end = new Coordinate(((int)(cn.Height - end.Y)), ((int)end.X));

            List<UXShape> toRender;

            DragPos drgPos = DragPos.None;
            switch (pos)
            {
                case AdornerDragPos.BotRight:
                    drgPos = DragPos.BottomRight;
                    break;
                case AdornerDragPos.TopLeft:
                    drgPos = DragPos.TopLeft;
                    break;
            }

            cn = UnselectAllBB(cn, WBOps);
            //cn.Children.Remove(shp);

            toRender = WBOps.ResizeShape(C_strt, C_end, shp.Uid, drgPos, true);
            cn = this.RenderUXElement(toRender, cn, WBOps);
            if (toRender != null && toRender.Count() == 2) cn = SelectShape(cn, toRender[1].WindowsShape, WBOps, 0);

            return cn;
        }


        public string getUserId(Shape sh, IWhiteBoardOperationHandler WBOps)
        {
            return WBOps.GetUserName(sh.Uid);
        }

    }

    /// <summary>
    /// Class to manage existing and new FreeHand instances by providing various methods by aggregating WhiteBoard Module    
    /// </summary>
    public class FreeHand
    {
        private System.Windows.Shapes.Polyline poly;
        private SolidColorBrush polyLineColor;
        private float polyLineThickness;
        string assgn_uid;
        Coordinate prev;
        Coordinate C_end;
        private bool testing;

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
            polyLineColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(hexCode));
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
        /// Render FreeHand instances shape updates on canvas  
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            if (shps == null || shps.Count() == 0) return cn;

            //UXShape has attribute
            foreach (UXShape shp in shps)
            {
                switch (shp.UxOperation)
                {
                    case (UXOperation.Create):
                        cn.Children.Add(shp.WindowsShape);
                        break;
                    case (UXOperation.Delete):
                        IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shp.WindowsShape.Uid);

                        cn.Children.Remove(iterat.ToList()[0]);
                        break;
                }
            }
            return cn;
        }

        /// <summary>
        /// ViewModel method to draw PolyLine/Eraser requested by the user
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="pt"> New point to be added into the PolyLine PointCollection </param>
        /// <param name="creation"> Boolean which is true if the first MouseDown event occured, i.e, user has just started to draw the new polyline</param>
        /// <param name="strokeColor"> Represents the hexcode of the color of polyline to be drawn </param>
        /// <param name="isEraser"> Boolean which is true if the drawn polyline is supposed to be an Eraser instance, used to set the Windows.Shapes.Tag property which is used by 'ChangeWbBackground' method locally</param>
        /// <returns> The updated Canvas </returns>
        public Canvas DrawPolyline(Canvas cn, IWhiteBoardOperationHandler WBOps, Point pt, bool creation = false, bool isEraser = false, bool shapeComp = false)
        {
            BoardColor stroke = new BoardColor(polyLineColor.Color.R, polyLineColor.Color.G, polyLineColor.Color.B);

            //SERVER REQUEST TO Create FINAL SHAPE
            List<UXShape> toRender = new List<UXShape>();

            if (creation)
            {
                poly = new System.Windows.Shapes.Polyline();

                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == "-1");
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

                if (isEraser == true)
                {
                    poly.Tag = "ERASER";

                }
                else
                {
                    poly.Tag = "FREEHAND";

                    if (!testing)
                    {
                        C_end = new Coordinate((float)pt.X, (float)pt.Y);
                        toRender = WBOps.CreatePolyline(C_end, C_end, polyLineThickness, stroke, shapeId: null, shapeComp: false);
                        prev = C_end;
                        if (!(toRender == null || toRender.Count == 0))
                        {
                            assgn_uid = toRender[0].WindowsShape.Uid;
                        }
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
                    if (isEraser == true)
                    {
                        poly.Points.Add(pt);

                        if (poly.Points.Count > 40)
                        {

                            poly.Points.RemoveAt(0);
                        }
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
                                    C_end = new Coordinate((float)pt.X, (float)pt.Y);
                                    toRender = WBOps.CreatePolyline(prev, C_end, polyLineThickness, stroke, shapeId: assgn_uid, shapeComp: false);
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
                if (isEraser) cn.Children.Remove(poly);
                else
                {
                    //Sending final point with shapeComp=true
                    if (assgn_uid != "-1")
                    {
                        if (!testing)
                        {
                            C_end = new Coordinate((float)poly.Points.Last().X, (float)poly.Points.Last().Y);
                            toRender = WBOps.CreatePolyline(C_end, C_end, polyLineThickness, stroke, shapeId: assgn_uid, shapeComp: true);

                            //Since the WBOps.CreatePolyline sends render requests of form Delete then Create,
                            //we choose to ignore Delete as we are doing temporary rendering
                            System.Windows.Shapes.Polyline pl = (System.Windows.Shapes.Polyline)toRender[1].WindowsShape;

                            //Removing temporary render from Canvas
                            cn.Children.Remove(poly);

                            if (!(toRender == null || toRender.Count() == 0))
                            {
                                //Adjusting the polyline render request to the user preference during Create Polyline operation
                                ((System.Windows.Shapes.Polyline)(toRender.ElementAt(1).WindowsShape)).StrokeLineJoin = PenLineJoin.Round;
                                ((System.Windows.Shapes.Polyline)(toRender.ElementAt(1).WindowsShape)).StrokeDashCap = PenLineCap.Round;

                                //Rendering the Polyline onto the Canvas
                                cn = RenderUXElement(new List<UXShape> { toRender[1] }, cn, WBOps);
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

        public Canvas DeletePolyline(Canvas cn, IWhiteBoardOperationHandler WBOps, System.Windows.Shapes.Polyline selectedLine)
        {
            //Call : Render UX element to delete the polyline 
            if (!testing)
            {
                List<UXShape> toRender = WBOps.DeleteShape(selectedLine.Uid);
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
    /// View Model of Whiteboard in MVVM design pattern 
    /// </summary>
    public class WhiteBoardViewModel : IClientBoardStateListener, INotifyPropertyChanged, IClientSessionNotifications, IWhiteBoardUpdater
    {
        //To be bound to the number of "Checkpoint #n" in Restore Checkpoint dropdown in Whiteboard.xaml
        private int _numCheckpoints;
        //private List<string> ckptList;
        public ObservableCollection<string> _chk;
        private bool testing;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> chckList
        {
            get
            {
                return _chk;
            }
            set
            {

                int n = this.NumCheckpoints;
                ObservableCollection<string> temp = new ObservableCollection<string>();
                for (int i = 0; i < n; i++)
                {
                    temp.Add("Checkpoint #" + (i + 1));
                }
                _chk = temp;
                OnPropertyChanged(nameof(chckList));
            }
        }

        public int NumCheckpoints
        {
            get { return _numCheckpoints; }
            set
            {
                if (_numCheckpoints != value)
                {
                    _numCheckpoints = value;
                    OnPropertyChanged(nameof(_numCheckpoints));
                    
                    int n = _numCheckpoints;
                    
                    ObservableCollection<string> temp = new ObservableCollection<string>();
                    for (int i = 0; i < n; i++)
                    {
                        chckList.Add("Checkpoint #" + (i + 1));
                    }
                    chckList = temp;
                }
            }
        }

        /// UX sets this enum to different options when user clicks on the appropriate tool icon
        public enum WBTools 
        {
            Initial, /// Initialised value, never to be used again
            Selection,
            NewLine,
            NewRectangle,
            NewEllipse,
            Rotate,
            Move,
            Eraser,
            FreeHand
        };

        public System.Windows.Point start;
        public System.Windows.Point end;
        private WBTools activeTool;
        public ShapeManager shapeManager;
        public FreeHand freeHand;
        public Canvas GlobCanvas;
        private IClientBoardStateManager manager;

        private bool isSubscribedToWBState;

        public IWhiteBoardOperationHandler WBOps;

        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                Application.Current.Dispatcher :
                Dispatcher.CurrentDispatcher;

        private IUXClientSessionManager _modelDb;

        /// <summary>
        /// Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module  
        /// </summary>
        public WhiteBoardViewModel(Canvas GlobCanvas, bool testing = false)
        {
            this.activeTool = WBTools.Initial;
            this.GlobCanvas = GlobCanvas;

            this.testing = testing;
            if (!testing)
            {
                this.WBOps = new WhiteBoardOperationHandler(new Coordinate(((int)GlobCanvas.Height), ((int)GlobCanvas.Width)));
                this.manager = ClientBoardStateManager.Instance;
                _modelDb = SessionManagerFactory.GetClientSessionManager();
                _modelDb.SubscribeSession(this);
            }

            this.shapeManager = new ShapeManager(testing : this.testing);
            this.freeHand = new FreeHand(testing: this.testing);

            this._numCheckpoints = 0;
            this._chk = new ObservableCollection<string>();

            //Initially not subscribed to WBStateManager
            if (!testing)
            {
                this.isSubscribedToWBState = false;

                //Canvas initialised as non-responsive until FetchState requests are fully completed
                this.GlobCanvas.IsEnabled = false;
            }
        }

        /// <summary>
        /// Listenes to the updates to the sesssion, used as a trigger event to subscribe to the Board State Manager
        /// </summary>
        public void OnClientSessionChanged(SessionData session)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<SessionData>((session) =>
                        {
                            lock (this)
                            {
                                if (!this.isSubscribedToWBState)
                                {
                                    //When this notification occurs, Dashboard has initialised the WhiteboardStateManager properly and we can finally subscribe to it
                                    //It is essential that this .Subscribe() method is only called once for a Whiteboard UX ViewModel
                                    this.manager.Subscribe(this, "whiteboard");
                                    this.GlobCanvas.IsEnabled = false;
                                    this.isSubscribedToWBState = true;
                                }
                            }
                        }),
                        session);
        }




        /// <summary>
        /// Changes the Background color of Canvas in View 
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="hexCode"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> The updated Canvas </returns>
        public void ChangeWbBackground(String WbhexCode)
        {
            this.GlobCanvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(WbhexCode));

            return;
        }
        /// <summary>
        /// Update the activeTool based on selected function on Toolbar 
        /// </summary>
        /// <param name="clickedTool"> Defines the tool on which the user clicked to be used to set the 'activeTool' enum accordingly </param>
        /// <returns> void, upon altering the 'activeTool' of this class instance accordingly </returns>

        public void ChangeActiveTool(string clickedTool)
        {
            switch (clickedTool)
            {
                case ("PenTool"):
                    this.activeTool = WBTools.FreeHand;
                    break;
                case ("LineTool"):
                    this.activeTool = WBTools.NewLine;
                    break;
                case ("RectTool"):
                    this.activeTool = WBTools.NewRectangle;
                    break;
                case ("EllipseTool"):
                    this.activeTool = WBTools.NewEllipse;
                    break;
                case ("SelectTool"):
                    this.activeTool = WBTools.Selection;
                    break;
                case ("EraseTool"):
                    this.activeTool = WBTools.Eraser;
                    break;
            }
            return;

        }


        /// <summary>
        /// Sets the `selectMouseDownPos` variable necessary for sending final Create/Move/Rotate requests to the server
        /// </summary>
        public void setSelectMouseDownPos(System.Windows.Point pnt)
        {
            this.shapeManager.selectMouseDownPos = pnt;
            return;
        }

        /// <summary>
        /// Returns the Active Tool enum that is currently selected
        /// </summary>
        public WBTools GetActiveTool()
        {
            return activeTool;
        }

        /// <summary>
        /// Changes the Privilege level of the current user  
        /// </summary>
        public void ChangeActivityState()
        {
            WBOps.SwitchState();
            return;
        }

        /// <summary>
        /// Function that should be called only when user accepts warning of Clear Whiteboard 
        /// </summary>
        public Canvas ClearCanvas(Canvas cn)
        {
            if(this.shapeManager.selectedShapes.Count() > 0) cn = shapeManager.UnselectAllBB(cn, WBOps);
            //cn.Children.Clear();

            //Only calling state manager's clear whiteboard here, whiteboard would be cleared when request received in the listener thread
            bool success = manager.ClearWhiteBoard();
            if (success) cn.IsEnabled = false;
            return cn;
        }

        /// <summary>
        /// Checkpoints the drawn shapes on canvas  
        /// </summary>
        public void SaveFrame()
        {
            manager.SaveCheckpoint();
        }

        public Canvas DeleteShape(Canvas cn)
        {
            cn = this.shapeManager.DeleteShape(cn, WBOps);
            return cn;
        }

        /// <summary>
        /// Restores the selected checkpoint, should be called only when user accepts warning of Restore Whiteboard
        /// </summary>
        public void RestoreFrame(int CheckNum, Canvas GlobCanvas)
        {
            //Unselecting all before clearing canvas 
            if (this.shapeManager.selectedShapes.Count > 0) this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);

            //The user should not be able to draw anything on the Canvas while the checkpoint is being fetched
            GlobCanvas.IsEnabled = false;

            manager.FetchCheckpoint(CheckNum);
        }

        /// <summary>
        //// Listens to the updates from the board state manager and serves the requests accordingly
        /// </summary>
        public void OnUpdateFromStateManager(List<UXShapeHelper> ServerUpdate)
        {

            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                  DispatcherPriority.Normal,
                  new Action<List<UXShapeHelper>>((ServerUpdate) =>
                  {
                      lock (this.shapeManager) lock (this.freeHand)
                      {
                          processServerUpdateBatch(ServerUpdate);
                      }
                  }

              ),
              ServerUpdate);
        }

        /// <summary>
        //// Unselects the selected shapes and sends an Undo Request to the board state manager, who sends the appropriate series of render requests received through the `OnUpdateFromStateManager`
        /// </summary>
        public void sendUndoRequest()
        {
            if(this.shapeManager.selectedShapes.Count > 0) this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);

            List<UXShape> renderUndo = WBOps.Undo();
            for (int i = 0; i < renderUndo.Count(); i++)
            {
                if (renderUndo[i].WindowsShape is System.Windows.Shapes.Polyline)
                {
                    this.GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { renderUndo[i] }, GlobCanvas, WBOps);
                }
                else
                {
                    this.GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { renderUndo[i] }, GlobCanvas, WBOps);
                }
            }
            return;
        }

        /// <summary>
        //// Unselects the selected shapes and sends a Redo Request to the board state manager, who sends the appropriate series of render requests received through the `OnUpdateFromStateManager`
        /// </summary>
        public void sendRedoRequest()
        {
            if (this.shapeManager.selectedShapes.Count > 0) this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);

            List<UXShape> renderUndo = WBOps.Redo();
            for (int i = 0; i < renderUndo.Count(); i++)
            {
                if (renderUndo[i].WindowsShape is System.Windows.Shapes.Polyline)
                {
                    this.GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { renderUndo[i] }, GlobCanvas, WBOps);
                }
                else
                {
                    this.GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { renderUndo[i] }, GlobCanvas, WBOps);
                }
            }
            return;
        }


        /// <summary>
        //// Performs the appropriate update to the UI when new checkpoint count from server is received
        /// </summary>
        public void increaseCheckpointNum(int latestNumCheckpoints)
        {
            if (latestNumCheckpoints == this._numCheckpoints)
            {
                return;
            }
            else
            {
                //Changing the bound element to the 'Load Checkpoint' dropdown
                this.NumCheckpoints = latestNumCheckpoints;
                return;
            }
        }

        public void processServerUpdateBatch(List<UXShapeHelper> receivedHelper)
        {
            List<UXShape> received = UXShape.ToUXShape(receivedHelper);

            //WE ASSUME that an update batch can only have a single Clear Canvas request 
            IEnumerable<UXShape> iterat = received.OfType<UXShape>().Where(x => x.OperationType == Operation.ClearState);
            //if (testing) Debug.Assert(iterat.Count() == 1);

            //WE ASSUME that an update batch can only have either no FetchState requests, or all FetchState requests
            IEnumerable<UXShape> iterat2 = received.OfType<UXShape>().Where(x => x.OperationType == Operation.FetchState);
            //if (testing) Debug.Assert(iterat2.Count() == 0 || iterat2.Count() == received.Count());
            if (received[0].OperationType == Operation.FetchState)
            {
                if(this.shapeManager.selectedShapes.Count > 0) this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);
                //New user has joined, the 'numCheckpoints' was last updated in the ViewModel Constructor as 0
                //if (testing) Debug.Assert(_numCheckpoints == 0);

                //ASSUMING that the user sending the Clear Board request has already been SHOWN THE WARNING, we clear the Canvas
                GlobCanvas.Children.Clear();
                //if (testing) Debug.Assert(GlobCanvas.IsEnabled == false);
                //Supposed to update the "Restore Checkpoint" dropdown with CheckPointNumber number of dropdown tiles
                increaseCheckpointNum(received[0].CheckPointNumber);
            }

            //WE ASSUME that an update batch can only have either no FetchCheckpoint requests, or all FetchCheckpoint requests
            IEnumerable<UXShape> iterat3 = received.OfType<UXShape>().Where(x => x.OperationType == Operation.FetchCheckpoint);
            //if (testing) Debug.Assert(iterat3.Count() == 0 || iterat3.Count() == received.Count());
            if (received[0].OperationType == Operation.FetchCheckpoint)
            {
                if(this.shapeManager.selectedShapes.Count > 0) this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);
                //ASSUMING THAT THE USER HAS ACCEPTED THE WARNING TO SAVE CHECKPOINT, SINCE ALL THE CHANGES MADE SINCE LAST CHECKPOINT WOULD BE LOST FOREVER
                GlobCanvas.Children.Clear();
                
            }

            for (int i = 0; i < received.Count(); i++)
            {
                switch (received[i].OperationType)
                {

                    //Case when new user joins and the whole state of server is sent to user
                    case Operation.FetchState:

                        ///
                        /// ASSUMING that this batch of server update contains ONLY AND ONLY Operation.FetchState requests
                        /// VERIFY THE ABOVE ASSUMPTION FROM ASHISH
                        ///
                        if (received[i].WindowsShape == null) {
                            this.GlobCanvas.IsEnabled = true;
                            continue; 
                        }
                        if (received[i].WindowsShape is System.Windows.Shapes.Polyline) GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);
                        else GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);

                        //Enabling the Canvas as all of the FetchState render requests in current batch have been rendered and the new user can now use Canvas
                        if (i == received.Count() - 1)  this.GlobCanvas.IsEnabled = true;

                        break;

                    case Operation.FetchCheckpoint:

                        if (received[i].WindowsShape is System.Windows.Shapes.Polyline) GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);
                        else GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);

                        //Enabling the Canvas as all of the FetchState render requests in current batch have been rendered and the new user can now use Canvas
                        if (i == received.Count() - 1) this.GlobCanvas.IsEnabled = true;
                        break;
                    case Operation.ClearState:
                        if(this.shapeManager.selectedShapes.Count > 0) this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);
                        //ASSUMING THAT THE USER HAS ACCEPTED THE WARNING TO CLEAR FRAME, SINCE ALL THE CHANGES MADE SINCE LAST CHECKPOINT WOULD BE LOST FOREVER
                        //based on above assumption that current server update batch can only have one ClearState request
 
                        GlobCanvas.Children.Clear();
                        //Enabling Canvas as it is now consistent with the server state
                        this.GlobCanvas.IsEnabled = true;

                        break;

                    case Operation.CreateCheckpoint:
                        increaseCheckpointNum(received[i].CheckPointNumber);
                        break;

                    case Operation.Create:
                        //If the operation is Create, directly render it onto the Canvas
                        if (received[i].WindowsShape == null) Trace.WriteLine("RenderUXElement received null");
                        else if(received[i].WindowsShape is System.Windows.Shapes.Polyline) GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);
                        else GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);

                        break;
                    case Operation.Delete:
                        //If a selected shape is to be delete by server request, unselect the shape first
                        if (this.shapeManager.selectedShapes.Count > 0 && this.shapeManager.selectedShapes.Contains(received[i].WindowsShape.Uid)) {
                            this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);
                        }
                        //If the operation is Delete, directly render it onto the Canvas
                        if (received[i].WindowsShape == null) Trace.WriteLine("RenderUXElement received null");
                        else if(received[i].WindowsShape is System.Windows.Shapes.Polyline) GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);
                        else GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);

                        break;
                    case Operation.Modify:
                        // If the operation is a deletion operation, unselect the shape before deletion
                        if (received[i].UxOperation == UXOperation.Delete && this.shapeManager.selectedShapes.Count > 0 && this.shapeManager.selectedShapes.Contains(received[i].WindowsShape.Uid))
                        {
                            this.GlobCanvas = this.shapeManager.UnselectAllBB(this.GlobCanvas, this.WBOps);
                        }
                        //If the operation is MODIFY, directly render it onto the Canvas
                        if (received[i].WindowsShape == null) Trace.WriteLine("RenderUXElement received null");
                        else if (received[i].WindowsShape is System.Windows.Shapes.Polyline) GlobCanvas = this.freeHand.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);
                        else GlobCanvas = this.shapeManager.RenderUXElement(new List<UXShape> { received[i] }, GlobCanvas, WBOps);

                        break;

                }
            }
        }

    }
}