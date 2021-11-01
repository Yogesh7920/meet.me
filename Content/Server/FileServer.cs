using System;

namespace Content
{
    internal class FileServer
    {
        public void Receive(ReceiveMessageData messageData)
        {
            throw new NotImplementedException();
        }
        private string SaveFile(ReceiveMessageData messageData)
        {
            return "";
        }

        private ReceiveMessageData FetchFile(string file)
        {
            return new ReceiveMessageData();
        }
    }
}