/// <author>Suchitra Yechuri</author>
/// <created>13/10/2021</created>
/// <summary>
///     Interaction logic for ChatView.xaml.
/// </summary>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.ViewModel;
using Microsoft.Win32;

namespace Client
{
    public partial class ChatView : UserControl
    {
        /// <summary>
        ///     Collection of all the messages.
        /// </summary>
        private readonly ObservableCollection<Message> _allMessages;

        /// <summary>
        ///     Creates an instance of the ChatView.
        /// </summary>
        public ChatView()
        {
            InitializeComponent();

            var viewModel = new ChatViewModel();

            // Subscribe to the property changed event
            viewModel.PropertyChanged += Listner;
            DataContext = viewModel;

            _allMessages = new ObservableCollection<Message>();
            // Binding AllMessages
            myChat.ItemsSource = _allMessages;
        }

        /// <summary>
        ///     Message Id of the message replied it.
        /// </summary>
        private int _replyMsgId { get; set; }

        /// <summary>
        ///     Updates the view with the new message on property changed event.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The property changed event </param>
        private void Listner(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            var viewModel = DataContext as ChatViewModel;

            if (propertyName == "ReceivedMsg")
            {
                _allMessages.Add(viewModel.ReceivedMsg);
                UpdateScrollBar(myChat);
            }
            else if (propertyName == "ReceivedMsgs")
            {
                _allMessages.Add(viewModel.ReceivedMsg);
            }
        }

        /// <summary>
        ///     Handles the sent button click.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The routed event data </param>
        private void OnSentButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SendTextBox.Text))
            {
                var viewModel = DataContext as ChatViewModel;

                // If the reply text box is empty then the message 
                // is not a reply message, else send the reply message id
                if (string.IsNullOrEmpty(ReplyTextBox.Text))
                    viewModel.SendChat(SendTextBox.Text, -1);
                else
                    viewModel.SendChat(SendTextBox.Text, _replyMsgId);

                SendTextBox.Text = string.Empty;
                ReplyTextBox.Text = "";
            }
        }

        /// <summary>
        ///     Handles the upload button click.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The routed event data </param>
        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatViewModel;
            // Create OpenFileDialog
            var openFileDlg = new OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            var result = openFileDlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                if (string.IsNullOrEmpty(ReplyTextBox.Text))
                    viewModel.SendFile(openFileDlg.FileName, -1);
                else
                    viewModel.SendFile(openFileDlg.FileName, _replyMsgId);
                ReplyTextBox.Text = "";
            }
        }

        /// <summary>
        ///     Handles the reply button click.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The routed event data </param>
        private void OnReplyButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var cmd = (Button) sender;
                if (cmd.DataContext is Message)
                {
                    // Get the message corresponding to the particular reply button
                    var m = (Message) cmd.DataContext;

                    // Display the message in the reply text box
                    ReplyTextBox.Text = m.TextMessage;

                    // Set the reply message id
                    _replyMsgId = m.MessageId;
                }
            }
        }

        /// <summary>
        ///     Handles the star button click.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The routed event data </param>
        private void OnStarButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatViewModel;

            if (sender is RadioButton)
            {
                var cmd = (RadioButton) sender;
                if (cmd.DataContext is Message)
                {
                    // Get the message corresponding to the particular star button
                    var m = (Message) cmd.DataContext;

                    viewModel.StarChat(m.MessageId);
                }
            }
        }

        /// <summary>
        ///     Handles the download button click.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The routed event data </param>
        private void OnDownloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var viewModel = DataContext as ChatViewModel;

                // Create SaveFileDialog
                var dlg = new SaveFileDialog();

                var cmd = (Button) sender;
                if (cmd.DataContext is Message)
                {
                    // Get the message corresponding to the particular download button
                    var m = (Message) cmd.DataContext;

                    // Set the default file name and extension
                    dlg.FileName = Path.GetFileNameWithoutExtension(m.TextMessage);
                    dlg.DefaultExt = Path.GetExtension(m.TextMessage);

                    // Show save file dialog box
                    var result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true) viewModel.DownloadFile(m.MessageId, dlg.FileName);
                }
            }
        }

        /// <summary>
        ///     Updates the scrollbar to the bottom of the listbox
        /// </summary>
        /// <param name="listBox"> The listbox consisting of the scrollbar </param>
        private void UpdateScrollBar(ListBox listBox)
        {
            if ((listBox != null) && (VisualTreeHelper.GetChildrenCount(listBox) != 0))
            {
                var border = (Border) VisualTreeHelper.GetChild(listBox, 0);
                var scrollViewer = (ScrollViewer) VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
    }
}