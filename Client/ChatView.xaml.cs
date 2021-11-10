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
    public partial class ChatView : Window
    {
        public string ReplyMsg { get; set; }
        public int ReplyMsgId { get; set; }
        public string FilePath { get; set; }
        //public string FileName { get; set; }
        ObservableCollection<Message> allmessages;
        public ChatView()
        {
            InitializeComponent();

            ChatViewModel viewModel = new ChatViewModel();
            //subscribe to the property changed event
            viewModel.PropertyChanged += Listner;
            this.DataContext = viewModel;

            allmessages = new ObservableCollection<Message>();
            allmessages.Add(new Message { TextMessage = "To File Check", Type = false, Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = true });
            allmessages.Add(new Message { TextMessage = "From Msg Check", Type = true, Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = false });
            allmessages.Add(new Message { TextMessage = "From File Check", Type = false, Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = false });
            allmessages.Add(new Message { TextMessage = "To Msg check", Type = true, Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = true });
            this.myChat.ItemsSource = allmessages;

            //System.Diagnostics.Debug.WriteLine(allmessages.Count);
        }
        private void Listner(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            ChatViewModel viewModel = this.DataContext as ChatViewModel;
            if (propertyName == "ReceivedFile")
            {
                allmessages.Add(new Message { TextMessage = viewModel.ReceivedMsg, Type = false ,Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = false });
            }
            else if(propertyName == "ReceivedMsg")
            {
                allmessages.Add(new Message { TextMessage = viewModel.ReceivedMsg, Type = true, Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = false });
            }
            UpdateScrollBar(myChat);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                FilePath = openFileDlg.FileName;
                allmessages.Add(new Message { TextMessage = FilePath.ToString(), Type = false, Time = DateTime.Now.ToString(), Status = "Sent", ToFrom = true });
                //TextBlock1.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }
        private void OnSentButtonClick(object sender, RoutedEventArgs e)
        {
            ChatViewModel viewModel = this.DataContext as ChatViewModel;
            if (String.IsNullOrEmpty(ReplyMsg))
            {
                viewModel.SendChat(this.SendTextBox.Text, -1);
            }
            else
            {
                viewModel.SendChat(this.SendTextBox.Text, ReplyMsgId);
            }
            ReplyMsg = "";
        }
        private void OnReplyButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button cmd = (Button)sender;
                if (cmd.DataContext is Message)
                {
                    Message m = (Message)cmd.DataContext;
                    ReplyMsg = m.TextMessage;
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
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
