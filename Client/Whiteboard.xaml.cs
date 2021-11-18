using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class WhiteBoardView : Window
    {
        private Button activeButton;
        private readonly string buttonDefaultColor = "#D500F9";
        private readonly string buttonSelectedColor = "#007C9C";

        public WhiteBoardView()
        {
            InitializeComponent();
            var viewModel = new WhiteBoardViewModel();
        }

        // Canvas Mouse actions 
        private void OnCanvasMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
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
                activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Collapsed) SelectToolBar.Visibility = Visibility.Visible;

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            MessageBox.Show("ClickedSelectTool");
        }

        private void ClickedRectTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            MessageBox.Show("ClickedRectTool");
        }

        private void ClickedEllTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            MessageBox.Show("ClickedEllTool");
        }

        private void ClickedFreehandTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            MessageBox.Show("ClickedFreehandTool");
        }

        private void ClickedEraserTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            MessageBox.Show("ClickedEraserTool");
        }

        private void ClickedLineTool(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonDefaultColor);
                activeButton.ClearValue(BackgroundProperty);
            }

            if (SelectToolBar.Visibility == Visibility.Visible) SelectToolBar.Visibility = Visibility.Collapsed;

            activeButton = sender as Button;
            activeButton.Background = (SolidColorBrush) new BrushConverter().ConvertFrom(buttonSelectedColor);
            MessageBox.Show("ClickedLineTool");
        }

        //Whiteboard General tools 
        private void ClickedSetBG(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedSetBG");
        }

        private void ClickedClearFrame(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedClearFrame");
        }

        private void ClickedSaveFrame(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedSaveFrame");
        }

        private void ClickedUndoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedUndo");
        }

        private void ClickedRedoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClickedRedo");
        }

        private void Bu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Bu.Toggled1)
                MessageBox.Show("Toggled On");
            else
                MessageBox.Show("Toggled Off");
        }

        //Parent Window click event
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}