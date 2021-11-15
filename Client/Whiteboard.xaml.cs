using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    public class SimpleCircleAdorner : Adorner
    {
        // Be sure to call the base class constructor.
        public SimpleCircleAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }
    }



    /// <summary>
    /// Interaction logic for Whiteboard.xaml
    /// </summary>
    public partial class WhiteBoardView : UserControl
    {
        //init variables 
        private System.Windows.Controls.Primitives.ToggleButton activeMainToolbarButton;
        private Button activeSelectToolbarButton;
        private WhiteBoardViewModel viewModel;
        public Canvas GlobCanvas;

        private int mouseLeftBtnMoveFlag = 0;
        private int mouseDownFlag = 0;
        private Shape mouseDownSh;

        //Button Dynamic Colors 
        private string buttonDefaultColor = "#D500F9";
        private string buttonSelectedColor = "#007C9C";

        //Color Palette 
        private string Black = "#000000";
        private string White = "#FFFFFF";
        private string Red = "#FF0000";
        private string Green = "#00FF00";
        private string Blue = "#0000FF";
        private string Yellow = "#FFFF00";
        private string Gray = "#808080";

        //Canvas BG available Colors 
        private string canvasBg1 = "#FFFFFF";
        private string canvasBg2 = "#FF0000";
        private string canvasBg3 = "#00FF00";
        private string canvasBg4 = "#0000FF";
        private string canvasBg5 = "#FFFF00";

        //pen and eraser properties 
        private string curCanvasBg = "#FFFFFF";
        private string curPenColor = "#000000";
        private string curEraseColor = "#cfcfcf";

        private float penThickness = 5;
        private float eraserThickness = 5;

        public WhiteBoardView()
        {
            InitializeComponent();
            this.GlobCanvas = MyCanvas;
            viewModel = new WhiteBoardViewModel(GlobCanvas);
        }

        // Function to clear flags and mouse variables to be called when a popup is opened/closed or active tool is changed
        private void clearFlags()
        {
            mouseDownFlag = 0;
            mouseLeftBtnMoveFlag = 0;
            mouseDownSh = null;
            viewModel.start = new Point { X = 0, Y = 0 };
            viewModel.end = new Point { X = 0, Y = 0 };
            return;
        }

        //Function to switch to selection tool after creation of shape 
        private void switchToSelection()
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);

            }

            if (this.SelectToolBar.Visibility == Visibility.Collapsed)
            {
                this.SelectToolBar.Visibility = Visibility.Visible;
            }

            activeMainToolbarButton = this.SelectTool;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
            return;
        }

        //clear selected shapes after switching 
        private void clearSelectedShapes()
        {
            this.viewModel.shapeManager.UnselectAllBB(GlobCanvas, this.viewModel.WBOps);
        }

        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("Mouse Levae Event!");
            switch (viewModel.GetActiveTool())
            {
                case (WhiteBoardViewModel.WBTools.FreeHand):
                    if (mouseDownFlag == 1)
                    {
                        Point fh_pt = e.GetPosition(GlobCanvas);
                        GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true, false, true);
                    }
                    break;
                case (WhiteBoardViewModel.WBTools.Eraser):
                    if (mouseDownFlag == 1)
                    {
                        Point fh_pt = e.GetPosition(GlobCanvas);
                        GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false, true, true);
                    }
                    break;

                case (WhiteBoardViewModel.WBTools.NewLine):
                    if (mouseDownFlag == 1)
                    {
                        this.viewModel.end = e.GetPosition(GlobCanvas);
                        //MessageBox.Show("NewLine: start = " + viewModel.start.ToString() + " end = " + viewModel.end.ToString());
                        GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewLine, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: true);

                        //select the shape after creation 
                        switchToSelection();

                        mouseDownFlag = 0;
                    }
                    break;
                case (WhiteBoardViewModel.WBTools.NewRectangle):
                    if (mouseDownFlag == 1)
                    {
                        this.viewModel.end = e.GetPosition(GlobCanvas);
                        GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewRectangle, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: true);

                        //select the shape after creation 
                        switchToSelection();

                        mouseDownFlag = 0;
                    }
                    break;
                case (WhiteBoardViewModel.WBTools.NewEllipse):
                    //sets the end point for the creation of new ellipse
                    if (mouseDownFlag == 1)
                    {
                        this.viewModel.end = e.GetPosition(GlobCanvas);
                        GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewEllipse, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: true);

                        //select the shape after creation 
                        switchToSelection();

                        mouseDownFlag = 0;
                    }
                    break;
                default:
                    break;
            }

        }

        private void OnCanvasMouseEnter(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouseDownFlag = 1;
                switch (viewModel.GetActiveTool())
                {
                    case (WhiteBoardViewModel.WBTools.FreeHand):
                        if (mouseDownFlag == 1)
                        {
                            Point fh_pt = e.GetPosition(GlobCanvas);
                            this.viewModel.freeHand.SetColor(curPenColor);
                            this.viewModel.freeHand.SetThickness(penThickness);
                            GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true);
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Eraser):
                        if (mouseDownFlag == 1)
                        {
                            Point fh_pt = e.GetPosition(GlobCanvas);
                            this.viewModel.freeHand.SetColor(curEraseColor);
                            this.viewModel.freeHand.SetThickness(eraserThickness);
                            GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true, true);

                            if (e.OriginalSource is Polyline && ((Shape)(e.OriginalSource)).Tag is not "ERASER")
                            {
                                Polyline selectedLine = e.OriginalSource as Polyline;
                                GlobCanvas = this.viewModel.freeHand.DeletePolyline(GlobCanvas, this.viewModel.WBOps, selectedLine);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        //Canvas Mouse Down 
        private void OnCanvasMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseLeftBtnMoveFlag = 0;   //init mouse move flag 
            UIElement el = (UIElement)sender;
            if (e.LeftButton == MouseButtonState.Pressed)   //check if left mouse button is pressed 
            {
                mouseDownFlag = 1;
                switch (viewModel.GetActiveTool())
                {
                    case (WhiteBoardViewModel.WBTools.FreeHand):
                        if (mouseDownFlag == 1)
                        {
                            Point fh_pt = e.GetPosition(GlobCanvas);
                            this.viewModel.freeHand.SetColor(curPenColor);
                            this.viewModel.freeHand.SetThickness(penThickness);
                            GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true);
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Eraser):
                        if (mouseDownFlag == 1)
                        {
                            //Draw a trailing erase line 
                            Point fh_pt = e.GetPosition(GlobCanvas);
                            this.viewModel.freeHand.SetColor(curEraseColor);
                            this.viewModel.freeHand.SetThickness(eraserThickness);
                            GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true, true);

                            if (e.OriginalSource is Polyline && ((Shape)(e.OriginalSource)).Tag is not "ERASER")
                            {
                                Polyline selectedLine = e.OriginalSource as Polyline;
                                GlobCanvas = this.viewModel.freeHand.DeletePolyline(GlobCanvas, this.viewModel.WBOps, selectedLine);
                            }

                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.NewLine):
                        //sets the starting point for the creation of new line
                        this.viewModel.start = e.GetPosition(GlobCanvas);
                        break;
                    case (WhiteBoardViewModel.WBTools.NewRectangle):
                        //sets the starting point for the creation of new rectangle
                        this.viewModel.start = e.GetPosition(GlobCanvas);
                        break;
                    case (WhiteBoardViewModel.WBTools.NewEllipse):
                        //sets the starting point for the creation of new ellipse
                        this.viewModel.start = e.GetPosition(GlobCanvas);
                        break;
                    case (WhiteBoardViewModel.WBTools.Selection):
                        //sets the starting point for usage in TranslateShape/RotateShape
                        this.viewModel.start = e.GetPosition(GlobCanvas);

                        if (e.OriginalSource is Shape && e.OriginalSource is not Polyline)
                        {
                            Shape mouseDownShape = e.OriginalSource as Shape;
                            mouseDownSh = mouseDownShape;
                        }
                        else
                        {
                            mouseDownSh = null;
                        }

                        break;
                }
            }
        }

        private void OnCanvasMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement el = (UIElement)sender;
            if (e.LeftButton == MouseButtonState.Released)
            {
                switch (viewModel.GetActiveTool())
                {
                    case (WhiteBoardViewModel.WBTools.FreeHand):
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                Point fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false);
                                mouseDownFlag = 0;
                            }
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Eraser):
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                Point fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false, true, shapeComp: true);

                                if (e.OriginalSource is Polyline && ((Shape)(e.OriginalSource)).Tag is not "ERASER")
                                {
                                    Polyline selectedLine = e.OriginalSource as Polyline;
                                    GlobCanvas = this.viewModel.freeHand.DeletePolyline(GlobCanvas, this.viewModel.WBOps, selectedLine);
                                }
                                mouseDownFlag = 0;

                            }
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.NewLine):
                        if (mouseDownFlag == 1)
                        {
                            //sets the end point for the creation of new line
                            this.viewModel.end = e.GetPosition(GlobCanvas);
                            //MessageBox.Show("NewLine: start = " + viewModel.start.ToString() + " end = " + viewModel.end.ToString());
                            GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewLine, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: true);

                            //select the shape after creation 
                            switchToSelection();

                            mouseDownFlag = 0;
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.NewRectangle):
                        if (mouseDownFlag == 1)
                        {
                            //sets the end point for the creation of new rectangle
                            this.viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewRectangle, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: true);

                            //select the shape after creation 
                            switchToSelection();
                            

                            mouseDownFlag = 0;
                        }

                        break;
                    case (WhiteBoardViewModel.WBTools.NewEllipse):
                        //sets the end point for the creation of new ellipse
                        if (mouseDownFlag == 1)
                        {
                            this.viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewEllipse, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: true);

                            //select the shape after creation 
                            switchToSelection();

                            mouseDownFlag = 0;
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Selection):
                        mouseDownFlag = 0;
                        //If mouse has actually moved between press and release of left click, the selected shapes are either moved or rotated WITHOUT unselecting any shape
                        if (mouseLeftBtnMoveFlag > 5)
                        {
                            if (e.OriginalSource is Shape)
                            {
                                Shape selectedShape = e.OriginalSource as Shape;
                                if (this.viewModel.end.X != 0 && this.viewModel.end.Y != 0)
                                {
                                    //sets the end point for usage in both TranslateShape/RotateShape when left mouse button is release
                                    this.viewModel.end = e.GetPosition(MyCanvas);

                                    if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                                    {
                                        this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                    }
                                    else
                                    {
                                        this.viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                    }
                                }
                                //Resetting the value of 'start' to perform the next Move functions
                                this.viewModel.start = e.GetPosition(MyCanvas);
                            }
                        }

                        //If mouse was not moved after left clicking, then shapes would be selected/unselected
                        else
                        {
                            //sets the starting point for usage in TranslateShape/RotateShape
                            this.viewModel.start = e.GetPosition(MyCanvas);

                            //IF-ELSE to handle Select operations, i.e, Single & Multi select.
                            /*if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                if (e.OriginalSource is Shape && e.OriginalSource is not Polyline)
                                {
                                
                                    Shape selectedShape = e.OriginalSource as Shape;
                                    GlobCanvas = viewModel.shapeManager.SelectShape(GlobCanvas, selectedShape, viewModel.WBOps, 1);
                                }
                                else
                                {
                                    GlobCanvas = viewModel.shapeManager.UnselectAllBB(GlobCanvas, viewModel.WBOps);
                                }
                            }*/

                            if (e.OriginalSource is Shape && e.OriginalSource is not Polyline)
                            {
                                Shape selectedShape = e.OriginalSource as Shape;
                                GlobCanvas = viewModel.shapeManager.SelectShape(GlobCanvas, selectedShape, viewModel.WBOps, 0);
                            }
                            else
                            {
                                GlobCanvas = viewModel.shapeManager.UnselectAllBB(GlobCanvas, viewModel.WBOps);
                            }
                        }

                        break;
                }
            }
            //Resetting the flag for next usage
            mouseLeftBtnMoveFlag = 0;
            return;
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("Canvas Mouse Move triggered");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouseLeftBtnMoveFlag += 1;
                switch (viewModel.GetActiveTool())
                {
                    case (WhiteBoardViewModel.WBTools.FreeHand):
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                Point fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false);
                            }
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Eraser):
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                Point fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false, true);


                                if (e.OriginalSource is Polyline && ((Shape)(e.OriginalSource)).Tag is not "ERASER")
                                {
                                    Polyline selectedLine = e.OriginalSource as Polyline;
                                    GlobCanvas = this.viewModel.freeHand.DeletePolyline(GlobCanvas, this.viewModel.WBOps, selectedLine);
                                }
                            }
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.NewLine):
                        //sets the end point for the creation of new line
                        if (mouseDownFlag == 1)
                        {
                            this.viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewLine, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: false);
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.NewRectangle):
                        //sets the end point for the creation of new rectangle
                        if (mouseDownFlag == 1)
                        {
                            this.viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewRectangle, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: false);
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.NewEllipse):
                        //sets the end point for the creation of new ellipse
                        if (mouseDownFlag == 1)
                        {
                            this.viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = this.viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps, WhiteBoardViewModel.WBTools.NewEllipse, viewModel.start, viewModel.end, fillColor: curCanvasBg, shapeComp: false);
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Selection):
                        //if(e.OriginalSource is Shape)
                        //{
                        Shape selectedShape = e.OriginalSource as Shape;
                        //sets the end point for usage in TranslateShape/RotateShape
                        this.viewModel.end = e.GetPosition(MyCanvas);

                        if ((this.viewModel.end.X != this.viewModel.start.X || this.viewModel.end.Y != this.viewModel.start.Y) && mouseDownFlag == 1)
                        //if (this.viewModel.end.X != 0 && this.viewModel.end.Y != 0)
                        {
                            //MessageBox.Show(this.viewModel.start.ToString(), this.viewModel.end.ToString());
                            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                            {
                                //if (e.OriginalSource is Shape)                                   
                                this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, false);
                            }
                            else
                            {
                                this.viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, false);
                                //Resetting the value of 'start' to perform the next Move functions
                                this.viewModel.start = e.GetPosition(MyCanvas);
                            }
                        }
                        //}
                        break;
                }
            }
            return;
        }


        //Pop-up togglers 
        //Canvas BG color Pop-Up
        private void OpenPopupButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SetBGButtonPopUp.StaysOpen = true;
        }

        private void OpenPopupButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearFlags();
            SetBGButtonPopUp.StaysOpen = false;
        }


        //Radio Button (Set Background Pop-Up)
        private void ColorBtn1Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg1);
            curCanvasBg = canvasBg1;
        }

        private void ColorBtn2Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg2);
            curCanvasBg = canvasBg2;
        }

        private void ColorBtn3Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg3);
            curCanvasBg = canvasBg3;
        }

        private void ColorBtn4Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg4);
            curCanvasBg = canvasBg4;
        }

        private void ColorBtn5Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg5);
            curCanvasBg = canvasBg5;
        }

        //Restore Canvas Frame Pop-Up
        private void OpenPopupRestoreButton_MouseEnter(object sender, MouseEventArgs e)
        {
            RestoreFramePopUp.StaysOpen = true;
        }

        private void OpenPopupRestoreButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearFlags();
            RestoreFramePopUp.StaysOpen = false;
        }

        //Selection Toolbar Pop-Ups 
        //Fill Shape Color Pop-Up
        private void OpenPopupFillShapeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SetShapeFillPopUp.StaysOpen = true;
        }

        private void OpenPopupFillShapeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearFlags();
            SetShapeFillPopUp.StaysOpen = false;
        }

        //Fill Shape Check Buttons 
        private void ColorFill1Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", canvasBg1, 1);
        }

        private void ColorFill2Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", canvasBg2, 1);
        }

        private void ColorFill3Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", canvasBg3, 1);
        }

        private void ColorFill4Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", canvasBg4, 1);
        }

        private void ColorFill5Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", canvasBg5, 1);
        }

        //Shape Stroke Properties Pop-Up
        private void OpenPopupShapeBorderButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SetShapeBorderPopUp.StaysOpen = true;
        }

        private void OpenPopupShapeBorderButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearFlags();
            SetShapeBorderPopUp.StaysOpen = false;
        }

        //Fill Border Check Buttons 
        private void ColorBorder1Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Black, 0);
        }

        private void ColorBorder2Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", canvasBg2, 0);
        }

        private void ColorBorder3Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", canvasBg3, 0);
        }

        private void ColorBorder4Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", canvasBg4, 0);
        }

        private void ColorBorder5Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", canvasBg5, 0);
        }

        //Stroke Thickness Slider Control 
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float thickness = (byte)StrokeThicknessSlider.Value;

            if (thickness > 0)
            {
                this.viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "StrokeThickness", Black, thickness);
            }

        }

        //Main Toolbar Pop-Ups 
        //Free Hand Pop-Up
        private void OpenPopupFreeHandButton_MouseEnter(object sender, MouseEventArgs e)
        {
            FreeHandPopUp.StaysOpen = true;
        }

        private void OpenPopupFreeHandButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearFlags();
            FreeHandPopUp.StaysOpen = false;
        }

        //Pen Color Check Buttons 
        private void ColorPen1Checked(object sender, RoutedEventArgs e)
        {
            this.viewModel.freeHand.SetColor(Black);
            curPenColor = Black;
        }

        private void ColorPen2Checked(object sender, RoutedEventArgs e)
        {
            this.viewModel.freeHand.SetColor(Red);
            curPenColor = Red;
        }

        private void ColorPen3Checked(object sender, RoutedEventArgs e)
        {
            this.viewModel.freeHand.SetColor(Green);
            curPenColor = Green;
        }

        private void ColorPen4Checked(object sender, RoutedEventArgs e)
        {
            this.viewModel.freeHand.SetColor(Blue);
            curPenColor = Blue;
        }

        private void ColorPen5Checked(object sender, RoutedEventArgs e)
        {
            this.viewModel.freeHand.SetColor(Yellow);
            curPenColor = Yellow;
        }

        //Stroke Thickness Slider Control 
        private void PenThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float thickness = (byte)FreeHandThicknessSlider.Value;

            if (thickness > 0)
            {
                this.viewModel.freeHand.SetThickness(thickness);
                penThickness = thickness;
            }

        }

        //Eraser Pop-up
        /*private void OpenPopupEraserButton_MouseEnter(object sender, MouseEventArgs e)
        {
            EraserPopUp.StaysOpen = true;
        }

        private void OpenPopupEraserButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearFlags();
            EraserPopUp.StaysOpen = false;
        }

        private void EraserThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float thickness = (byte)EraserThicknessSlider.Value;

            if (thickness > 0)
            {
                this.viewModel.freeHand.SetThickness(thickness);
                eraserThickness = thickness;
            }

        }*/

        //Main Toolbar Here 
        //Toolbar selection tool 
        private void ClickedSelectTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);
                clearSelectedShapes();
            }

            if (this.SelectToolBar.Visibility == Visibility.Collapsed)
            {
                this.SelectToolBar.Visibility = Visibility.Visible;
            }

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
            return;
        }

        //Toolbar Rectangle tool
        private void ClickedRectTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);
                clearSelectedShapes();
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
            return;
        }

        //Toolbar Ellipse tool
        private void ClickedEllTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);
                clearSelectedShapes();
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
            return;
        }

        //Toolbar FreeHand tool
        private void CheckedFreehandTool(object sender, RoutedEventArgs e)
        {
            // Code for Un-Checked state
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);
                clearSelectedShapes();
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Pen;
            return;
        }

        //Toolbar Eraser tool
        private void ClickedEraserTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);
                clearSelectedShapes();
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = ((TextBlock)this.Resources["CursorErase32"]).Cursor;
            return;
        }

        //Toolbar Line tool
        private void ClickedLineTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeMainToolbarButton.ClearValue(System.Windows.Controls.Primitives.ToggleButton.BackgroundProperty);
                clearSelectedShapes();
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
            return;
        }

        //Selection Toolbar
        //Fill Border Tool Button
        private void ClickedFillBorderTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);

            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            return;
        }

        //Fill Shape Tool Button
        private void ClickedFillShapeTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            //viewModel.shapeManager.SetBackgroundColor();
            return;
        }

        //Duplicate Shape Tool Button
        private void ClickedDuplicateTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            return;
        }

        //Delete Shape Tool Button
        private void ClickedDeleteTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas = viewModel.shapeManager.DeleteSelectedShapes(GlobCanvas, viewModel.WBOps);
            return;
        }

        //Whiteboard General tools 

        //Clear Frame Button Control 
        private void ClickedClearFrame(object sender, RoutedEventArgs e)
        {
            //To change this 
            GlobCanvas.Children.Clear();
            return;
        }

        //Save Frame Button Control
        private void ClickedSaveFrame(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedSaveFrame");
            return;
        }

        //Undo Button Control
        private void ClickedUndoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedUndo");
            return;
        }

        //Redo Button Control
        private void ClickedRedoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedRedo");
            return;
        }

        //Toggle Button Control (Canvas State Lock)
        private void Bu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Bu.Toggled1 == true)
            {
                MessageBox.Show("Toggled On");
            }
            else
            {
                MessageBox.Show("Toggled Off");
            }
        }


    }
}