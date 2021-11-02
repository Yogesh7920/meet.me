using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Content;

namespace Client.ViewModel
{
    public class ChatViewModel :
        INotifyPropertyChanged, // Notifies clients that a property value has changed.
        IContentListener // Notifies clients that has a message has been received.
    {

        IDictionary<int, Message> _messages;
        /// <summary>
        /// The received caption.
        /// </summary>
        public string ReceivedMsg
        {
            get; private set;
        }

        public string ReceivedFile
        {
            get; private set;
        }

        public ChatViewModel()
        {
            _messages = new Dictionary<int, Message>();
            _model = ContentClientFactory.getInstance();
            _model.CSubscribe(this);
        }

        public void SendChat(string message)
        {
            SendMessageData msg = new SendMessageData();
            msg.Type = MessageType.Chat;
            msg.Message = message;
            msg.ReplyThreadId = -1;
            _model.CSend(msg);
        }

        public void SendFile(string message)
        {
            SendMessageData msg = new SendMessageData();
            msg.Type = MessageType.File;
            msg.Message = message;
            msg.ReplyThreadId = -1;
            _model.CSend(msg);
        }
        public void OnMessage(ReceiveMessageData messageData)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<string, string>((path, text) =>
                        {
                            lock (this)
                            {

                                if(messageData.Event == MessageEvent.NewMessage)
                                {
                                    if (messageData.Type == MessageType.File)
                                    {
                                        this.ReceivedFile = messageData.Message;
                                        this.OnPropertyChanged("ReceivedFile");
                                    }
                                    else
                                    {
                                        this.ReceivedMsg = messageData.Message;
                                        this.OnPropertyChanged("ReceivedMsg");
                                    }
                                }
                                else if(messageData.Event == MessageEvent.Star)
                                {

                                }
                                else if(messageData.Event == MessageEvent.Download)
                                {

                                }

                            }
                        }),
                        messageData);
        }

        public void OnAllMessages(List<Thread> allMessages)
        {

        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Underlying data model.
        /// </summary>
        private IContentClient _model;
    }
}
