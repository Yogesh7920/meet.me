using Content;

namespace Testing.Content
{
    ///<summary>
    /// This file will contain required sample datastructures to test files and modules
    /// </summary>
    public class Utils
    {
        private FakeCommunicator _fakeCommunicator;

        public Utils()
        {
            _fakeCommunicator = new FakeCommunicator();
        }

        public FakeCommunicator GetFakeCommunicator()
        {
            return _fakeCommunicator;
        }

        public SendMessageData GenerateChatSendMsgData(string msg = "Hello", int[] rcvIds = null, int replyId = -1, MessageType type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            var toConvert = new SendMessageData();
            toConvert.Message = msg;
            toConvert.Type = type;
            toConvert.ReplyThreadId = replyId;
            toConvert.ReceiverIds = rcvIds;
            return toConvert;
        }

        public MessageData GenerateChatMessageData(MessageEvent chatEvent = MessageEvent.NewMessage, string msg = "Hello", int[] rcvIds = null, int replyId = -1, MessageType type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            SendMessageData SampleData = GenerateChatSendMsgData(msg, rcvIds, replyId, type);
            ChatClient contentChatClient = new ChatClient(_fakeCommunicator);
            MessageData MsgData = contentChatClient.SendToMessage(SampleData, chatEvent);
            return MsgData;
        }

        public MessageData GenerateNewMessageData(string Message, int MessageId = 1, int[] rcvIds = null, int ReplyThreadId = -1, int SenderId = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Message = Message;
            msg.MessageId = MessageId;
            msg.ReceiverIds = rcvIds;
            msg.SenderId = SenderId;
            msg.ReplyThreadId = ReplyThreadId;
            msg.Starred = Starred;
            msg.Type = Type;
            return msg;
        }

        public ReceiveMessageData GenerateNewReceiveMessageData(string Message, int MessageId = 1, int[] rcvIds = null, int ReplyThreadId = -1, int SenderId = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            var msg = new ReceiveMessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Message = Message;
            msg.MessageId = MessageId;
            msg.ReceiverIds = rcvIds;
            msg.SenderId = SenderId;
            msg.ReplyThreadId = ReplyThreadId;
            msg.Starred = Starred;
            msg.Type = Type;
            return msg;
        }


        public MessageData GenerateStarMessageData(string Message, int MessageId = 1, int[] rcvIds = null, int ReplyThreadId = -1, int SenderId = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Star;
            msg.Message = Message;
            msg.MessageId = MessageId;
            msg.ReceiverIds = rcvIds;
            msg.SenderId = SenderId;
            msg.ReplyThreadId = ReplyThreadId;
            msg.Starred = Starred;
            msg.Type = Type;
            return msg;
        }

        public MessageData GenerateUpdateMessageData(string Message, int MessageId = 1, int[] rcvIds = null, int ReplyThreadId = -1, int SenderId = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.Message = Message;
            msg.MessageId = MessageId;
            msg.ReceiverIds = rcvIds;
            msg.SenderId = SenderId;
            msg.ReplyThreadId = ReplyThreadId;
            msg.Starred = Starred;
            msg.Type = Type;
            return msg;
        }

        public MessageData GenerateDownloadMessageData(string SavePath, SendFileData FileData, int MessageId = 1, int[] rcvIds = null, int ReplyThreadId = -1, int SenderId = -1, bool Starred = false, MessageType Type = MessageType.Chat)
        {
            if (rcvIds == null)
            {
                rcvIds = new int[0];
            }
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Download;
            msg.Message = SavePath;
            msg.MessageId = MessageId;
            msg.ReceiverIds = rcvIds;
            msg.SenderId = SenderId;
            msg.ReplyThreadId = ReplyThreadId;
            msg.Starred = Starred;
            msg.Type = Type;
            msg.FileData = FileData;
            return msg;
        }

        public SendMessageData GetSendMessageData1()
        {
            var toconvert1 = new SendMessageData();
            toconvert1.Message = "Hello";
            toconvert1.Type = MessageType.Chat;
            toconvert1.ReplyThreadId = -1;
            toconvert1.ReceiverIds = new int[0];
            return toconvert1;
        }

        public MessageData GetMessageData1()
        {
            SendMessageData SampleData = GetSendMessageData1();
            ChatClient conch = new ChatClient(_fakeCommunicator);
            MessageData MsgData = conch.SendToMessage(SampleData, MessageEvent.NewMessage);
            return MsgData;
        }

        public SendMessageData GetSendMessageData2()
        {
            var toconvert2 = new SendMessageData();
            toconvert2.Message = null;
            toconvert2.Type = MessageType.Chat;
            toconvert2.ReplyThreadId = -1;
            toconvert2.ReceiverIds = new int[0];
            return toconvert2;
        }

        ///<summary>
        /// We need output string from server to trigger INotificationHandler function so that we can deserialized it and update chatContext map
        /// </summary>
    }
}
