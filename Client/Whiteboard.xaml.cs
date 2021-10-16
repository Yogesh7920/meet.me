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
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WhiteBoardView : Window
    {
        public WhiteBoardView()
        {
            InitializeComponent();
            WhiteBoardViewModel viewModel = new WhiteBoardViewModel();
        }

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

        private void OnToolbarButtonClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ClickedSelectTool(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickedSelectTool");
            return;
        }
        private void ClickedRectTool(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickedRectTool");
            return;
        }
        private void ClickedEllTool(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickedEllTool");
            return;
        }
        private void ClickedFreehandTool(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickedFreehandTool");
            return;
        }
        private void ClickedEraserTool(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ClickedEraserTool");
            return;
        }
    }
}
