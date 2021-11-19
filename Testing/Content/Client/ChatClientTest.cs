using NUnit.Framework;
using Content;
using FluentAssertions;
namespace Testing.Content
{
    public class ChatClientTests
    {
		
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestConvert()
        {
			var toconvert = new SendMessageData();
			toconvert.Message = "Apple";
			toconvert.Type = MessageType.Chat;
			toconvert.ReplyThreadId = -1;
			toconvert.ReceiverIds = new int[0];
			ChatClient conch = new ChatClient();
            MessageData x = conch.SendToMessage(toconvert,MessageEvent.NewMessage);

			Assert.AreEqual(x.Message, "Apple");
			Assert.AreEqual(x.Event, MessageEvent.NewMessage);
			Assert.AreEqual(x.Type, MessageType.Chat);
			Assert.AreEqual(x.FileData, null);
			Assert.AreEqual(x.Starred, false);
			Assert.AreEqual(x.ReplyThreadId, -1);
			Assert.AreEqual(x.ReceiverIds.Length, 0);


        }
    }
}