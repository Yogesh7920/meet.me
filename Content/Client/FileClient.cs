using System;
using System.Diagnostics;
using System.IO;
using Networking;

namespace Content
{
    internal class FileClient
    {
        private readonly ICommunicator _communicator;

        private readonly ISerializer _serializer;

        public FileClient()
        {
            _serializer = new Serializer();
            _communicator = CommunicationFactory.GetCommunicator();
        }

        public int UserId { get; set; }

        /// <summary>
        ///     Send a file type message to the server
        /// </summary>
        /// <param name="message">SendMessageData object specifying the file message to send</param>

        public void Send(SendMessageData message)
        {
            Trace.WriteLine("[FileClient] Received file send request");
            if (message.Type != MessageType.File)
                throw new ArgumentException("Message argument to FileClient::Send does not have MessageType File");

            // check if file with given file path exists
            var filepath = message.Message;

            if (!File.Exists(filepath)) throw new FileNotFoundException("File {0} not found", filepath);

            // initialize a MessageData object that will be sent to the server
            var toSend = new MessageData();
            var filedata = new SendFileData(filepath);

            // set toSend's fields appropriately
            toSend.Event = MessageEvent.NewMessage;
            toSend.Type = MessageType.File;
            toSend.MessageId = -1;
            toSend.Message = filedata.fileName;
            toSend.SenderId = UserId;
            toSend.ReceiverIds = message.ReceiverIds;
            toSend.ReplyThreadId = -1;
            toSend.SentTime = DateTime.Now;
            toSend.Starred = false;

            toSend.FileData = filedata;

            // serialize the message
            Trace.WriteLine("[FileClient] Serializing the file data");
            var toSendSerialized = _serializer.Serialize(toSend);

            // send the message
            Trace.WriteLine("[FileClient] Sending the file to server");
            _communicator.Send(toSendSerialized, "Content");
        }

        public void Download(int messageId, string savepath)
        {
            var toSend = new MessageData();

            toSend.Event = MessageEvent.Download;
            toSend.Type = MessageType.File;
            toSend.MessageId = messageId;
            toSend.Message = savepath;
            toSend.FileData = null;

            // serialize the message and send via network
            var toSendSerialized = _serializer.Serialize(toSend);
            Trace.WriteLine("[FileClient] Sending file download request to server");
            _communicator.Send(toSendSerialized, "Content");
        }
    }
}