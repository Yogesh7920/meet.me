using NUnit.Framework;
using Content;
using System.IO;
using System;

namespace Testing.Content
{
    public class FileServerTests
    {
        private ContentDatabase database;
        private FileServer fileServer;

        [SetUp]
        public void Setup()
        {
            database = new ContentDatabase();
            fileServer = new FileServer(database);
        }

        [Test]
        public void Receive_StoringFile_ShouldBeAbleToStoreFile()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            MessageData file1 = new MessageData();
            file1.Message = "Test_File.pdf";
            file1.Type = MessageType.File;
            file1.FileData = new SendFileData(pathA);
            file1.SenderId = 1;
            file1.ReplyThreadId = -1;
            file1.Event = MessageEvent.NewMessage;

            MessageData recv = fileServer.Receive(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);
        }

        [Test]
        public void Receive_FetchingFile_ShouldBeAbleToFetchAStoredFile()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            MessageData file1 = new MessageData();
            file1.Message = "Test_File.pdf";
            file1.Type = MessageType.File;
            file1.FileData = new SendFileData(pathA);
            file1.SenderId = 1;
            file1.ReplyThreadId = -1;
            file1.Event = MessageEvent.NewMessage;

            MessageData recv = fileServer.Receive(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            MessageData file = new MessageData();
            file.MessageId = recv.MessageId;
            file.Type = MessageType.File;
            file.Event = MessageEvent.Download;

            recv = fileServer.Receive(file);

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
        public void Receive_GivingInvalidEventForFileType_NullShouldBeReturned()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            MessageData file1 = new MessageData();
            file1.Message = "Test_File.pdf";
            file1.Type = MessageType.File;
            file1.FileData = new SendFileData(pathA);
            file1.SenderId = 1;
            file1.ReplyThreadId = -1;
            file1.Event = MessageEvent.NewMessage;

            MessageData recv = fileServer.Receive(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            MessageData file = new MessageData();
            file.MessageId = 0;
            file.Type = MessageType.File;
            file.Event = MessageEvent.Star;

            recv = fileServer.Receive(file);
            Assert.IsNull(recv);

            file = new MessageData();
            file.MessageId = 0;
            file.Type = MessageType.File;
            file.Event = MessageEvent.Update;

            recv = fileServer.Receive(file);
            Assert.IsNull(recv);
        }

        [Test]
        public void Receive_FetchingAFilesThatDoesNotExist_NullShouldBeReturned()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            MessageData file1 = new MessageData();
            file1.Message = "Test_File.pdf";
            file1.Type = MessageType.File;
            file1.FileData = new SendFileData(pathA);
            file1.SenderId = 1;
            file1.ReplyThreadId = -1;
            file1.Event = MessageEvent.NewMessage;

            MessageData recv = fileServer.Receive(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            MessageData file = new MessageData();
            file.MessageId = 10;
            file.Type = MessageType.File;
            file.Event = MessageEvent.Download;

            recv = fileServer.Receive(file);
            Assert.IsNull(recv);
        }

        [Test]
        public void Receive_StoringAndFetchingMultipleFiles_ShouldBeAbleToStoreFilesAndFetchThem()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            string pathB = path[0] + "\\Testing\\Content\\Utils.cs";

            MessageData file1 = new MessageData();
            file1.Message = "Test_File.pdf";
            file1.Type = MessageType.File;
            file1.FileData = new SendFileData(pathA);
            file1.SenderId = 1;
            file1.ReplyThreadId = -1;
            file1.Event = MessageEvent.NewMessage;

            MessageData recv = fileServer.Receive(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            MessageData file2 = new MessageData();
            file2.Message = "Utils.cs";
            file2.Type = MessageType.File;
            file2.FileData = new SendFileData(pathB);
            file2.SenderId = 1;
            file2.ReplyThreadId = -1;
            file2.Event = MessageEvent.NewMessage;

            recv = fileServer.Receive(file2);

            Assert.AreEqual(file2.Message, recv.Message);
            Assert.AreNotEqual(recv.MessageId, file1.MessageId);
            Assert.AreNotEqual(recv.ReplyThreadId, file1.ReplyThreadId);
            Assert.AreEqual(file2.Type, recv.Type);
            Assert.AreEqual(file2.SenderId, recv.SenderId);
            Assert.AreEqual(file2.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            MessageData file3 = new MessageData();
            file3.Message = "c.txt";
            file3.Type = MessageType.File;
            file3.FileData = new SendFileData(pathB);
            file3.SenderId = 1;
            file3.ReplyThreadId = file1.ReplyThreadId;
            file3.Event = MessageEvent.NewMessage;

            recv = fileServer.Receive(file3);

            Assert.AreEqual(file3.Message, recv.Message);
            Assert.AreNotEqual(recv.MessageId, file1.MessageId);
            Assert.AreNotEqual(recv.MessageId, file2.MessageId);
            Assert.AreEqual(recv.ReplyThreadId, file1.ReplyThreadId);
            Assert.AreEqual(file3.Type, recv.Type);
            Assert.AreEqual(file3.SenderId, recv.SenderId);
            Assert.AreEqual(file3.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            file1.Event = MessageEvent.Download;
            recv = fileServer.Receive(file1);

            Assert.AreEqual(file1.Message, recv.Message);
            Assert.AreEqual(file1.MessageId, recv.MessageId);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderId, recv.SenderId);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file1.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file1.FileData.fileContent, recv.FileData.fileContent);
            Assert.AreEqual(file1.ReplyThreadId, recv.ReplyThreadId);

            file2.Event = MessageEvent.Download;
            recv = fileServer.Receive(file2);

            Assert.AreEqual(file2.Message, recv.Message);
            Assert.AreEqual(file2.MessageId, recv.MessageId);
            Assert.AreEqual(file2.Type, recv.Type);
            Assert.AreEqual(file2.SenderId, recv.SenderId);
            Assert.AreEqual(file2.Event, recv.Event);
            Assert.AreEqual(file2.FileData.fileSize, recv.FileData.fileSize);
            Assert.AreEqual(file2.FileData.fileName, recv.FileData.fileName);
            Assert.AreEqual(file2.FileData.fileContent, recv.FileData.fileContent);
            Assert.AreEqual(file2.ReplyThreadId, recv.ReplyThreadId);

            file3.Event = MessageEvent.Download;
            recv = fileServer.Receive(file3);

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
    }
}