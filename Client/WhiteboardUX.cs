using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;
using Whiteboard;
using System.Windows.Documents;

namespace Client
{


    /// <summary>
    /// Interface which listens to fetched server updates by IWhiteBoardState and local updates by IShapeOperation
    /// </summary>
    interface IWhiteBoardUpdater
    {
        /// <summary>
        /// Fetch updates from IWhiteBoardState for rendering in the view  
        /// </summary>
        abstract void FetchServerUpdates();

        /// <summary>
        /// Render fetched updates on canvas  
        /// </summary>
        abstract Canvas RenderUXElement(List<UXShape> shps, Canvas cn);
    }

    public class BorderAdorner : Adorner
    {
        //use thumb for resizing elements
        Thumb topLeft, topRight, bottomLeft, bottomRight;
        //visual child collection for adorner
        VisualCollection visualChilderns;
        ShapeManager shapeManager;
        Canvas cn;
        IWhiteBoardOperationHandler WbOp; 

        public BorderAdorner(UIElement element, ShapeManager shapeManager, Canvas cn, IWhiteBoardOperationHandler WbOp) : base(element)
        {
            visualChilderns = new VisualCollection(this);
            this.shapeManager = shapeManager;
            this.cn = cn;
            this.WbOp = WbOp; 

            //adding thumbs for drawing adorner rectangle and setting cursor
            BuildAdornerCorners(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorners(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorners(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorners(ref bottomRight, Cursors.SizeNWSE);

            //registering drag delta events for thumb drag movement
            topLeft.DragDelta += TopLeft_DragDelta;
            topRight.DragDelta += TopRight_DragDelta;
            bottomLeft.DragDelta += BottomLeft_DragDelta;
            bottomRight.DragDelta += BottomRight_DragDelta;
        }

        private void BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb bottomRightCorner = sender as Thumb;
            //setting new height and width after drag
            if (adornedElement != null && bottomRightCorner != null)
            {
                EnforceSize(adornedElement);

                //Coordinate strt = new Coordinate((int)Canvas.GetLeft(adornedElement), (int)Canvas.GetTop(adornedElement));
                //Coordinate end = new Coordinate((int)(Canvas.GetLeft(adornedElement) + e.HorizontalChange), (int)(Canvas.GetTop(adornedElement) + e.VerticalChange));
                //WbOp.ResizeShape()

                this.shapeManager.ResizeShape(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, bottomRightCorner, 0); 
                
                /*double oldWidth = adornedElement.Width;
                double oldHeight = adornedElement.Height;

                double newWidth = Math.Max(adornedElement.Width + e.HorizontalChange, bottomRightCorner.DesiredSize.Width);
                double newHeight = Math.Max(e.VerticalChange + adornedElement.Height, bottomRightCorner.DesiredSize.Height); 

                adornedElement.Width = newWidth;
                adornedElement.Height = newHeight;*/
            }
        }

        private void TopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb topRightCorner = sender as Thumb;
            //setting new height, width and canvas top after drag
            if (adornedElement != null && topRightCorner != null)
            {
                EnforceSize(adornedElement);

                this.shapeManager.ResizeShape(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, topRightCorner, 1);

                /*double oldWidth = adornedElement.Width;
                double oldHeight = adornedElement.Height;

                double newWidth = Math.Max(adornedElement.Width + e.HorizontalChange, topRightCorner.DesiredSize.Width);
                double newHeight = Math.Max(adornedElement.Height - e.VerticalChange, topRightCorner.DesiredSize.Height);
                adornedElement.Width = newWidth;

                double oldTop = Canvas.GetTop(adornedElement);
                double newTop = oldTop - (newHeight - oldHeight);
                adornedElement.Height = newHeight;
                Canvas.SetTop(adornedElement, newTop);*/
            }
        }

        private void TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb topLeftCorner = sender as Thumb;
            //setting new height, width and canvas top, left after drag
            if (adornedElement != null && topLeftCorner != null)
            {
                EnforceSize(adornedElement);

                this.shapeManager.ResizeShape(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, topLeftCorner, 2);

                /*double oldWidth = adornedElement.Width;
                double oldHeight = adornedElement.Height;

                double newWidth = Math.Max(adornedElement.Width - e.HorizontalChange, topLeftCorner.DesiredSize.Width);
                double newHeight = Math.Max(adornedElement.Height - e.VerticalChange, topLeftCorner.DesiredSize.Height);

                double oldLeft = Canvas.GetLeft(adornedElement);
                double newLeft = oldLeft - (newWidth - oldWidth);
                adornedElement.Width = newWidth;
                Canvas.SetLeft(adornedElement, newLeft);

                double oldTop = Canvas.GetTop(adornedElement);
                double newTop = oldTop - (newHeight - oldHeight);
                adornedElement.Height = newHeight;
                Canvas.SetTop(adornedElement, newTop);*/
            }
        }

