using NUnit.Framework;
using Content;

/*
namespace Content
{
    public class SendMessageData
    {
        /// <summary>
        ///     Message string if MessageType is Chat else file path if MessageType is File
        /// </summary>
        public string Message;

        /// <summary>
        ///     List of ids for receivers, empty in case of broadcast
        /// </summary>
        public int[] ReceiverIds;

        /// <summary>
        ///     Id of the thread if message belongs to an already existing thread else -1
        /// </summary>
        public int ReplyThreadId;

        /// <summary>
        ///     Message Type - File or Chat
        /// </summary>
        public MessageType Type;
    }
} 

using System;
using System.Collections.Generic;

namespace Content
{
    public class ChatContext
    {
        /// <summary>
        ///     Time of creation of thread
        /// </summary>
        public DateTime CreationTime;

        /// <summary>
        ///     List of all the messages in the thread
        /// </summary>
        public List<ReceiveMessageData> MsgList;

        /// <summary>
        ///     Number of messages in the thread
        /// </summary>
        public int NumOfMessages;

        /// <summary>
        ///     Id of the thread
        /// </summary>
        public int ThreadId;
    }
}

using System;

namespace Content
{
    [Serializable]
    public class ReceiveMessageData
    {
        /// <summary>
        ///     Message Event - Update, NewMessage, Star, Download
        /// </summary>
        public MessageEvent Event;

        /// <summary>
        ///     Message string if messageType is Chat else file name for File messageType
        /// </summary>
        public string Message;

        /// <summary>
        ///     Id of the message
        /// </summary>
        public int MessageId;

        /// <summary>
        ///     List of ids for receviers, all if empty
        /// </summary>
        public int[] ReceiverIds;

        /// <summary>
        ///     Id of thread to which this message belongs
        /// </summary>
        public int ReplyThreadId;

        /// <summary>
        ///     User id of the message sender
        /// </summary>
        public int SenderId;

        /// <summary>
        ///     Time at which message was sent
        /// </summary>
        public DateTime SentTime;

        /// <summary>
        ///     True if this message is starred else False
        /// </summary>
        public bool Starred;

        /// <summary>
        ///     Message Type - File or Chat
        /// </summary>
        public MessageType Type;
    }
}
*/
namespace Testing
{
    [TestFixture]
    public class ContentModuleTesting
    {
        [Setup]
        public void SetUp()
        {
            // User ID for various test users
            int _userID1 = 1001;
            int _userID2 = 1002;
            int _userID3 = 1003;
        }
        // Server for Content module
        private readonly ContentClient _contentServer = ContentServerFactory.GetInstance();
        // Test users at client side, three users to test private and broadcast send and receive for files and chats
        private readonly ContentClient _contentClientUser1 = new ContentClient();
        private readonly ContentClient _contentClientUser2 = new ContentClient();
        private readonly ContentClient _contentClientUser3 = new ContentClient();

        [Test]
        [TestCase(1001)]
        [TestCase(1002)]
        public void SetUser_ContentClientCreation_userIDOfContentClientEqualsToAssignedValue(int userId)
        {
            IContentClient _contentClient = new ContentClientFactory.getInstance();
            _contentClient.setUser(userId);
            int userIdOfClass = _contentClient.UserId;
            Assert.AreEqual(userIdOfClass, userId);
        }
    }
}
