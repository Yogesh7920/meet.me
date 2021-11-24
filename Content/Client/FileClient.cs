using Networking;
/// <author>Yuvraj Raghuvanshi</author>
/// <created>1/11/2021</created>
using System;
using System.Diagnostics;
using System.IO;

namespace Content
{
    internal class FileClient
    {
        private ICommunicator _communicator;
        public ICommunicator Communicator
        {
            get => _communicator;
            set => _communicator = value;
        }

        private readonly ISerializer _serializer;

        public FileClient(ICommunicator communicator)
        {
            _serializer = new Serializer();
            _communicator = communicator;
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

            if (!File.Exists(filepath)) throw new FileNotFoundException("File " + filepath + " not found");

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
            toSend.SenderId = UserId;
            // serialize the message and send via network
            var toSendSerialized = _serializer.Serialize(toSend);
            Trace.WriteLine("[FileClient] Sending file download request to server");
            _communicator.Send(toSendSerialized, "Content");
        }
    }
}