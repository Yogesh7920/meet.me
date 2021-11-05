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
using Whiteboard;

namespace Client
{
    /// <summary>
    /// Interaction logic for Whiteboard.xaml
    /// </summary>
    public partial class WhiteBoardView : Window
    {
        private Button activeButton;
        private WhiteBoardViewModel viewModel;
        public Canvas GlobCanvas;
        private string buttonDefaultColor = "#D500F9";
        private string buttonSelectedColor = "#007C9C";


        public WhiteBoardView()
        {
            InitializeComponent();
            this.GlobCanvas = MyCanvas; 
            viewModel = new WhiteBoardViewModel(GlobCanvas);
        }

        // Canvas Mouse actions 
        private void OnCanvasMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
           if (e.LeftButton == MouseButtonState.Pressed){
                this.viewModel.start = e.GetPosition(MyCanvas);
           }
            
        }

        private void OnCanvasMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCanvasMouseMove(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenPopupButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SetBGButtonPopUp.StaysOpen = true;
        }

        private void OpenPopupButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SetBGButtonPopUp.StaysOpen = false;
        }

        private void OpenPopupRestoreButton_MouseEnter(object sender, MouseEventArgs e)
        {
            RestoreFramePopUp.StaysOpen = true;
        }

        private void OpenPopupRestoreButton_MouseLeave(object sender, MouseEventArgs e)
        {
            RestoreFramePopUp.StaysOpen = false;
        }

        //Toolbar selection tool 
        private void ClickedSelectTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeButton.ClearValue(Button.BackgroundProperty);
            }

            if (this.SelectToolBar.Visibility == Visibility.Collapsed)
            {
                this.SelectToolBar.Visibility = Visibility.Visible;
            }

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeButton.Name);
            return;
        }
        private void ClickedRectTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeButton.ClearValue(Button.BackgroundProperty);
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeButton.Name);
            return;
        }
        private void ClickedEllTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeButton.ClearValue(Button.BackgroundProperty);
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeButton.Name);
            return;
        }
        private void ClickedFreehandTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeButton.ClearValue(Button.BackgroundProperty);
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeButton.Name);
            return;
        }
        private void ClickedEraserTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeButton.ClearValue(Button.BackgroundProperty);
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeButton.Name);
            return;
        }

        private void ClickedLineTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonDefaultColor));
                activeButton.ClearValue(Button.BackgroundProperty);
            }

            if (this.SelectToolBar.Visibility == Visibility.Visible)
            {
                this.SelectToolBar.Visibility = Visibility.Collapsed;
            }

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(buttonSelectedColor));
            viewModel.ChangeActiveTool(activeButton.Name);
            return;
        }

        //Radio Button (Set Background)
        private void ColorBtn1Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg1);
        }

        private void ColorBtn2Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg2);
        }

        private void ColorBtn3Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg3);
        }

        private void ColorBtn4Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg4);
        }

        private void ColorBtn5Checked(object sender, RoutedEventArgs e)
        {
            GlobCanvas = viewModel.ChangeWbBackground(GlobCanvas, canvasBg5);
        }


        //Whiteboard General tools 
        private void ClickedClearFrame(object sender, RoutedEventArgs e)
        {
            GlobCanvas.Children.Clear();
            return;
        }

        private void ClickedSaveFrame(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedSaveFrame");
            return;
        }

        private void ClickedUndoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedUndo");
            return;
        }

        private void ClickedRedoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedRedo");
            return;
        }

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
        //Parent Window click event
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}