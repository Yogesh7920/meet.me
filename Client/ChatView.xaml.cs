using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Client.ViewModel;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public int ReplyMsgId { get; set; }
        
        ObservableCollection<Message> allmessages;
        public ChatView()
        {
            InitializeComponent();

            ChatViewModel viewModel = new ChatViewModel();
            //subscribe to the property changed event
            viewModel.PropertyChanged += Listner;
            this.DataContext = viewModel;

            allmessages = new ObservableCollection<Message>();
            //allmessages.Add(new Message { TextMessage = "To File Check", Type = false, Time = DateTime.Now.ToShortTimeString(), ToFrom = true });
            //allmessages.Add(new Message { TextMessage = "From Msg Check", Type = true, Time = DateTime.Now.ToShortTimeString(), ToFrom = false });
            //allmessages.Add(new Message { TextMessage = "From File Check", Type = false, Time = DateTime.Now.ToShortTimeString(), ToFrom = false });
            //allmessages.Add(new Message { TextMessage = "To Msg check", Type = true, Time = DateTime.Now.ToShortTimeString(), ToFrom = true });
            this.myChat.ItemsSource = allmessages;
        }
        private void Listner(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            ChatViewModel viewModel = this.DataContext as ChatViewModel;
            if (propertyName == "ReceivedMsg")
            {
                allmessages.Add(viewModel.ReceivedMsg);
            }
            UpdateScrollBar(myChat);
        }
        private void OnSentButtonClick(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(this.SendTextBox.Text);
            if (!string.IsNullOrEmpty(this.SendTextBox.Text))
            {
                ChatViewModel viewModel = this.DataContext as ChatViewModel;
                if (string.IsNullOrEmpty(this.ReplyTextBox.Text))
                {
                    viewModel.SendChat(this.SendTextBox.Text, -1);
                }
                else
                {
                    viewModel.SendChat(this.SendTextBox.Text, ReplyMsgId);
                }
                this.SendTextBox.Text = string.Empty;
                this.ReplyTextBox.Text = "";
            }
        }
        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            ChatViewModel viewModel = this.DataContext as ChatViewModel;
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                if (string.IsNullOrEmpty(this.ReplyTextBox.Text))
                {
                    viewModel.SendFile(openFileDlg.FileName, -1);
                }
                else
                {
                    viewModel.SendFile(openFileDlg.FileName, ReplyMsgId);
                }
                this.ReplyTextBox.Text = "";
            }
        }
        private void OnReplyButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button cmd = (Button)sender;
                if (cmd.DataContext is Message)
                {
                    Message m = (Message)cmd.DataContext;
                    this.ReplyTextBox.Text = m.TextMessage;
                    ReplyMsgId = m.MessageId;
                    //System.Diagnostics.Debug.WriteLine(m.TextMessage);
                }
            }
        }
        private void OnStarButtonClick(object sender, RoutedEventArgs e)
        {
            ChatViewModel viewModel = this.DataContext as ChatViewModel;
            if (sender is RadioButton)
            {
                RadioButton cmd = (RadioButton)sender;
                if (cmd.DataContext is Message)
                {
                    Message m = (Message)cmd.DataContext;
                    viewModel.StarChat(m.MessageId);
                    //System.Diagnostics.Debug.WriteLine(m.TextMessage);
                }
            }
        }
        private void OnDownloadButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ChatViewModel viewModel = this.DataContext as ChatViewModel;
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".text"; // Default file extension
                //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                Button cmd = (Button)sender;
                if (cmd.DataContext is Message)
                {
                    Message m = (Message)cmd.DataContext;

                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        //string filename = dlg.FileName;
                        //System.Diagnostics.Debug.WriteLine(dlg.FileName);
                        //System.Diagnostics.Debug.WriteLine(m.TextMessage);
                        viewModel.DownloadFile(m.MessageId, dlg.FileName);
                    }
                }
            }
            
        }
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this).DragMove();
            }
        }
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
