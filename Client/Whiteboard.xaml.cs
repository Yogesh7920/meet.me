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
    }
}
