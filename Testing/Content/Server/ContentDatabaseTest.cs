using NUnit.Framework;
using Content;
using System;
using System.Collections.Generic;

namespace Testing.Content
{
    public class ContentDatabaseTests
    {
        private ContentDatabase contentDatabase;

        [SetUp]
        public void Setup()
        {
            contentDatabase = new ContentDatabase();
        }

        [Test]
        public void FileTest()
        {
            //string pathA = @"trace.txt";
            //string pathB = @"whiteboard.dll";

            //MessageData file1 = new MessageData();
            //file1.Message = "trace.txt";
            //file1.Type = MessageType.File;
            //file1.FileData = new SendFileData(pathA);
            //file1.SenderId = 1;
            //file1.ReplyThreadId = -1;
            //file1.Event = MessageEvent.NewMessage;

            //MessageData recv = contentDatabase.StoreFile(file1);

            //Assert.AreEqual(file1.Message, recv.Message);
            //Assert.AreEqual(file1.Type, recv.Type);
            //Assert.AreEqual(file1.SenderId, recv.SenderId);
            //Assert.AreEqual(file1.Event, recv.Event);
            //Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            //Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            //Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);

            //MessageData file2 = new MessageData();
            //file2.Message = "whiteboard.dll";
            //file2.Type = MessageType.File;
            //file2.FileData = new SendFileData(pathB);
            //file2.SenderId = 1;
            //file2.ReplyThreadId = -1;
            //file2.Event = MessageEvent.NewMessage;

            //recv = contentDatabase.StoreFile(file2);

            //Assert.AreEqual(file2.Message, recv.Message);
            //Assert.AreNotEqual(recv.MessageId, file1.MessageId);
            //Assert.AreNotEqual(recv.ReplyThreadId, file1.ReplyThreadId);
            //Assert.AreEqual(file2.Type, recv.Type);
            //Assert.AreEqual(file2.SenderId, recv.SenderId);
            //Assert.AreEqual(file2.Event, recv.Event);
            //Assert.AreEqual(file2.FileData.fileSize, recv.FileData.fileSize);
            //Assert.AreEqual(file2.FileData.fileName, recv.FileData.fileName);
            //Assert.AreEqual(file2.FileData.fileContent, recv.FileData.fileContent);

            //MessageData file3 = new MessageData();
            //file3.Message = "c.txt";
            //file3.Type = MessageType.File;
            //file3.FileData = new SendFileData(pathB);
            //file3.SenderId = 1;
            //file3.ReplyThreadId = file1.ReplyThreadId;
            //file3.Event = MessageEvent.NewMessage;

            //recv = contentDatabase.StoreFile(file3);

            //Assert.AreEqual(file3.Message, recv.Message);
            //Assert.AreNotEqual(recv.MessageId, file1.MessageId);
            //Assert.AreNotEqual(recv.MessageId, file2.MessageId);
            //Assert.AreEqual(recv.ReplyThreadId, file1.ReplyThreadId);
            //Assert.AreEqual(file3.Type, recv.Type);
            //Assert.AreEqual(file3.SenderId, recv.SenderId);
            //Assert.AreEqual(file3.Event, recv.Event);
            //Assert.AreEqual(file3.FileData.fileSize, recv.FileData.fileSize);
            //Assert.AreEqual(file3.FileData.fileName, recv.FileData.fileName);
            //Assert.AreEqual(file3.FileData.fileContent, recv.FileData.fileContent);

            //recv = contentDatabase.GetFiles(file1.MessageId);

            //Assert.AreEqual(file1.Message, recv.Message);
            //Assert.AreEqual(file1.MessageId, recv.MessageId);
            //Assert.AreEqual(file1.Type, recv.Type);
            //Assert.AreEqual(file1.SenderId, recv.SenderId);
            //Assert.AreEqual(file1.Event, recv.Event);
            //Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            //Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            //Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);
            //Assert.AreEqual(file1.ReplyThreadId, recv.ReplyThreadId);

            //recv = contentDatabase.GetFiles(file2.MessageId);

            //Assert.AreEqual(file2.Message, recv.Message);
            //Assert.AreEqual(file2.MessageId, recv.MessageId);
            //Assert.AreEqual(file2.Type, recv.Type);
            //Assert.AreEqual(file2.SenderId, recv.SenderId);
            //Assert.AreEqual(file2.Event, recv.Event);
            //Assert.AreEqual(file2.FileData.fileSize, recv.FileData.fileSize);
            //Assert.AreEqual(file2.FileData.fileName, recv.FileData.fileName);
            //Assert.AreEqual(file2.FileData.fileContent, recv.FileData.fileContent);
            //Assert.AreEqual(file2.ReplyThreadId, recv.ReplyThreadId);

            //recv = contentDatabase.GetFiles(file3.MessageId);

            //Assert.AreEqual(file3.Message, recv.Message);
            //Assert.AreEqual(file3.MessageId, recv.MessageId);
            //Assert.AreEqual(file3.Type, recv.Type);
            //Assert.AreEqual(file3.SenderId, recv.SenderId);
            //Assert.AreEqual(file3.Event, recv.Event);
            //Assert.AreEqual(file3.FileData.fileSize, recv.FileData.fileSize);
            //Assert.AreEqual(file3.FileData.fileName, recv.FileData.fileName);
            //Assert.AreEqual(file3.FileData.fileContent, recv.FileData.fileContent);
            //Assert.AreEqual(file3.ReplyThreadId, recv.ReplyThreadId);
        }

