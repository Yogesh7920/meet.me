using NUnit.Framework;
using Content;
using FluentAssertions;
namespace Testing.Content
{
    public class ChatClientTests
    {
		private ChatClient _conch;
        [SetUp]
        public void Setup()
        {
			_conch = new ChatClient();
        }

        [Test]
        public void TestConvert()
        {
			var toconvert = new SendMessageData();
			toconvert.Message = "Apple";
			toconvert.Type = MessageType.Chat;
			toconvert.ReplyThreadId = -1;
			toconvert.ReceiverIds = new int[0];
            MessageData x = _conch.SendToMessage(toconvert,MessageEvent.NewMessage);

			Assert.AreEqual(x.Message, "Apple");
			Assert.AreEqual(x.Event, MessageEvent.NewMessage);
			Assert.AreEqual(x.Type, MessageType.Chat);
			Assert.AreEqual(x.FileData, null);
			Assert.That(typeof(x.SenderId).ToString() == "int");
			Assert.AreEqual(x.Starred, false);
			Assert.AreEqual(x.ReplyThreadId, -1);
			Assert.AreEqual(x.ReceiverIds.Length, 0);


        }
    }
}