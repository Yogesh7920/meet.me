using System;
using System.Windows;
using System.Windows.Input;
using Client.ViewModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace Client
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
        public DashboardViewModel DashboardVM { get; set; }

        public DashboardView()
        {
            InitializeComponent();
            this.DashboardVM = new DashboardViewModel();
            this.DataContext = DashboardVM;
        }

        /// <summary>
        /// Copies the summary text to the clipboard 
        /// which can be used further for any purpose.
        /// </summary>
        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ChatSummary.Text);
        }

        /// <summary>
        /// Refreshes the dashboard window  with latest analytics
        /// </summary>
        private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            this.DashboardVM.UpdateVM();
            usersVsTimeChart.Series[0].Values = this.DashboardVM.usersCountList;
            usersVsTimeChart.AxisX[0].Labels = this.DashboardVM.timestampList;
            usersVsTimeChart.Update();

            usersVsMessagesPlot.AxisX[0].Labels = this.DashboardVM.usersList;
            usersVsMessagesPlot.Series[0].Values = this.DashboardVM.messagesCountList;
            usersVsMessagesPlot.Update();
        }

        /// <summary>
        /// Window Minimize Functionality
        /// </summary>
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Window Maximize Functionality
        /// </summary>
        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.WindowState = WindowState.Maximized;
            MaximizeButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Window Restore Functionality
        /// </summary>
        private void OnRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Window Close Functionality
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Window Drag Functionality
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }

}