        private void BottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb bottomLeftCorner = sender as Thumb;
            //setting new height, width and canvas left after drag
            if (adornedElement != null && bottomLeftCorner != null)
            {
                EnforceSize(adornedElement);

                this.shapeManager.ResizeShape(cn, WbOp, (Shape)adornedElement, e.HorizontalChange, e.VerticalChange, bottomLeftCorner, 3);

                /*double oldWidth = adornedElement.Width;
                double oldHeight = adornedElement.Height;

                double newWidth = Math.Max(adornedElement.Width - e.HorizontalChange, topRightCorner.DesiredSize.Width);
                double newHeight = Math.Max(adornedElement.Height + e.VerticalChange, topRightCorner.DesiredSize.Height);

                double oldLeft = Canvas.GetLeft(adornedElement);
                double newLeft = oldLeft - (newWidth - oldWidth);
                adornedElement.Width = newWidth;
                Canvas.SetLeft(adornedElement, newLeft);

                adornedElement.Height = newHeight;*/
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

            //arranging thumbs
            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desireWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desireWidth - adornerWidth / 2, desireHeight - adornerHeight / 2, adornerWidth, adornerHeight));

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
    public class ShapeManager : IWhiteBoardUpdater
    {

        public List<string> selectedShapes = new List<string>();
        private Dictionary<string, string> BBmap = new Dictionary<string, string>();
        public IWhiteBoardOperationHandler WBOps;
        private AdornerLayer adornerLayer;

        int counter = 0;

        public ShapeManager()
        {

        }

        /// <summary>
        /// Fetch shape updates from IWhiteBoardState for rendering in the view   
        /// </summary>
        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }


