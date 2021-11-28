/// <author>Suchitra Yechuri</author>
/// <created>13/10/2021</created>
/// <summary>
///     Interaction logic for ChatView.xaml.
/// </summary>

using Client.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client
{
    public partial class ChatView : UserControl
    {
        /// <summary>
        ///     Message Id of the message replied it.
        /// </summary>
        private int _replyMsgId { get; set; }

        /// <summary>
        ///     Collection of all the messages.
        /// </summary>
        private ObservableCollection<Message> _allMessages;

        /// <summary>
        ///     Creates an instance of the ChatView.
        /// </summary>
        public ChatView()
        {
            InitializeComponent();

            ChatViewModel viewModel = new ChatViewModel();

            // Subscribe to the property changed event
            viewModel.PropertyChanged += Listner;
            this.DataContext = viewModel;

            _allMessages = new ObservableCollection<Message>();
            // Binding AllMessages
            this.myChat.ItemsSource = _allMessages;
        }

        /// <summary>
        ///     Updates the view with the new message on property changed event.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The property changed event </param>
        private void Listner(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            ChatViewModel viewModel = this.DataContext as ChatViewModel;

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
            if (!string.IsNullOrEmpty(this.SendTextBox.Text))
            {
                ChatViewModel viewModel = this.DataContext as ChatViewModel;

                // If the reply text box is empty then the message 
                // is not a reply message, else send the reply message id
                if (string.IsNullOrEmpty(this.ReplyTextBox.Text))
                {
                    viewModel.SendChat(this.SendTextBox.Text, -1);
                }
                else
                {
                    viewModel.SendChat(this.SendTextBox.Text, _replyMsgId);
                }

                this.SendTextBox.Text = string.Empty;
                this.ReplyTextBox.Text = "";
            }
        }

        /// <summary>
        ///     Handles the upload button click.
        /// </summary>
        /// <param name="sender"> The sender of the notification </param>
        /// <param name="e"> The routed event data </param>
        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            ChatViewModel viewModel = this.DataContext as ChatViewModel;
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                if (string.IsNullOrEmpty(this.ReplyTextBox.Text))
                {
                    viewModel.SendFile(openFileDlg.FileName, -1);
                }
                else
                {
                    viewModel.SendFile(openFileDlg.FileName, _replyMsgId);
                }
                this.ReplyTextBox.Text = "";
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
                Button cmd = (Button)sender;
                if (cmd.DataContext is Message)
                {
                    // Get the message corresponding to the particular reply button
                    Message m = (Message)cmd.DataContext;

                    // Display the message in the reply text box
                    this.ReplyTextBox.Text = m.TextMessage;

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
            ChatViewModel viewModel = this.DataContext as ChatViewModel;

            if (sender is RadioButton)
            {
                RadioButton cmd = (RadioButton)sender;
                if (cmd.DataContext is Message)
                {
                    // Get the message corresponding to the particular star button
                    Message m = (Message)cmd.DataContext;

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
                ChatViewModel viewModel = this.DataContext as ChatViewModel;

                // Create SaveFileDialog
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                Button cmd = (Button)sender;
                if (cmd.DataContext is Message)
                {
                    // Get the message corresponding to the particular download button
                    Message m = (Message)cmd.DataContext;

                    // Set the default file name and extension
                    dlg.FileName = Path.GetFileNameWithoutExtension(m.TextMessage);
                    dlg.DefaultExt = Path.GetExtension(m.TextMessage);

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    // Process save file dialog box results
                    if (result == true)
                    {
                        viewModel.DownloadFile(m.MessageId, dlg.FileName);
                    }
                }
            }
            
        }

        /// <summary>
        ///     Updates the scrollbar to the bottom of the listbox
        /// </summary>
        /// <param name="listBox"> The listbox consisting of the scrollbar </param>
        private void UpdateScrollBar(ListBox listBox)
        {
            if (listBox != null)
            {
                var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
    }
}