        [Test]
        public void MessageTest()
        {
            MessageData message1 = new MessageData();
            message1.Message = "Hello";
            message1.Type = MessageType.Chat;
            message1.SenderId = 1;
            message1.ReplyThreadId = 0;
            message1.Event = MessageEvent.NewMessage;

            MessageData recv = contentDatabase.StoreMessage(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            MessageData message2 = new MessageData();
            message2.Message = "Hello2";
            message2.Type = MessageType.Chat;
            message2.SenderId = 1;
            message2.ReplyThreadId = message1.ReplyThreadId;
            message2.Event = MessageEvent.NewMessage;

            recv = contentDatabase.StoreMessage(message2);

            Assert.AreEqual(message2.Message, recv.Message);
            Assert.AreNotEqual(message2.MessageId, message1.MessageId);
            Assert.AreEqual(message2.Type, recv.Type);
            Assert.AreEqual(message2.SenderId, recv.SenderId);
            Assert.AreEqual(message2.Event, recv.Event);
            Assert.IsNull(recv.FileData);
            Assert.AreEqual(message2.ReplyThreadId, recv.ReplyThreadId);

            MessageData message3 = new MessageData();
            message3.Message = "Hello3";
            message3.Type = MessageType.Chat;
            message3.SenderId = 1;
            message3.ReplyThreadId = -1;
            message3.Event = MessageEvent.NewMessage;

            recv = contentDatabase.StoreMessage(message3);

            Assert.AreEqual(message3.Message, recv.Message);
            Assert.AreNotEqual(message3.MessageId, message2.MessageId);
            Assert.AreNotEqual(message3.MessageId, message1.MessageId);
            Assert.AreEqual(message3.Type, recv.Type);
            Assert.AreEqual(message3.SenderId, recv.SenderId);
            Assert.AreEqual(message3.Event, recv.Event);
            Assert.IsNull(recv.FileData);
            Assert.AreNotEqual(message2.ReplyThreadId, recv.ReplyThreadId);
            Assert.AreNotEqual(message1.ReplyThreadId, recv.ReplyThreadId);

            ReceiveMessageData msg = contentDatabase.GetMessage(message1.ReplyThreadId, message1.MessageId);

            Assert.AreEqual(message1.Message, msg.Message);
            Assert.AreEqual(msg.MessageId, message1.MessageId);
            Assert.AreEqual(message1.Type, msg.Type);
            Assert.AreEqual(message1.SenderId, msg.SenderId);
            Assert.AreEqual(message1.Event, msg.Event);
            Assert.AreEqual(message1.ReplyThreadId, msg.ReplyThreadId);

            msg = contentDatabase.GetMessage(message2.ReplyThreadId, message2.MessageId);

            Assert.AreEqual(message2.Message, msg.Message);
            Assert.AreEqual(message2.MessageId, msg.MessageId);
            Assert.AreEqual(message2.Type, msg.Type);
            Assert.AreEqual(message2.SenderId, msg.SenderId);
            Assert.AreEqual(message2.Event, msg.Event);
            Assert.AreEqual(message2.ReplyThreadId, msg.ReplyThreadId);

            msg = contentDatabase.GetMessage(message3.ReplyThreadId, message3.MessageId);

            Assert.AreEqual(message3.Message, msg.Message);
            Assert.AreEqual(message3.MessageId, msg.MessageId);
            Assert.AreEqual(message3.Type, msg.Type);
            Assert.AreEqual(message3.SenderId, msg.SenderId);
            Assert.AreEqual(message3.Event, msg.Event);
            Assert.AreEqual(message3.ReplyThreadId, msg.ReplyThreadId);
        }

        [Test]
        public void ChatContextTest()
        {
            contentDatabase = new ContentDatabase();

            FileTest();
            MessageTest();

            List<ChatContext> msgList = contentDatabase.GetChatContexts();

            foreach (var m in msgList)
            {
                foreach (var n in m.MsgList)
                {
                    Assert.AreEqual(n, contentDatabase.GetMessage(m.ThreadId, n.MessageId));
                }
            }
        }
    }
}