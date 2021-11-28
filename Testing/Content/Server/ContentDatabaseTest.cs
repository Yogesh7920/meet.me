/// <author>Sameer Dhiman</author>
/// <created>7/11/2021</created>
/// <summary>
///     This file contains tests for ChatDatabase
/// </summary>

using System;
using System.IO;
using Content;
using NUnit.Framework;

namespace Testing.Content
{
    public class ContentDatabaseTests
    {
        private Utils _utils;
        private ContentDatabase contentDatabase;

        [SetUp]
        public void Setup()
        {
            contentDatabase = new ContentDatabase();
            _utils = new Utils();
        }

        [Test]
        public void StoreFile_StoringAFile_ShouldBeAbleToStoreFile()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var file1 = new MessageData
            {
                Message = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage
            };

            var recv = contentDatabase.StoreFile(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);
        }

        [Test]
        public void GetFiles_StoringAndFetchingAFileFromContentDatabase_ShouldBeAbleToFetchStoredFile()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var file1 = new MessageData
            {
                Message = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage
            };

            var recv = contentDatabase.StoreFile(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);

            recv = contentDatabase.GetFiles(file1.MessageId);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.MessageId, recv.MessageId);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);
            Assert.AreEqual(file1.ReplyThreadId, recv.ReplyThreadId);
        }

        [Test]
        public void GetFiles_TryingToFetchAFileThatDoesNotExist_NullShouldBeReturned()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var file1 = new MessageData
            {
                Message = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage
            };

            var recv = contentDatabase.StoreFile(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);

            recv = contentDatabase.GetFiles(10);

            Assert.IsNull(recv);
        }

        [Test]
        public void StoreFile_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var pathB = path[0] + "\\Testing\\Content\\Utils.cs";

            var file1 = new MessageData
            {
                Message = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage
            };

            var recv = contentDatabase.StoreFile(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);

            var file2 = new MessageData
            {
                Message = "Utils.cs",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage
            };

            recv = contentDatabase.StoreFile(file2);

            Assert.AreEqual(file2.Message, recv.Message);
            Assert.AreNotEqual(recv.MessageId, file1.MessageId);
            Assert.AreNotEqual(recv.ReplyThreadId, file1.ReplyThreadId);
            Assert.AreEqual(file2.Type, recv.Type);
            Assert.AreEqual(file2.SenderId, recv.SenderId);
            Assert.AreEqual(file2.Event, recv.Event);
            Assert.AreEqual(file2.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file2.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file2.FileData.fileContent, recv.FileData.fileContent);

            var file3 = new MessageData
            {
                Message = "c.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderId = 1,
                ReplyThreadId = file1.ReplyThreadId,
                Event = MessageEvent.NewMessage
            };

            recv = contentDatabase.StoreFile(file3);

            Assert.AreEqual(file3.Message, recv.Message);
            Assert.AreNotEqual(recv.MessageId, file1.MessageId);
            Assert.AreNotEqual(recv.MessageId, file2.MessageId);
            Assert.AreEqual(recv.ReplyThreadId, file1.ReplyThreadId);
            Assert.AreEqual(file3.Type, recv.Type);
            Assert.AreEqual(file3.SenderId, recv.SenderId);
            Assert.AreEqual(file3.Event, recv.Event);
            Assert.AreEqual(file3.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file3.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file3.FileData.fileContent, recv.FileData.fileContent);

            recv = contentDatabase.GetFiles(file1.MessageId);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.MessageId, recv.MessageId);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);
            Assert.AreEqual(file1.ReplyThreadId, recv.ReplyThreadId);

            recv = contentDatabase.GetFiles(file2.MessageId);

            Assert.AreEqual(file2.Message, recv.Message);
            Assert.AreEqual(file2.MessageId, recv.MessageId);
            Assert.AreEqual(file2.Type, recv.Type);
            Assert.AreEqual(file2.SenderId, recv.SenderId);
            Assert.AreEqual(file2.Event, recv.Event);
            Assert.AreEqual(file2.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file2.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file2.FileData.fileContent, recv.FileData.fileContent);
            Assert.AreEqual(file2.ReplyThreadId, recv.ReplyThreadId);

            recv = contentDatabase.GetFiles(file3.MessageId);

            Assert.AreEqual(file3.Message, recv.Message);
            Assert.AreEqual(file3.MessageId, recv.MessageId);
            Assert.AreEqual(file3.Type, recv.Type);
            Assert.AreEqual(file3.SenderId, recv.SenderId);
            Assert.AreEqual(file3.Event, recv.Event);
            Assert.AreEqual(file3.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file3.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file3.FileData.fileContent, recv.FileData.fileContent);
            Assert.AreEqual(file3.ReplyThreadId, recv.ReplyThreadId);
        }

        [Test]
        public void StoreMessage_StoringASingleMessage_MessageShouldBeStored()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            var recv = contentDatabase.StoreMessage(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsNull(recv.FileData);
        }

        [Test]
        public void GetMessage_StoringAndFetchingAMessage_ShouldBeAbleToFetchStoredMessage()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            var recv = contentDatabase.StoreMessage(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var msg = contentDatabase.GetMessage(message1.ReplyThreadId, message1.MessageId);

            Assert.AreEqual(message1.Message, msg.Message);
            Assert.AreEqual(msg.MessageId, message1.MessageId);
            Assert.AreEqual(message1.Type, msg.Type);
            Assert.AreEqual(message1.SenderId, msg.SenderId);
            Assert.AreEqual(message1.Event, msg.Event);
            Assert.AreEqual(message1.ReplyThreadId, msg.ReplyThreadId);
        }

        [Test]
        public void GetMessage_FetchingAnInvalidMessage_NullShouldBeReturned()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            var message2 = _utils.GenerateNewMessageData("Hello2", SenderId: 1);

            var recv = contentDatabase.StoreMessage(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            recv = contentDatabase.StoreMessage(message2);

            Assert.AreEqual(message2.Message, recv.Message);
            Assert.AreEqual(message2.Type, recv.Type);
            Assert.AreEqual(message2.SenderId, recv.SenderId);
            Assert.AreEqual(message2.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var message3 = _utils.GenerateNewMessageData("Hello3", SenderId: 1, ReplyThreadId: message1.ReplyThreadId);

            recv = contentDatabase.StoreMessage(message3);

            Assert.AreEqual(message3.Message, recv.Message);
            Assert.AreEqual(message3.Type, recv.Type);
            Assert.AreEqual(message3.SenderId, recv.SenderId);
            Assert.AreEqual(message3.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var msg = contentDatabase.GetMessage(10, message1.MessageId);
            Assert.IsNull(msg);

            msg = contentDatabase.GetMessage(message1.ReplyThreadId, 10);
            Assert.IsNull(msg);

            msg = contentDatabase.GetMessage(message2.ReplyThreadId, 2);
            Assert.IsNull(msg);
        }

        [Test]
        public void StoreMessage_StoringMultipleMessages_ShouldBeAbleToStoreAndFetchMultipleMessages()
        {
            var message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            var recv = contentDatabase.StoreMessage(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var message2 = _utils.GenerateNewMessageData("Hello2", SenderId: 1, ReplyThreadId: message1.ReplyThreadId);

            recv = contentDatabase.StoreMessage(message2);

            Assert.AreEqual(message2.Message, recv.Message);
            Assert.AreNotEqual(message2.MessageId, message1.MessageId);
            Assert.AreEqual(message2.Type, recv.Type);
            Assert.AreEqual(message2.SenderId, recv.SenderId);
            Assert.AreEqual(message2.Event, recv.Event);
            Assert.IsNull(recv.FileData);
            Assert.AreEqual(message2.ReplyThreadId, recv.ReplyThreadId);

            var message3 = _utils.GenerateNewMessageData("Hello3", SenderId: 1);

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

            var msg = contentDatabase.GetMessage(message1.ReplyThreadId, message1.MessageId);

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
        public void
            GetChatContexts_StoringMultipleFilesAndMessages_ShouldBeAbleToFetchAllChatContextsAndMessagesAreInCorrectChatContexts()
        {
            contentDatabase = new ContentDatabase();

            StoreFile_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles();
            StoreMessage_StoringMultipleMessages_ShouldBeAbleToStoreAndFetchMultipleMessages();

            var msgList = contentDatabase.GetChatContexts();

            foreach (var m in msgList)
            foreach (var n in m.MsgList)
                Assert.AreEqual(n, contentDatabase.GetMessage(m.ThreadId, n.MessageId));
        }
    }
}