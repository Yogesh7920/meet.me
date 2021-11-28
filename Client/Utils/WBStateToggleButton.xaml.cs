///Adapted from https://github.com/TacticDevGit/C-WPf-Toggle-Switch-UI-Control/tree/master/ToggleSwitch/ToggleSwitch

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    ///     Interaction logic for ToggleBuhtton.xaml
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        private readonly Thickness LeftSide = new(-39, 0, 0, 0);
        private readonly SolidColorBrush Off = new(Color.FromRgb(160, 160, 160));
        private readonly SolidColorBrush On = new(Color.FromRgb(160, 160, 160));
        private readonly Thickness RightSide = new(0, 0, -39, 0);

        public ToggleButton()
        {
            InitializeComponent();
            Back.Fill = Off;
            Toggled1 = false;
            Dot.Margin = LeftSide;
        }

        public bool Toggled1 { get; set; }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled1)
            {
                Back.Fill = new SolidColorBrush((Color)Application.Current.Resources["Color4"]);
                Toggled1 = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Toggled1 = false;
                Dot.Margin = LeftSide;
            }
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled1)
            {
                Back.Fill = new SolidColorBrush((Color)Application.Current.Resources["Color4"]);
                Toggled1 = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Toggled1 = false;
                Dot.Margin = LeftSide;
            }
        }
    }
}