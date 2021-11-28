using System;
using System.Diagnostics;
using System.IO;
using Networking; /// <author>Yuvraj Raghuvanshi</author>
/// <created>1/11/2021</created>

namespace Content
{
    internal class FileClient
    {
        private readonly ISerializer _serializer;

        public FileClient(ICommunicator communicator)
        {
            _serializer = new Serializer();
            Communicator = communicator;
        }

        public ICommunicator Communicator { get; set; }

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

            try
            {
                var toSendSerialized = _serializer.Serialize(toSend);

                // send the message
                Trace.WriteLine("[FileClient] Sending the file to server");
                Communicator.Send(toSendSerialized, "Content");
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    $"[FileClient] Exception encountered during sending data: {e.GetType().Name}: {e.Message}");
            }
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

            try
            {
                // serialize the message and send via network
                var toSendSerialized = _serializer.Serialize(toSend);
                Trace.WriteLine("[FileClient] Sending file download request to server");
                Communicator.Send(toSendSerialized, "Content");
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    $"[FileClient] Exception encountered during sending download request: {e.GetType().Name}: {e.Message}");
            }
        }
    }
}