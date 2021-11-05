using System;
using System.IO;
using Networking;
using MongoDB.Bson;

namespace Content
{
    class FileClient
    {
        private int userId;
        public int UserId { get => userId; set => userId = value; }
        
        private ISerializer serializer = new Serializer();
        private ICommunicator communicator = CommunicationFactory.GetCommunicator();

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
            toSend.SenderId = userId;
            toSend.ReceiverIds = message.ReceiverIds;
            toSend.ReplyThreadId = ObjectId.Empty;
            toSend.SentTime = DateTime.Now;
            toSend.Starred = false;

            toSend.FileData = filedata;

            // serialize the message
            string toSendSerialized = serializer.Serialize(toSend);

            // send the message
            communicator.Send(toSendSerialized, "Content");
        }


    }
}