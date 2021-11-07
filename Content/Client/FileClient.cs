using System;
using System.IO;
using Networking;
using MongoDB.Bson;

namespace Content
{
    class FileClient
    {
        private int _userId;
        public int UserId { get => _userId; set => _userId = value; }
        
        private ISerializer _serializer;
        private ICommunicator _communicator;
        
        public FileClient()
        {
            _serializer = new Serializer();
            _communicator = CommunicationFactory.GetCommunicator();
        }

        /// <summary>
        /// Send a file type message to the server
        /// </summary>
        /// <param name="message">SendMessageData object specifying the file message to send</param>
        public void Send(SendMessageData message)
        {
            if (message.Type != MessageType.File)
            {
                throw new ArgumentException("Message argument to FileClient::Send does not have MessageType File");
            }

            // check if file with given file path exists
            string filepath = message.Message;

            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("File {0} not found", filepath);
            }

            // initialize a MessageData object that will be sent to the server
            MessageData toSend = new MessageData();
            SendFileData filedata = new SendFileData(filepath);

            // set toSend's fields appropriately
            toSend.Event = MessageEvent.NewMessage;
            toSend.Type = MessageType.File;
            toSend.MessageId = ObjectId.Empty;
            toSend.Message = filedata.fileName;
            toSend.SenderId = _userId;
            toSend.ReceiverIds = message.ReceiverIds;
            toSend.ReplyThreadId = ObjectId.Empty;
            toSend.SentTime = DateTime.Now;
            toSend.Starred = false;

            toSend.FileData = filedata;

            // serialize the message
            string toSendSerialized = _serializer.Serialize(toSend);

            // send the message
            _communicator.Send(toSendSerialized, "Content");
        }

        public void Download(ObjectId messageId, string savepath)
        {
            MessageData toSend = new MessageData();

            toSend.Event = MessageEvent.Download;
            toSend.Type = MessageType.File;
            toSend.MessageId = messageId;
            toSend.Message = savepath;
            toSend.FileData = null;

            // serialize the message and send via network
            string toSendSerialized = _serializer.Serialize(toSend);
            _communicator.Send(toSendSerialized, "Content");
        }
    }
}