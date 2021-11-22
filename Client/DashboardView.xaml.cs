using Client.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
        public DashboardViewModel DashboardVM { get; set; }

        public DashboardView()
        {
            InitializeComponent();
            //DashboardViewModel DashboardVM = new DashboardViewModel();
            DashboardVM = new DashboardViewModel();
            DataContext = DashboardVM;
        }

        /// <summary>
        /// Closes the dashboard window when the user clicks on the Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Refreshes the dashboard window by invoking UpdateVM method of DashboardViewModel
        /// when the user clicks on the refresh button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            this.DashboardVM.UpdateVM();
        }

        /// <summary>
        /// To drag the window from titlebar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// Copies the summary text to the clipboard 
        /// which can be used further for any purpose.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ChatSummary.Text);
        }
    }
}
