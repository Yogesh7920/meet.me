using System;

namespace Content
{
    internal class FileServer
    {
        public void Receive(MessageData messageData)
        {
            throw new NotImplementedException();
        }
        private string SaveFile(MessageData messageData)
        {
            return "";
        }

        private MessageData FetchFile(string file)
        {
            return new MessageData();
        }
    }
}