/**
 * Owned By: Arpan Tripathi
 * Created By: Arpan Tripathi
 * Date Created: 25/10/2021
 * Date Modified: 28/11/2021
**/

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    ///     Interaction logic for Whiteboard.xaml
    /// </summary>
    public partial class WhiteBoardView : UserControl
    {
        //init variables 
        private System.Windows.Controls.Primitives.ToggleButton activeMainToolbarButton;
        private Button activeSelectToolbarButton;

        //Color Palette 
        private readonly string Black = "#161B22";
        private readonly string Blue = "#005CC3";

        //Button Dynamic Colors 
        private readonly string buttonDefaultColor = "#D500F9";
        private readonly string buttonSelectedColor = "#007C9C";

        //Canvas BG available Colors 
        private readonly string canvasBg1 = "#FFFFFF";
        private readonly string canvasBg2 = "#fff0f5";
        private readonly string canvasBg3 = "#e7feff";
        private readonly string canvasBg4 = "#faf0e6";
        private readonly string canvasBg5 = "#ffffe0";
        private bool close_popup = false;

        //pen and eraser properties 
        private string curCanvasBg = "#FFFFFF";
        private readonly string curEraseColor = "#cfcfcf";
        private string curPenColor = "#000000";
        private readonly float eraserThickness = 5;
        public Canvas GlobCanvas;
        private string Gray = "#909090";
        private readonly string Green = "#1E5631";
        private int mouseDownFlag;
        private Shape mouseDownSh;

        private int mouseLeftBtnMoveFlag;

        private float penThickness = 5;
        private RadioButton rbutton;
        private readonly string Red = "#900604";

        //variable to keep track for rotaion of shape is in progress or not 
        private bool rotation;
        private readonly WhiteBoardViewModel viewModel;
        private readonly string White = "#FFFFFF";
        private readonly string Yellow = "#EFC002";

        /// <summary>
        ///     Constructor for View in MVVM pattern
        /// </summary>
        public WhiteBoardView()
        {
            InitializeComponent();

            GlobCanvas = MyCanvas;
            viewModel = new WhiteBoardViewModel(GlobCanvas);
            DataContext = viewModel;
            RestorFrameDropDown.SelectionChanged += RestorFrameDropDown_SelectionChanged;
        }

        /// <summary>
        ///     Checkpoint listbox selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestorFrameDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox) sender;

            if (RestorFrameDropDown.SelectedItem != null)
            {
                var item = listbox.SelectedItem.ToString();
                var numeric = new string(item.Where(char.IsDigit).ToArray());
                var cp = int.Parse(numeric);

                var result = MessageBox.Show(
                    "Are you sure you want to load checkpoint " + numeric +
                    " ? All progress since the last checkpoint would be lost!",
                    "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    viewModel.RestoreFrame(cp, GlobCanvas);
                    RestorFrameDropDown.SelectedItem = null;
                    return;
                }

                RestorFrameDropDown.SelectedItem = null;
                return;
            }
        }

        /// <summary>
        ///     Function to clear flags and mouse variables to be called when a popup is opened/closed or active tool is changed
        /// </summary>
        private void clearFlags()
        {
            mouseDownFlag = 0;
            mouseLeftBtnMoveFlag = 0;
            mouseDownSh = null;
            rotation = false;
            viewModel.start = new Point {X = 0, Y = 0};
            viewModel.end = new Point {X = 0, Y = 0};
        }

        /// <summary>
        ///     Function to switch to selection tool after creation of shape
        /// </summary>
        private void switchToSelection()
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Collapsed) SelectToolBar.Visibility = Visibility.Visible;

            activeMainToolbarButton = SelectTool;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
        }

        /// <summary>
        ///     Function to clear selected shapes
        /// </summary>
        private void clearSelectedShapes()
        {
            viewModel.shapeManager.UnselectAllBB(GlobCanvas, viewModel.WBOps);
        }

        /// <summary>
        ///     Canvas Mouse leave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            switch (viewModel.GetActiveTool())
            {
                case WhiteBoardViewModel.WBTools.FreeHand:
                    if (mouseDownFlag == 1)
                    {
                        var fh_pt = e.GetPosition(GlobCanvas);
                        GlobCanvas =
                            viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false, false, true);
                        mouseDownFlag = 0;
                    }

                    break;
                case WhiteBoardViewModel.WBTools.Eraser:
                    if (mouseDownFlag == 1)
                    {
                        var fh_pt = e.GetPosition(GlobCanvas);
                        GlobCanvas =
                            viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false, true, true);
                    }

                    break;

                case WhiteBoardViewModel.WBTools.NewLine:
                    if (mouseDownFlag == 1)
                    {
                        viewModel.end = e.GetPosition(GlobCanvas);
                        //MessageBox.Show("NewLine: start = " + viewModel.start.ToString() + " end = " + viewModel.end.ToString());
                        GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                            WhiteBoardViewModel.WBTools.NewLine, viewModel.start, viewModel.end, fillColor: curCanvasBg,
                            shapeComp: true);

                        //select the shape after creation 
                        switchToSelection();

                        mouseDownFlag = 0;
                    }

                    break;
                case WhiteBoardViewModel.WBTools.NewRectangle:
                    if (mouseDownFlag == 1)
                    {
                        viewModel.end = e.GetPosition(GlobCanvas);
                        GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                            WhiteBoardViewModel.WBTools.NewRectangle, viewModel.start, viewModel.end,
                            fillColor: curCanvasBg, shapeComp: true);

                        //select the shape after creation 
                        switchToSelection();

                        mouseDownFlag = 0;
                    }

                    break;
                case WhiteBoardViewModel.WBTools.NewEllipse:
                    //sets the end point for the creation of new ellipse
                    if (mouseDownFlag == 1)
                    {
                        viewModel.end = e.GetPosition(GlobCanvas);
                        GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                            WhiteBoardViewModel.WBTools.NewEllipse, viewModel.start, viewModel.end,
                            fillColor: curCanvasBg, shapeComp: true);

                        //select the shape after creation 
                        switchToSelection();

                        mouseDownFlag = 0;
                    }

                    break;
                case WhiteBoardViewModel.WBTools.Selection:
                    mouseDownFlag = 0;
                    //If mouse has actually moved between press and release of left click, the selected shapes are either moved or rotated WITHOUT unselecting any shape
                    if (mouseLeftBtnMoveFlag > 5)
                        if (viewModel.end.X != 0 && viewModel.end.Y != 0)
                        {
                            if (rotation)
                            {
                                viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                    viewModel.end, mouseDownSh, true);
                                rotation = false;
                            }
                            else
                            {
                                viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                    viewModel.end, mouseDownSh, true);
                            }
                        }

                    break;
            }
        }

        /// <summary>
        ///     Canvas mouse enter event
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
                    case WhiteBoardViewModel.WBTools.FreeHand:
                        if (mouseDownFlag == 1)
                        {
                            var fh_pt = e.GetPosition(GlobCanvas);
                            viewModel.freeHand.SetColor(curPenColor);
                            viewModel.freeHand.SetThickness(penThickness);
                            GlobCanvas = viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true);
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.Eraser:
                        if (mouseDownFlag == 1)
                        {
                            var fh_pt = e.GetPosition(GlobCanvas);
                            viewModel.freeHand.SetColor(curEraseColor);
                            viewModel.freeHand.SetThickness(eraserThickness);
                            GlobCanvas =
                                viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true, true);

                            if (e.OriginalSource is Polyline && ((Shape) e.OriginalSource).Tag is not "ERASER")
                            {
                                var selectedLine = e.OriginalSource as Polyline;
                                GlobCanvas =
                                    viewModel.freeHand.DeletePolyline(GlobCanvas, viewModel.WBOps, selectedLine);
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        ///     Canvas mouse down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanvasMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseLeftBtnMoveFlag = 0; //init mouse move flag 
            var el = (UIElement) sender;
            if (e.LeftButton == MouseButtonState.Pressed) //check if left mouse button is pressed 
            {
                mouseDownFlag = 1;
                switch (viewModel.GetActiveTool())
                {
                    case WhiteBoardViewModel.WBTools.FreeHand:
                        if (mouseDownFlag == 1)
                        {
                            var fh_pt = e.GetPosition(GlobCanvas);
                            viewModel.freeHand.SetColor(curPenColor);
                            viewModel.freeHand.SetThickness(penThickness);
                            GlobCanvas = viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true);
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.Eraser:
                        if (mouseDownFlag == 1)
                        {
                            //Draw a trailing erase line 
                            var fh_pt = e.GetPosition(GlobCanvas);
                            viewModel.freeHand.SetColor(curEraseColor);
                            viewModel.freeHand.SetThickness(eraserThickness);
                            GlobCanvas =
                                viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, true, true);

                            if (e.OriginalSource is Polyline && ((Shape) e.OriginalSource).Tag is not "ERASER")
                            {
                                var selectedLine = e.OriginalSource as Polyline;
                                GlobCanvas =
                                    viewModel.freeHand.DeletePolyline(GlobCanvas, viewModel.WBOps, selectedLine);
                            }
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewLine:
                        //sets the starting point for the creation of new line
                        viewModel.start = e.GetPosition(GlobCanvas);
                        break;
                    case WhiteBoardViewModel.WBTools.NewRectangle:
                        //sets the starting point for the creation of new rectangle
                        viewModel.start = e.GetPosition(GlobCanvas);
                        break;
                    case WhiteBoardViewModel.WBTools.NewEllipse:
                        //sets the starting point for the creation of new ellipse
                        viewModel.start = e.GetPosition(GlobCanvas);
                        break;
                    case WhiteBoardViewModel.WBTools.Selection:
                        //sets the starting point for usage in TranslateShape/RotateShape
                        viewModel.start = e.GetPosition(GlobCanvas);

                        if (e.OriginalSource is Shape && e.OriginalSource is not Polyline)
                        {
                            //setting the initial mouse down position for usage in shapeManager.MoveShape,
                            //as we need to send the server the very start and very end points during a final move operation
                            //since we are doing temporary rendering by using differential (start,end) points pair
                            viewModel.setSelectMouseDownPos(e.GetPosition(GlobCanvas));
                            var mouseDownShape = e.OriginalSource as Shape;
                            mouseDownSh = mouseDownShape;
                            viewModel.shapeManager.selectMouseStuck = e.GetPosition(GlobCanvas);
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
        ///     Canvas Mouse Button Up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanvasMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var el = (UIElement) sender;
            if (e.LeftButton == MouseButtonState.Released)
                switch (viewModel.GetActiveTool())
                {
                    case WhiteBoardViewModel.WBTools.FreeHand:
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                var fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false,
                                    shapeComp: true);
                                mouseDownFlag = 0;
                            }
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.Eraser:
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                var fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false,
                                    true, true);

                                if (e.OriginalSource is Polyline && ((Shape) e.OriginalSource).Tag is not "ERASER")
                                {
                                    var selectedLine = e.OriginalSource as Polyline;
                                    GlobCanvas =
                                        viewModel.freeHand.DeletePolyline(GlobCanvas, viewModel.WBOps, selectedLine);
                                }

                                mouseDownFlag = 0;
                            }
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewLine:
                        if (mouseDownFlag == 1)
                        {
                            //sets the end point for the creation of new line
                            viewModel.end = e.GetPosition(GlobCanvas);
                            //MessageBox.Show("NewLine: start = " + viewModel.start.ToString() + " end = " + viewModel.end.ToString());
                            GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                                WhiteBoardViewModel.WBTools.NewLine, viewModel.start, viewModel.end,
                                fillColor: curCanvasBg, shapeComp: true);

                            //select the shape after creation 
                            switchToSelection();

                            mouseDownFlag = 0;
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewRectangle:
                        if (mouseDownFlag == 1)
                        {
                            //sets the end point for the creation of new rectangle
                            viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                                WhiteBoardViewModel.WBTools.NewRectangle, viewModel.start, viewModel.end,
                                fillColor: curCanvasBg, shapeComp: true);

                            //select the shape after creation 
                            switchToSelection();


                            mouseDownFlag = 0;
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewEllipse:
                        //sets the end point for the creation of new ellipse
                        if (mouseDownFlag == 1)
                        {
                            viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                                WhiteBoardViewModel.WBTools.NewEllipse, viewModel.start, viewModel.end,
                                fillColor: curCanvasBg, shapeComp: true);

                            //select the shape after creation 
                            switchToSelection();

                            mouseDownFlag = 0;
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.Selection:
                        mouseDownFlag = 0;
                        //If mouse has actually moved between press and release of left click, the selected shapes are either moved or rotated WITHOUT unselecting any shape
                        if (mouseLeftBtnMoveFlag > 5)
                        {
                            //if ((e.OriginalSource is Shape && ((Shape)e.OriginalSource) == mouseDownSh) || rotation == true)
                            //{

                            if (viewModel.end.X != 0 && viewModel.end.Y != 0)
                            {
                                //sets the end point for usage in both TranslateShape/RotateShape when left mouse button is release
                                //this.viewModel.end = e.GetPosition(MyCanvas);

                                if (rotation)
                                {
                                    viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                        viewModel.end, mouseDownSh, true);
                                    rotation = false;
                                }
                                /*else if (Keyboard.IsKeyUp(Key.LeftAlt) && rotation == true)
                                    {
                                        this.viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start, viewModel.end, mouseDownSh, true);
                                        rotation = false;
                                    }*/
                                else
                                {
                                    viewModel.end = e.GetPosition(MyCanvas);
                                    viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                        viewModel.end, mouseDownSh, true);
                                }
                            }

                            //Resetting the value of 'start' to perform the next Move functions
                            viewModel.start = e.GetPosition(MyCanvas);
                            //}
                        }

                        //If mouse was not moved after left clicking, then shapes would be selected/unselected
                        else
                        {
                            //sets the starting point for usage in TranslateShape/RotateShape
                            viewModel.start = e.GetPosition(MyCanvas);

                            if (e.OriginalSource is Shape && e.OriginalSource is not Polyline)
                            {
                                var selectedShape = e.OriginalSource as Shape;
                                GlobCanvas =
                                    viewModel.shapeManager.SelectShape(GlobCanvas, selectedShape, viewModel.WBOps);
                            }
                            else
                            {
                                GlobCanvas = viewModel.shapeManager.UnselectAllBB(GlobCanvas, viewModel.WBOps);
                            }
                        }

                        break;
                }

            //Resetting the flag for next usage
            mouseLeftBtnMoveFlag = 0;
        }

        /// <summary>
        ///     Canvas Mouse Move event
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
                    case WhiteBoardViewModel.WBTools.FreeHand:
                        lock (this)
                        {
                            if (mouseDownFlag == 1 && mouseLeftBtnMoveFlag > 5)
                            {
                                var fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt);
                                mouseLeftBtnMoveFlag = 0;
                            }
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.Eraser:
                        lock (this)
                        {
                            if (mouseDownFlag == 1)
                            {
                                var fh_pt = e.GetPosition(GlobCanvas);
                                GlobCanvas = viewModel.freeHand.DrawPolyline(GlobCanvas, viewModel.WBOps, fh_pt, false,
                                    true);


                                if (e.OriginalSource is Polyline && ((Shape) e.OriginalSource).Tag is not "ERASER")
                                {
                                    var selectedLine = e.OriginalSource as Polyline;
                                    GlobCanvas =
                                        viewModel.freeHand.DeletePolyline(GlobCanvas, viewModel.WBOps, selectedLine);
                                }
                            }
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewLine:
                        //sets the end point for the creation of new line
                        if (mouseDownFlag == 1)
                        {
                            viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                                WhiteBoardViewModel.WBTools.NewLine, viewModel.start, viewModel.end,
                                fillColor: curCanvasBg, shapeComp: false);
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewRectangle:
                        //sets the end point for the creation of new rectangle
                        if (mouseDownFlag == 1)
                        {
                            viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                                WhiteBoardViewModel.WBTools.NewRectangle, viewModel.start, viewModel.end,
                                fillColor: curCanvasBg, shapeComp: false);
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.NewEllipse:
                        //sets the end point for the creation of new ellipse
                        if (mouseDownFlag == 1)
                        {
                            viewModel.end = e.GetPosition(GlobCanvas);
                            GlobCanvas = viewModel.shapeManager.CreateShape(GlobCanvas, viewModel.WBOps,
                                WhiteBoardViewModel.WBTools.NewEllipse, viewModel.start, viewModel.end,
                                fillColor: curCanvasBg, shapeComp: false);
                        }

                        break;
                    case WhiteBoardViewModel.WBTools.Selection:
                        if (e.OriginalSource is Shape && (Shape) e.OriginalSource == mouseDownSh)
                        {
                            //sets the end point for usage in TranslateShape/RotateShape
                            viewModel.end = e.GetPosition(MyCanvas);

                            if ((viewModel.end.X != viewModel.start.X || viewModel.end.Y != viewModel.start.Y) &&
                                mouseDownFlag == 1)
                                //if (this.viewModel.end.X != 0 && this.viewModel.end.Y != 0)
                            {
                                //MessageBox.Show(this.viewModel.start.ToString(), this.viewModel.end.ToString());
                                if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                                {
                                    //if (e.OriginalSource is Shape)                                   
                                    viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                        viewModel.end, mouseDownSh, false);
                                    //Resetting the value of 'start' to perform the next Move functions
                                    viewModel.start = e.GetPosition(MyCanvas);

                                    rotation = true;
                                }
                                else if (rotation)
                                {
                                    //if (e.OriginalSource is Shape)                                   
                                    viewModel.shapeManager.RotateShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                        viewModel.end, mouseDownSh, false);
                                    //Resetting the value of 'start' to perform the next Move functions
                                    viewModel.start = e.GetPosition(MyCanvas);

                                    //rotation = true;
                                }
                                else
                                {
                                    viewModel.shapeManager.MoveShape(GlobCanvas, viewModel.WBOps, viewModel.start,
                                        viewModel.end, mouseDownSh, false);
                                    //Resetting the value of 'start' to perform the next Move functions
                                    viewModel.start = e.GetPosition(MyCanvas);
                                }
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        ///     MouseWheel function to mock Canvas coordinate system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MessageBox.Show("Scrolled at X =" + e.GetPosition(GlobCanvas).X + " ,Y = " + e.GetPosition(GlobCanvas).Y);
            MessageBox.Show("Canvas has Width = " + GlobCanvas.ActualWidth + " , Height = " + GlobCanvas.ActualHeight);

            var blackBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#000000");

            var shp = new Rectangle {Width = 50, Height = 50, Stroke = blackBrush};
            GlobCanvas.Children.Add(shp);

            //Check for Systems.Windows Canvas.SetLeft & SetTop
            Canvas.SetLeft(shp, 20);
            Canvas.SetTop(shp, 100);
            MessageBox.Show("The Shape should be present at X=20 & Y=100 acc. to System.Windows convention");

            //Check for System.Windows Shapes Height & Width
            var origHt = shp.Height;
            var origWt = shp.Width;
            shp.Width = 150;
            shp.Height = 30;

            MessageBox.Show(
                "The Shape should be of Width=150 (parallel to X axis) & Height=30 (parallel to Y axis) acc. to System.Windows convention");

            shp.Height = origHt;
            shp.Width = origWt;

            GlobCanvas.Children.Remove(shp);
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
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Black);
            rbutton.IsChecked = false;
        }

        private void ColorBorder2Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Red);
            rbutton.IsChecked = false;
        }

        private void ColorBorder3Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Green);
            rbutton.IsChecked = false;
        }

        private void ColorBorder4Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Blue);
            rbutton.IsChecked = false;
        }

        private void ColorBorder5Checked(object sender, RoutedEventArgs e)
        {
            rbutton = sender as RadioButton;
            GlobCanvas = viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "Stroke", Yellow);
            rbutton.IsChecked = false;
        }

        //Stroke Thickness Slider Control 
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float thickness = (byte) StrokeThicknessSlider.Value;

            if (thickness > 0 && viewModel != null)
                viewModel.shapeManager.CustomizeShape(GlobCanvas, viewModel.WBOps, "StrokeThickness", Black, thickness);
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
            viewModel.freeHand.SetColor(Black);
            curPenColor = Black;
        }

        private void ColorPen2Checked(object sender, RoutedEventArgs e)
        {
            viewModel.freeHand.SetColor(Red);
            curPenColor = Red;
        }

        private void ColorPen3Checked(object sender, RoutedEventArgs e)
        {
            viewModel.freeHand.SetColor(Green);
            curPenColor = Green;
        }

        private void ColorPen4Checked(object sender, RoutedEventArgs e)
        {
            viewModel.freeHand.SetColor(Blue);
            curPenColor = Blue;
        }

        private void ColorPen5Checked(object sender, RoutedEventArgs e)
        {
            viewModel.freeHand.SetColor(Yellow);
            curPenColor = Yellow;
        }

        //Stroke Thickness Slider Control 
        private void PenThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float thickness = (byte) FreeHandThicknessSlider.Value;

            if (thickness > 0)
            {
                viewModel.freeHand.SetThickness(thickness);
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
        ///     Toolbar selection tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedSelectTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
                clearSelectedShapes();
            }

            if (SelectToolBar.Visibility == Visibility.Collapsed) SelectToolBar.Visibility = Visibility.Visible;

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
        }

        /// <summary>
        ///     Toolbar Rectangle tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedRectTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
                clearSelectedShapes();
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
        }

        /// <summary>
        ///     Toolbar Ellipse tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedEllTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
                clearSelectedShapes();
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
        }

        /// <summary>
        ///     Toolbar FreeHand tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedFreehandTool(object sender, RoutedEventArgs e)
        {
            // Code for Un-Checked state
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
                clearSelectedShapes();
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Pen;
        }

        /// <summary>
        ///     Toolbar Eraser tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedEraserTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
                clearSelectedShapes();
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = ((TextBlock) Resources["CursorErase32"]).Cursor;
        }

        /// <summary>
        ///     Toolbar Line tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedLineTool(object sender, RoutedEventArgs e)
        {
            if (activeMainToolbarButton != null)
            {
                activeMainToolbarButton.Background =
                    (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeMainToolbarButton.ClearValue(BackgroundProperty);
                clearSelectedShapes();
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeMainToolbarButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            activeMainToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas.Cursor = Cursors.Arrow;
        }

        //Selection Toolbar
        /// <summary>
        ///     Fill Border Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedFillBorderTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            activeSelectToolbarButton.ClearValue(BackgroundProperty);

            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
        }

        /// <summary>
        ///     Fill Shape Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedFillShapeTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            activeSelectToolbarButton.ClearValue(BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            //viewModel.shapeManager.SetBackgroundColor();
        }

        /// <summary>
        ///     Duplicate Shape Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedDuplicateTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            activeSelectToolbarButton.ClearValue(BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);

            viewModel.shapeManager.DuplicateShape(GlobCanvas, viewModel.WBOps);
        }

        /// <summary>
        ///     Delete Shape Tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedDeleteTool(object sender, RoutedEventArgs e)
        {
            activeSelectToolbarButton = sender as Button;
            activeSelectToolbarButton.Background =
                (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            activeSelectToolbarButton.ClearValue(BackgroundProperty);
            //viewModel.ChangeActiveTool(activeMainToolbarButton.Name);
            GlobCanvas = viewModel.DeleteShape(GlobCanvas);
        }

        //Whiteboard General tools 

        /// <summary>
        ///     Clear Frame Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedClearFrame(object sender, RoutedEventArgs e)
        {
            if (Bu_P.Toggled1)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to clear frame ? All progress since the last checkpoint would be lost.",
                    "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    GlobCanvas = viewModel.ClearCanvas(GlobCanvas);
                    return;
                }

                return;
            }

            MessageBox.Show("You must be a user of high priority to call clear canvas!");
        }

        /// <summary>
        ///     Save Frame Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedSaveFrame(object sender, RoutedEventArgs e)
        {
            //this.viewModel.NumCheckpoints += 1;
            viewModel.SaveFrame();
        }

        /// <summary>
        ///     Undo Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedUndoButton(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ClickedUndo");
            viewModel.sendUndoRequest();
        }

        /// <summary>
        ///     Redo Button Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickedRedoButton(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ClickedRedo");
            viewModel.sendRedoRequest();
        }

        /// <summary>
        ///     Toggle Button Control (Canvas State Lock)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Bu.Toggled1)
            {
                viewModel.ChangeActivityState();
                ActivityBlock.Text = "Inactive";
                MessageBox.Show("Changed State to Inactive");
            }
            else
            {
                viewModel.ChangeActivityState();
                ActivityBlock.Text = "Active";
                MessageBox.Show("Changed State to Active");
            }
        }

        /// <summary>
        ///     Toggle Button Control (User Priority)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bu_PMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Bu_P.Toggled1)
            {
                viewModel.WBOps.SetUserLevel(1);
                PriorityBlock.Text = "High PR";
                MessageBox.Show("Switched to High Priority");
            }
            else
            {
                viewModel.WBOps.SetUserLevel(0);
                PriorityBlock.Text = "Medium PR";
                MessageBox.Show("Switched to Medium Priority");
            }
        }
    }
}