        public Canvas CreateSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp)
        {
            adornerLayer = AdornerLayer.GetAdornerLayer(sh);
            BorderAdorner adr = new BorderAdorner(sh,this,cn,WBOp);
            adr.IsClipEnabled = true;
            adornerLayer.Add(adr);
            return cn;
        }


        public Canvas DeleteSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp)
        {
            adornerLayer.Remove(adornerLayer.GetAdorners(sh)[0]);
            return cn;
        }

        public Canvas UnselectAllBB(Canvas cn, IWhiteBoardOperationHandler WBOp)
        {
            /*foreach (var item in BBmap.Keys)
            {
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);
                Shape sh = (iterat.ToList()[0]) as Shape;
                cn = DeleteSelectionBB(cn, sh, WBOp);
                selectedShapes.Remove(sh.Uid.ToString());
            }*/

            foreach (var item in selectedShapes)
            {
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);

                //Check Condition 
                Debug.Assert(iterat.Count() == 1);

                Shape sh = (iterat.ToList()[0]) as Shape;
                cn = DeleteSelectionBB(cn, sh, WBOp);
            }

            selectedShapes.Clear();

            return cn;
        }

        public Canvas DeleteSelectedShapes(Canvas cn, IWhiteBoardOperationHandler WBOp)
        {


            //remove shapes 
            foreach (var item in selectedShapes)
            {
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);

                //Check Condition 
                Debug.Assert(iterat.Count() == 1);

                Shape sh = (iterat.ToList()[0]) as Shape;
                cn.Children.Remove(sh);
            }

            //remove bounding boxes 
            cn = UnselectAllBB(cn, WBOp);

            return cn;
        }

        /// <summary>
        /// Handle input events for selection : this includes evnents for single shape selection and multiple shape selection  
        /// </summary>
        /// <param name="sh"> System.Windows.Shape instance to be selected </param>
        /// <param name="mode"> mode=0 if shape selected without Ctrl pressed, else mode=1 </param>
        /// <returns> void, upon altering the 'selectedShapes' of this class instane accordingly </returns>
        public Canvas SelectShape(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp, int mode = 0)
        {

            SolidColorBrush strokeColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            switch (mode)
            {
                //selectedShapes.ToString()
                //single shape selection case
                case 0:
                    Trace.WriteLine("Selection of shape with Uid = " + sh.Uid.ToString() + "requested by user with Ctrl NOT pressed");
                    //If selected shape is the selection box rectangle 
                    if (BBmap.ContainsValue(sh.Uid))
                    {
                        cn = UnselectAllBB(cn, WBOp);
                    }
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
                    else if (BBmap.ContainsValue(sh.Uid))
                    {
                        cn = UnselectAllBB(cn, WBOp);
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
        /// Sycronizes the border of selected shapes which are moved/rotated by user input or server updates
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOp"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shUID"> UID of Selected Shape in Canvas that is moved/rotated and needs to have updated dotted-selection boundary</param>
        /// <returns> Updated Canvas instance with the updated boundary of moved/rotated shape </returns>
        public Canvas SyncBorders(Canvas cn, IWhiteBoardOperationHandler WBOp, Shape sh)
        {
            //Finding bounding box
            //Shape bbox = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == bbUID).ToList()[0];
            //Setting the position of bounding box to be same as updated shape
            //Canvas.SetLeft(bbox, Canvas.GetLeft(sh));
            //Canvas.SetTop(bbox, Canvas.GetTop(sh));
            //Setting the angular orientation of bounding box to be same as updated shape
            //bbox.RenderTransform = sh.RenderTransform;

            adornerLayer = AdornerLayer.GetAdornerLayer(sh);

            BorderAdorner adr = new BorderAdorner(sh, this, cn, WBOp);
            adr.IsClipEnabled = true;
            
            adornerLayer.Add(adr);

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
        public Canvas CreateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, WhiteBoardViewModel.WBTools activeTool, Point strt, Point end, float strokeWidth = 1, string strokeColor = "#FFFFFF", string shapeId = null, bool shapeComp = false)
        {
            List<UXShape> toRender;
            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));

            SolidColorBrush strokeColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(strokeColor));
            BoardColor strk_clr = new BoardColor(strokeColorBrush.Color.R, strokeColorBrush.Color.G, strokeColorBrush.Color.B);


            if (end.X < 0 || end.Y < 0 || end.X > cn.Width || end.Y > cn.Height) MessageBox.Show("!!!!!");


            //If the mouse touches the border of the canvas, then shape is final
            if (end.X == 0 || end.Y == 0 || end.X == cn.Width || end.Y == cn.Height) shapeComp = true;

            switch (activeTool)
            {
                case WhiteBoardViewModel.WBTools.NewLine:
                    lock (this)
                    { 
                        Trace.WriteLine("User requested creation of a line with start = "+ strt.ToString() + "end = " + end.ToString());
                        //toRender = WBOps.CreateLine(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp); //return is of form List of UXShape
                        //cn = this.RenderUXElement(toRender, cn);
                    }
                    break;
                case WhiteBoardViewModel.WBTools.NewRectangle:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of a rectangle with start = " + strt.ToString() + "end = " + end.ToString());
                        //toRender = WBOps.CreateRectangle(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        //cn = this.RenderUXElement(toRender, cn);
                    }
                    break;
                case WhiteBoardViewModel.WBTools.NewEllipse:
                    lock (this)
                    {
                        Trace.WriteLine("User requested creation of an ellipse with start = " + strt.ToString() + "end = " + end.ToString());
                        //toRender = WBOps.CreateEllipse(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        //cn = this.RenderUXElement(toRender, cn);
                    }
                    break;
            }



            if (shapeComp == true) MessageBox.Show("start = " + strt.ToString() + ", end = " + end.ToString());
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

            if (mouseDownSh == null || BBmap.ContainsValue(mouseDownSh.Uid))
            {
                return cn;
            }
            else if (mouseDownSh != null && !selectedShapes.Contains(mouseDownSh.Uid))
            {
                if ( !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    cn = UnselectAllBB(cn, WBOps);
                }
                cn = SelectShape(cn, mouseDownSh, WBOps, 1);
            }
            Trace.WriteLine("Beginning moving shape with Uid" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() + "to end point " + end.ToString());
            Trace.WriteLine("List of Uids of selected shapes affected by move:" + selectedShapes.ToString());

            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));
            List<UXShape> toRender;

            lock (this)
            {
                foreach (string shUID in selectedShapes)
                {
                    //UNCOMMENT LATER
                    /*lock (this)
                    {
                        toRender = WBOps.TranslateShape(C_strt, C_end, shpUID, shapeComp);
                        cn = this.RenderUXElement(toRender, cn);
                    }*/

                    /* Temporary WB Module code to test functionality */             
                    IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                    //Check Condition 
                    Debug.Assert(iterat.Count() == 1);


                    Shape sh = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];
                    
                    int topleft_x = (int)Canvas.GetLeft(iterat.ToList()[0]);
                    int topleft_y = (int)Canvas.GetTop(iterat.ToList()[0]);

                    //MessageBox.Show("Entered MoveShape event");
                    //MessageBox.Show(topleft_x.ToString(), topleft_y.ToString());

                    int diff_topleft_x = (int)strt.X - (int)end.X;
                    int diff_topleft_y = (int)strt.Y - (int)end.Y;
                    int center_x = (int)(topleft_x - diff_topleft_x + sh.Width / 2);
                    int center_y = (int)(topleft_y - diff_topleft_y + sh.Height / 2);

                    if (sh is System.Windows.Shapes.Ellipse)
                    {
                        System.Windows.Shapes.Ellipse newEl = new System.Windows.Shapes.Ellipse();
                        newEl.Width = sh.Width;
                        newEl.Height = sh.Height;
                        newEl.Fill = sh.Fill;
                        newEl.Stroke = sh.Stroke;
                        newEl.StrokeThickness = sh.StrokeThickness;
                        newEl.Uid = sh.Uid;
                        newEl.RenderTransform = sh.RenderTransform; 
                        if (center_x > 0 && center_x < cn.Width) Canvas.SetLeft(newEl, topleft_x - diff_topleft_x);
                        else if (center_x > cn.Width) Canvas.SetLeft(newEl, Canvas.GetLeft(sh));
                        else Canvas.SetLeft(newEl, Canvas.GetLeft(sh));

                        if (center_y > 0 && center_y < cn.Height) Canvas.SetTop(newEl, topleft_y - diff_topleft_y);
                        else if (center_y > cn.Height) Canvas.SetTop(newEl, Canvas.GetTop(sh));
                        else Canvas.SetTop(newEl, Canvas.GetTop(sh));

                        cn.Children.Remove(sh);
                        cn.Children.Add(newEl);
                        sh = newEl;
                    }
                    else if (sh is System.Windows.Shapes.Rectangle)
                    {
                        System.Windows.Shapes.Rectangle newRec = new System.Windows.Shapes.Rectangle();
                        newRec.Width = sh.Width;
                        newRec.Height = sh.Height;
                        newRec.Fill = sh.Fill;
                        newRec.Stroke = sh.Stroke;
                        newRec.StrokeThickness = sh.StrokeThickness;
                        newRec.Uid = sh.Uid;
                        newRec.RenderTransform = sh.RenderTransform;
                        if (center_x > 0 && center_x < cn.Width) Canvas.SetLeft(newRec, topleft_x - diff_topleft_x);
                        else if (center_x > cn.Width) Canvas.SetLeft(newRec, cn.Width);
                        else Canvas.SetLeft(newRec, 0);

                        if (center_y > 0 && center_y < cn.Height) Canvas.SetTop(newRec, topleft_y - diff_topleft_y);
                        else if (center_y > cn.Height) Canvas.SetTop(newRec, cn.Height);
                        else Canvas.SetTop(newRec, 0);
                        cn.Children.Remove(sh);
                        cn.Children.Add(newRec);
                        sh = newRec;
                    }
                    else if (sh is System.Windows.Shapes.Line)
                    {
                        System.Windows.Shapes.Rectangle newLine = new System.Windows.Shapes.Rectangle();
                        newLine.Width = sh.Width;
                        newLine.Height = sh.Height;
                        newLine.Fill = sh.Fill;
                        newLine.Stroke = sh.Stroke;
                        newLine.StrokeThickness = sh.StrokeThickness;
                        newLine.Uid = sh.Uid;
                        newLine.RenderTransform = sh.RenderTransform;
                        if (center_x > 0 && center_x < cn.Width) Canvas.SetLeft(newLine, topleft_x - diff_topleft_x);
                        else if (center_x > cn.Width) Canvas.SetLeft(newLine, cn.Width);
                        else Canvas.SetLeft(newLine, 0);

                        if (center_y > 0 && center_y < cn.Height) Canvas.SetTop(newLine, topleft_y - diff_topleft_y);
                        else if (center_y > cn.Height) Canvas.SetTop(newLine, cn.Height);
                        else Canvas.SetTop(newLine, 0);
                        cn.Children.Remove(sh);
                        cn.Children.Add(newLine);
                        sh = newLine;
                    }


                    //MessageBox.Show(diff_topleft_x.ToString(), diff_topleft_y.ToString());

                    /*if (center_x > 0 && center_x < cn.Width)
                    {
                        Canvas.SetLeft(iterat.ToList()[0], topleft_x - diff_topleft_x);
                    }

                    if(center_y > 0 && center_y < cn.Height)
                    {
                        Canvas.SetTop(iterat.ToList()[0], topleft_y - diff_topleft_y);
                    }*/

                    //Necessary step to synchronize borders on rotation of selected shapes
                    cn = SyncBorders(cn, WBOps, sh);
                }               
            }
            Trace.WriteLine("Sent move request to the client for the shape with Uid:" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() +
                "to end point " + end.ToString() + ", where list of Uids of selected shapes are:" + selectedShapes.ToString() + "with shapeComp = ", shapeComp.ToString());
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

            if (mouseDownSh == null || BBmap.ContainsValue(mouseDownSh.Uid))
            {
                return cn;
            }
            else if (mouseDownSh != null && !selectedShapes.Contains(mouseDownSh.Uid))
            {
                if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    cn = UnselectAllBB(cn, WBOps);
                }
                cn = SelectShape(cn, mouseDownSh, WBOps, 1);
            }
            Trace.WriteLine("Beginning rotating shape with Uid" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() + "to end point " + end.ToString());
            Trace.WriteLine("List of Uids of selected shapes affected by rotate:" + selectedShapes.ToString());

            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));

            List<UXShape> toRender;
            foreach (string shUID in selectedShapes)
            {
                //UNCOMMENT LATER
                /*lock (this)
                {
                    toRender = WBOps.RotateShape(C_strt, C_end, shpUID, shapeComp);
                    cn = this.RenderUXElement(toRender, cn);
                }*/
                /* Temporary WB Module code to test functionality */
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                //Check Condition 
                Debug.Assert(iterat.Count() == 1);

                Shape sh = (Shape)cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID).ToList()[0];

                /*Code to find the angle made by start & end point on the center of the shape*/
                int topleft_x = (int)Canvas.GetLeft(sh);
                int topleft_y = (int)Canvas.GetTop(sh);
                int center_x = (int)(topleft_x + sh.Width/2);
                int center_y = (int)(topleft_y + sh.Height/2);

                Point strt_shifted = new Point(strt.X - center_x, strt.Y - center_y);
                Point end_shifted = new Point(end.X - center_x, end.Y - center_y);

                double radians_strt = Math.Atan2( strt_shifted.Y , strt_shifted.X );
                double angle_strt = radians_strt * (180 / Math.PI);

                double radians_end = Math.Atan2(end_shifted.Y, end_shifted.X);
                double angle_end = radians_end * (180 / Math.PI);


                int ang = (int)(angle_end - angle_strt);
                /*Code to find the angle made by start & end point on the center of the shape*/

                RotateTransform rotateTransform = new RotateTransform
                {
                    Angle = ang,
                    CenterX = (sh.Width / 2), //topleft_x,
                    CenterY = (sh.Height / 2) //topleft_y
                };
                sh.RenderTransform = rotateTransform;
                /* Temporary WB Module code to test functionality */


                //Necessary step to synchronize borders on rotation of selected shapes
                cn = SyncBorders(cn, WBOps, sh);
            }


            Trace.WriteLine("Sent rotate request to the client for the shape with Uid:" + mouseDownSh.Uid.ToString() + "from start point" + strt.ToString() + 
                "to end point " + end.ToString() + ", where the list of Uids of selected shapes are:" + selectedShapes.ToString() + "with shapeComp = ", shapeComp.ToString());
            
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
        /// <returns> The updated Canvas </returns>
        public Canvas DuplicateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, List<UXShape> shps, float strokeWidth, SolidColorBrush strokeColor, int offs_x = 10, int offs_y = 10)
        {
            BoardColor fill = new BoardColor(strokeColor.Color.R, strokeColor.Color.G, strokeColor.Color.B);

            foreach (UXShape shp in shps)
            {
                List<UXShape> toRender;
                switch (shp.ShapeIdentifier)
                {
                    case (Whiteboard.ShapeType.ELLIPSE):
                        lock (this)
                        {
                            System.Windows.Shapes.Ellipse eps = (System.Windows.Shapes.Ellipse)shp.WindowsShape;
                            int topleft_x = (int)Canvas.GetLeft(eps);
                            int topleft_y = (int)Canvas.GetTop(eps);
                            int bottomright_x = (int)(topleft_x + eps.Width);
                            int bottomright_y = (int)(topleft_y + eps.Height);
                            Coordinate strt = new Coordinate(topleft_x, topleft_y);
                            Coordinate end = new Coordinate(bottomright_x, bottomright_y);
                            toRender = WBOps.CreateEllipse(strt, end, strokeWidth, fill, eps.Uid, true);
                        }
                        break;
                    //CANNOT SELECT POLYLINE DUE TO DESIGN CONSTRAINTS
                    //case (Whiteboard.ShapeType.LINE):
                    //    break;
                    case (Whiteboard.ShapeType.RECTANGLE):
                        lock (this)
                        {
                            System.Windows.Shapes.Rectangle rect = (System.Windows.Shapes.Rectangle)shp.WindowsShape;
                            int topleft_x = (int)Canvas.GetLeft(rect);
                            int topleft_y = (int)Canvas.GetTop(rect);
                            int bottomright_x = (int)(topleft_x + rect.Width);
                            int bottomright_y = (int)(topleft_y + rect.Height);

                            Coordinate strt = new Coordinate(topleft_x, topleft_y);
                            Coordinate end = new Coordinate(bottomright_x, bottomright_y);

                            toRender = WBOps.CreateEllipse(strt, end, strokeWidth, fill, rect.Uid, true);
                            cn = this.RenderUXElement(toRender, cn);
                        }
                        break;
                    case (Whiteboard.ShapeType.LINE):
                        //TODO: Verify the working of this
                        lock (this)
                        {
                            System.Windows.Shapes.Line lin = (System.Windows.Shapes.Line)shp.WindowsShape;
                            int topleft_x = (int)Canvas.GetLeft(lin);
                            int topleft_y = (int)Canvas.GetTop(lin);
                            int bottomright_x = (int)(topleft_x + lin.Width);
                            int bottomright_y = (int)(topleft_y + lin.Height);

                            Coordinate strt = new Coordinate(topleft_x, topleft_y);
                            Coordinate end = new Coordinate(bottomright_x, bottomright_y);

                            toRender = WBOps.CreateEllipse(strt, end, strokeWidth, fill, lin.Uid, true);
                            cn = this.RenderUXElement(toRender, cn);
                        }
                        break;
                }
            }
            return cn;
        }

        /// <summary>
        /// Render fetched shape updates on canvas  
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn)
        {
            //UXShape has attribute
            foreach (UXShape shp in shps)
            {
                switch (shp.UxOperation)
                {
                    case (UXOperation.CREATE):
                        //Rendering the new shape with the appropriate geometry in terms of translation and rotation
                        Canvas.SetLeft(shp.WindowsShape, shp.TranslationCoordinate.C);
                        Canvas.SetTop(shp.WindowsShape, shp.TranslationCoordinate.R);
                        //Setting the angular orientation of bounding box to be same as updated shape
                        RotateTransform rotateTransform = new RotateTransform
                        {
                            Angle = shp.AngleOfRotation,
                            CenterX = (shp.WindowsShape.Width / 2), //topleft_x,
                            CenterY = (shp.WindowsShape.Height / 2) //topleft_y
                        };
                        shp.WindowsShape.RenderTransform = rotateTransform;
                        cn.Children.Add(shp.WindowsShape);
                        break;
                    case (UXOperation.DELETE):
                        IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shp.WindowsShape.Uid);

                        //Check Condition 
                        Debug.Assert(iterat.Count() == 1);

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
        /// <param name="shps"> Shapes that are to be deleted, expected to be 'selectedShapes' attribute of the aggregating ViewModel instance </param>
        /// <returns> The updated Canvas </returns>
        public Canvas DeleteShape(Canvas cn, IWhiteBoardOperationHandler WBOps, List<UXShape> shps)
        {
            Trace.WriteLine("List of Uids of selected shapes that are supposed to be deleted:", selectedShapes.ToString());
            List<UXShape> toRender;
            foreach (UXShape shp in shps)
            {
                lock (this)
                {
                    toRender = WBOps.DeleteShape(shp.WindowsShape.Uid);
                    cn = this.RenderUXElement(toRender, cn);
                }
            }
            Trace.WriteLine("Sent delete requests to the Client for the selected shapes with Uids:", selectedShapes.ToString());
            return cn;
        }

        /// <summary>
        /// To adjust finer attributes of selected shape, like Fill color/ Border width etc
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> Shapes that are to be altered, expected to be 'selectedShapes' attribute of the aggregating ViewModel instance </param>
        /// <returns> The updated Canvas </returns>
        public Canvas CustomizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, string property, string hexCode, float thickness)
        {
            List<UXShape> toRender;
            SolidColorBrush color = (SolidColorBrush)(new BrushConverter().ConvertFrom(hexCode));
            foreach (string shUID in selectedShapes)
            {

                //UNCOMMENT LATER
                /*lock (this)
                {
                    toRender = WBOps.RotateShape(C_strt, C_end, shpUID, shapeComp);
                    cn = this.RenderUXElement(toRender, cn);
                }*/
                /* Temporary WB Module code to test functionality */

                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shUID);

                //Check Condition 
                Debug.Assert(iterat.Count() == 1);

                //Convert the UI element to Shape type 
                Shape sh = (Shape)iterat.ToList()[0];

                switch (property)
                {
                    case "Stroke":
                        sh.Stroke = color;
                        break;
                    case "StrokeThickness":
                        sh.StrokeThickness = thickness;
                        break;
                    case "Fill":
                        sh.Fill = color; 
                        break;
                }
            }

            return cn; 
        }

        /// <summary>
        /// Resize the list of selected shapes 'selectedShapes' based on start and end points 
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> Shapes that are to be altered, expected to be 'selectedShapes' attribute of the aggregating ViewModel instance </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> The updated Canvas </returns>
        public Canvas ResizeShape(Canvas cn, IWhiteBoardOperationHandler WBOp, Shape adornedElement, double horizontalDrag, double verticalDrag, Thumb corner, int pos)
        {

            double oldWidth, newWidth, oldHeight, newHeight;
            double oldLeft, newLeft;
            double oldTop, newTop;

            /*Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));
            List<UXShape> toRender;
            foreach (UXShape shp in shps)
            {
                lock (this)
                {
                    toRender = WBOps.ResizeShape(C_strt, C_end, shp.WindowsShape.Uid, shapeComp);
                    cn = this.RenderUXElement(toRender, cn);
                }
            }
            return cn;*/

            switch (pos)
            {
                //Bottom Right Corner 
                case 0:
                    oldWidth = adornedElement.Width;
                    oldHeight = adornedElement.Height;

                    newWidth = Math.Max(adornedElement.Width + horizontalDrag, corner.DesiredSize.Width);
                    newHeight = Math.Max(verticalDrag + adornedElement.Height, corner.DesiredSize.Height);

                    adornedElement.Width = newWidth;
                    adornedElement.Height = newHeight;
                    break;
                //Top Right Corner 
                case 1:
                    oldWidth = adornedElement.Width;
                    oldHeight = adornedElement.Height;

                    newWidth = Math.Max(adornedElement.Width + horizontalDrag, corner.DesiredSize.Width);
                    newHeight = Math.Max(adornedElement.Height - verticalDrag, corner.DesiredSize.Height);
                    adornedElement.Width = newWidth;

                    oldTop = Canvas.GetTop(adornedElement);
                    newTop = oldTop - (newHeight - oldHeight);
                    adornedElement.Height = newHeight;
                    Canvas.SetTop(adornedElement, newTop);
                    break;
                //Top Left Corner 
                case 2:
                    oldWidth = adornedElement.Width;
                    oldHeight = adornedElement.Height;

                    newWidth = Math.Max(adornedElement.Width - horizontalDrag, corner.DesiredSize.Width);
                    newHeight = Math.Max(adornedElement.Height - verticalDrag, corner.DesiredSize.Height);

                    oldLeft = Canvas.GetLeft(adornedElement);
                    newLeft = oldLeft - (newWidth - oldWidth);
                    adornedElement.Width = newWidth;
                    Canvas.SetLeft(adornedElement, newLeft);

                    oldTop = Canvas.GetTop(adornedElement);
                    newTop = oldTop - (newHeight - oldHeight);
                    adornedElement.Height = newHeight;
                    Canvas.SetTop(adornedElement, newTop);
                    break;
                //Bottom Left Corner 
                case 3:
                    oldWidth = adornedElement.Width;
                    oldHeight = adornedElement.Height;

                    newWidth = Math.Max(adornedElement.Width - horizontalDrag, corner.DesiredSize.Width);
                    newHeight = Math.Max(adornedElement.Height + verticalDrag, corner.DesiredSize.Height);

                    oldLeft = Canvas.GetLeft(adornedElement);
                    newLeft = oldLeft - (newWidth - oldWidth);
                    adornedElement.Width = newWidth;
                    Canvas.SetLeft(adornedElement, newLeft);

                    adornedElement.Height = newHeight;
                    break;
            }
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
    public class FreeHand : IWhiteBoardUpdater
    {
        private System.Windows.Shapes.Polyline poly;
        private SolidColorBrush polyLineColor;
        private float polyLineThickness;

        //Consructor for the class 
        public FreeHand()
        {
            polyLineColor = new SolidColorBrush(Colors.Black);
            polyLineThickness = 5;
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
        /// Fetch FreeHand instances updates from IWhiteBoardState for rendering in the view   
        /// </summary>
        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Render FreeHand instances shape updates on canvas  
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn)
        {
            //Write implementation code
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
        public Canvas DrawPolyline(Canvas cn, IWhiteBoardOperationHandler WBOps, Point pt, bool creation = false, bool isEraser = false)
        {
            if (creation)
            {
                poly = new System.Windows.Shapes.Polyline();


                if (isEraser == true)
                {
                    poly.Tag = "ERASER";
                }
                else
                {
                    poly.Tag = "FREEHAND";
                }

                poly.Stroke = polyLineColor;
                poly.StrokeThickness = polyLineThickness;

                poly.Points.Add(pt);
                cn.Children.Add(poly);
            }
            else
            {
                poly.Points.Add(pt);
            }

            return cn;  
        }

        public Canvas CustomizePolyline(Canvas cn, IWhiteBoardOperationHandler WBOps)
        {
            return cn;
        }
    }

    /// <summary>
    /// View Model of Whiteboard in MVVM design pattern 
    /// </summary>
    public class WhiteBoardViewModel
    {

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

        public IWhiteBoardOperationHandler WBOps;

        /// <summary>
        /// Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module  
        /// </summary>
        public WhiteBoardViewModel(Canvas GlobCanvas)
        {
            this.shapeManager = new ShapeManager();
            this.freeHand = new FreeHand();
            this.activeTool = WBTools.Initial;
            this.WBOps = new WhiteBoardOperationHandler(new Coordinate(((int)GlobCanvas.Height), ((int)GlobCanvas.Width)));
        }



        /// <summary>
        /// Changes the Background color of Canvas in View 
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="hexCode"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <returns> The updated Canvas </returns>
        public Canvas ChangeWbBackground(Canvas cn, String WbhexCode)
        {
            cn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(WbhexCode));

            //Setting the color of the Eraser polylines to be the same as the new canvas background color LOCALLY
            foreach (Shape sh in cn.Children)
            {
                if (sh is System.Windows.Shapes.Polyline && (string)sh.Tag == "ERASER")
                {
                    sh.Fill = cn.Background;
                    sh.Stroke = cn.Background;
                }
            }
            return cn;
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

        public WBTools GetActiveTool()
        {
            return activeTool;
        }

        /// <summary>
        /// Changes the Privilege level of the current user  
        /// </summary>
        public void ChangePrivilegeSwitch()
        {
            WBOps.SwitchState();
            return;
        }

        /// <summary>
        /// Checkpoints the drawn shapes on canvas  
        /// </summary>
        public void SaveFrame(Canvas GlobCanvas)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resotres the selected checkpoint  
        /// </summary>
        public void RestoreFrame(Canvas GlobCanvas)
        {
            throw new NotImplementedException();
        }
    }
}