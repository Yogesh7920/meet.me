/**
 * Owned By: Arpan Tripathi
 * Created By: Arpan Tripathi
 * Date Created: 25/10/2021
 * Date Modified: 28/11/2021
**/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Whiteboard.xaml
    /// </summary>
    public partial class WhiteBoardView : UserControl
    {
        //init variables 
        private System.Windows.Controls.Primitives.ToggleButton activeMainToolbarButton;
        private Button activeSelectToolbarButton;
        private RadioButton rbutton; 
        private WhiteBoardViewModel viewModel;
        public Canvas GlobCanvas;

        private int mouseLeftBtnMoveFlag = 0;
        private int mouseDownFlag = 0;
        private Shape mouseDownSh;

        //Button Dynamic Colors 
        private string buttonDefaultColor = "#D500F9";
        private string buttonSelectedColor = "#007C9C";

        //Color Palette 
        private string Black = "#161B22";
        private string White = "#FFFFFF";
        private string Red = "#900604";
        private string Green = "#1E5631";
        private string Blue = "#005CC3";
        private string Yellow = "#EFC002";
        private string Gray = "#909090";

        //Canvas BG available Colors 
        private string canvasBg1 = "#FFFFFF";
        private string canvasBg2 = "#F8F8FF";
        private string canvasBg3 = "#FFFAFA";
        private string canvasBg4 = "#FFFFF0";
        private string canvasBg5 = "#FFFAF0";

        //pen and eraser properties 
        private string curCanvasBg = "#FFFFFF";
        private string curPenColor = "#000000";
        private string curEraseColor = "#cfcfcf";

        private float penThickness = 5;
        private float eraserThickness = 5;

        //variable to keep track for rotaion of shape is in progress or not 
        bool rotation = false;
        bool close_popup = false; 

        /// <summary>
        /// Constructor for View in MVVM pattern
        /// </summary>
        public WhiteBoardView()
        {
            InitializeComponent();

            this.GlobCanvas = MyCanvas;
            viewModel = new WhiteBoardViewModel(GlobCanvas);
            this.DataContext = viewModel;
            this.RestorFrameDropDown.SelectionChanged += RestorFrameDropDown_SelectionChanged;
        }

        /// <summary>
        /// Checkpoint listbox selection changed event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestorFrameDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            
            if(this.RestorFrameDropDown.SelectedItem != null){
                
                string item = listbox.SelectedItem.ToString();
                string numeric = new String(item.Where(Char.IsDigit).ToArray());
                int cp = int.Parse(numeric);

                MessageBoxResult result = MessageBox.Show("Are you sure you want to load checkpoint " + numeric + " ? All progress since the last checkpoint would be lost!",
                              "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    viewModel.RestoreFrame(cp, GlobCanvas);
                    this.RestorFrameDropDown.SelectedItem = null;
                    return;
                }
                else
                {
                    this.RestorFrameDropDown.SelectedItem = null;
                    return;
                }
            }
            else
            {
                return; 
            }

        }

        /// <summary>
        /// Function to clear flags and mouse variables to be called when a popup is opened/closed or active tool is changed
        /// </summary>
        private void clearFlags()
        {
            mouseDownFlag = 0;
            mouseLeftBtnMoveFlag = 0;
            mouseDownSh = null;
            rotation = false; 
            viewModel.start = new Point { X = 0, Y = 0 };
            viewModel.end = new Point { X = 0, Y = 0 };
            return;
        }

        /// <summary>
        /// Function to switch to selection tool after creation of shape 
        /// </summary>
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

        /// <summary>
        /// Function to clear selected shapes
        /// </summary>
        private void clearSelectedShapes()
        {
            this.viewModel.shapeManager.UnselectAllBB(GlobCanvas, this.viewModel.WBOps);
        }

        /// <summary>
        /// Canvas Mouse leave event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            switch (viewModel.GetActiveTool())
            {
                case (WhiteBoardViewModel.WBTools.FreeHand):
                    if (mouseDownFlag == 1)
                    {
                        Point fh_pt = e.GetPosition(GlobCanvas);
                        GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false, false, true);
                        mouseDownFlag = 0;
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
                case (WhiteBoardViewModel.WBTools.Selection):
                    mouseDownFlag = 0;
                    //If mouse has actually moved between press and release of left click, the selected shapes are either moved or rotated WITHOUT unselecting any shape
                    if (mouseLeftBtnMoveFlag > 5)
                    {
                        if (this.viewModel.end.X != 0 && this.viewModel.end.Y != 0)
                        {

                            if (rotation == true)
                            {
                                this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                rotation = false;
                            }
                            else
                            {
                                this.viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Canvas mouse enter event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Canvas mouse down event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                            GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, creation: true);
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Eraser):
                        if (mouseDownFlag == 1)
                        {
                            //Draw a trailing erase line 
                            Point fh_pt = e.GetPosition(GlobCanvas);
                            this.viewModel.freeHand.SetColor(curEraseColor);
                            this.viewModel.freeHand.SetThickness(eraserThickness);
                            GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, creation: true, isEraser: true);

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
                            //setting the initial mouse down position for usage in shapeManager.MoveShape,
                            //as we need to send the server the very start and very end points during a final move operation
                            //since we are doing temporary rendering by using differential (start,end) points pair
                            this.viewModel.setSelectMouseDownPos(e.GetPosition(GlobCanvas));
                            Shape mouseDownShape = e.OriginalSource as Shape;
                            mouseDownSh = mouseDownShape;
                            this.viewModel.shapeManager.selectMouseStuck = e.GetPosition(GlobCanvas);
                        }
                        else
                        {
                            mouseDownSh = null;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Canvas Mouse Button Up event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, creation: false, shapeComp: true);
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
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, creation: false, isEraser: true, shapeComp: true);

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
                            //if ((e.OriginalSource is Shape && ((Shape)e.OriginalSource) == mouseDownSh) || rotation == true)
                            //{

                                if (this.viewModel.end.X != 0 && this.viewModel.end.Y != 0)
                                {
                                    //sets the end point for usage in both TranslateShape/RotateShape when left mouse button is release
                                    //this.viewModel.end = e.GetPosition(MyCanvas);

                                    if (rotation == true)
                                    {
                                        this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                        rotation = false; 
                                    }
                                    /*else if (Keyboard.IsKeyUp(Key.LeftAlt) && rotation == true)
                                    {
                                        this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                        rotation = false;
                                    }*/
                                    else
                                    {
                                        this.viewModel.end = e.GetPosition(MyCanvas);
                                        this.viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                    }
                                }
                                //Resetting the value of 'start' to perform the next Move functions
                                this.viewModel.start = e.GetPosition(MyCanvas);
                            //}
                        }

                        //If mouse was not moved after left clicking, then shapes would be selected/unselected
                        else
                        {
                            //sets the starting point for usage in TranslateShape/RotateShape
                            this.viewModel.start = e.GetPosition(MyCanvas);

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

        /// <summary>
        /// Canvas Mouse Move event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                            if (mouseDownFlag == 1 && mouseLeftBtnMoveFlag > 5)
                            {
                                Point fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, creation: false);
                                mouseLeftBtnMoveFlag = 0;
                            }
                        }
                        break;
                    case (WhiteBoardViewModel.WBTools.Eraser):
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                Point fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = this.viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, creation: false, isEraser: true);


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
                        if(e.OriginalSource is Shape && ((Shape)e.OriginalSource) == mouseDownSh)
                        {
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
                                    //Resetting the value of 'start' to perform the next Move functions
                                    this.viewModel.start = e.GetPosition(MyCanvas);

                                    rotation = true;

                                }else if(rotation == true)
                                {
                                    //if (e.OriginalSource is Shape)                                   
                                    this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, false);
                                    //Resetting the value of 'start' to perform the next Move functions
                                    this.viewModel.start = e.GetPosition(MyCanvas);

                                    //rotation = true;
                                }
                                else
                                {
                                    this.viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, false);
                                    //Resetting the value of 'start' to perform the next Move functions
                                    this.viewModel.start = e.GetPosition(MyCanvas);
                                }
                            }
                        }
                        break;
                }
            }
            return;
        }

        /// <summary>
        /// MouseWheel function to mock Canvas coordinate system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MessageBox.Show("Scrolled at X =" + e.GetPosition(GlobCanvas).X.ToString() + " ,Y = " + e.GetPosition(GlobCanvas).Y.ToString());
            MessageBox.Show("Canvas has Width = " + GlobCanvas.ActualWidth + " , Height = " + GlobCanvas.ActualHeight);

            SolidColorBrush blackBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));

            System.Windows.Shapes.Rectangle shp = new System.Windows.Shapes.Rectangle { Width = 50, Height = 50, Stroke = blackBrush };
            GlobCanvas.Children.Add(shp);

            //Check for Systems.Windows Canvas.SetLeft & SetTop
            Canvas.SetLeft(shp, 20);
            Canvas.SetTop(shp, 100);
            MessageBox.Show("The Shape should be present at X=20 & Y=100 acc. to System.Windows convention");

            //Check for System.Windows Shapes Height & Width
            double origHt = shp.Height;
            double origWt = shp.Width;
            shp.Width = 150;
            shp.Height = 30;

            MessageBox.Show("The Shape should be of Width=150 (parallel to X axis) & Height=30 (parallel to Y axis) acc. to System.Windows convention");

            shp.Height = origHt;
            shp.Width = origWt;

            GlobCanvas.Children.Remove(shp);
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
            viewModel.ChangeWbBackground(canvasBg1);
            curCanvasBg = canvasBg1;
        }

        private void ColorBtn2Checked(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeWbBackground(canvasBg2);
            curCanvasBg = canvasBg2;
        }

        private void ColorBtn3Checked(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeWbBackground(canvasBg3);
            curCanvasBg = canvasBg3;
        }

        private void ColorBtn4Checked(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeWbBackground(canvasBg4);
            curCanvasBg = canvasBg4;
        }

        private void ColorBtn5Checked(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeWbBackground(canvasBg5);
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
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", White, 1);
            rbutton.IsChecked = false;
        }

        private void ColorFill2Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", Red, 1);
            rbutton.IsChecked = false;
        }

        private void ColorFill3Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", Green, 1);
            rbutton.IsChecked = false;
        }

        private void ColorFill4Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", Blue, 1);
            rbutton.IsChecked = false;
        }

        private void ColorFill5Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Fill", Yellow, 1);
            rbutton.IsChecked = false;
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
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Black, 0);
            rbutton.IsChecked = false; 
        }

        private void ColorBorder2Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Red, 0);
            rbutton.IsChecked = false;
        }

        private void ColorBorder3Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Green, 0);
            rbutton.IsChecked = false;
        }

        private void ColorBorder4Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Blue, 0);
            rbutton.IsChecked = false;
        }

        private void ColorBorder5Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Yellow, 0);
            rbutton.IsChecked = false;
        }

        //Stroke Thickness Slider Control 
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float thickness = (byte)StrokeThicknessSlider.Value;

            if (thickness > 0 && viewModel != null)
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

        /// <summary>
        /// Toolbar selection tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toolbar Rectangle tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toolbar Ellipse tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toolbar FreeHand tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toolbar Eraser tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toolbar Line tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Fill Border Tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedFillBorderTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);

            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            return;
        }

        /// <summary>
        /// Fill Shape Tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedFillShapeTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            //viewModel.shapeManager.SetBackgroundColor();
            return;
        }

        /// <summary>
        /// Duplicate Shape Tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedDuplicateTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);

            viewModel.shapeManager.DuplicateShape(GlobCanvas, viewModel.WBOps);
            return;
        }

        /// <summary>
        /// Delete Shape Tool 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedDeleteTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            activeSelectToolbarButton.ClearValue(Button.BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas = viewModel.DeleteShape(GlobCanvas);
            return;
        }

        //Whiteboard General tools 

        /// <summary>
        /// Clear Frame Button Control 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedClearFrame(object sender, RoutedEventArgs e)
        {
            if(Bu_P.Toggled1 == true)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to clear frame ? All progress since the last checkpoint would be lost.",
                          "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    GlobCanvas = viewModel.ClearCanvas(GlobCanvas);
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("You must be a user of high priority to call clear canvas!");
            }
        }

        /// <summary>
        /// Save Frame Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedSaveFrame(object sender, RoutedEventArgs e)
        {
            //this.viewModel.NumCheckpoints += 1;
            this.viewModel.SaveFrame();
            return;
        }

        /// <summary>
        /// Undo Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedUndoButton(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ClickedUndo");
            this.viewModel.sendUndoRequest();
            return;
        }

        /// <summary>
        /// Redo Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedRedoButton(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ClickedRedo");
            this.viewModel.sendRedoRequest();
            return;
        }

        /// <summary>
        /// Toggle Button Control (Canvas State Lock)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Bu.Toggled1 == true)
            {
                this.viewModel.ChangeActivityState();
                this.ActivityBlock.Text = "Inactive";
                MessageBox.Show("Changed State to Inactive");
            }
            else
            {
                this.viewModel.ChangeActivityState();
                this.ActivityBlock.Text = "Active";
                MessageBox.Show("Changed State to Active");
            }
        }

        /// <summary>
        /// Toggle Button Control (User Priority)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bu_PMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Bu_P.Toggled1 == true)
            {
                this.viewModel.WBOps.SetUserLevel(1);
                this.PriorityBlock.Text = "High PR";
                MessageBox.Show("Switched to High Priority");
            }
            else
            {
                this.viewModel.WBOps.SetUserLevel(0);
                this.PriorityBlock.Text = "Medium PR";
                MessageBox.Show("Switched to Medium Priority");
            }
        }
    }
}