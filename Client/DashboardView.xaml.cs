/// <author>Mitul Kataria</author>
/// <created>19/11/2021</created>
/// <summary>
///		This file consists of DashboardView class
///		which handles the structure, layout and appearance of 
///		what a user sees on the Dashboard screen.
///		It displays a representation of the Dashboard model and 
///		receives the user's interaction and it forwards the handling of 
///		these to the DashboardViewModel through the data binding.
/// </summary>

using System.Windows;
using System.Windows.Input;
using Client.ViewModel;

namespace Client
{
    /// <summary>
    ///     Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();
            DashboardVM = new DashboardViewModel();
            DataContext = DashboardVM;
        }

        public DashboardViewModel DashboardVM { get; set; }

        /// <summary>
        ///     Copies the summary text to the clipboard
        ///     which can be used further for any purpose.
        /// </summary>
        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ChatSummary.Text);
        }

        /// <summary>
        ///     Refreshes the dashboard window  with latest analytics
        /// </summary>
        private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            DashboardVM.UpdateVM();
            usersVsTimeChart.Series[0].Values = DashboardVM.usersCountList;
            usersVsTimeChart.AxisX[0].Labels = DashboardVM.timestampList;
            usersVsTimeChart.Update();

            usersVsMessagesPlot.AxisX[0].Labels = DashboardVM.usersList;
            usersVsMessagesPlot.Series[0].Values = DashboardVM.messagesCountList;
            usersVsMessagesPlot.Update();
        }

        /// <summary>
        ///     Window Minimize Functionality
        /// </summary>
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        ///     Window Maximize Functionality
        /// </summary>
        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            WindowState = WindowState.Maximized;
            MaximizeButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Window Restore Functionality
        /// </summary>
        private void OnRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Window Close Functionality
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Window Drag Functionality
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}