using NUnit.Framework;
using Content;
using FluentAssertions;
using Networking;
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
			SendMessageData SampleData = Utils.GetSendMessageData2();
			ChatClient conch = new ChatClient();
			MessageData x = conch.SendToMessage(SampleData, MessageEvent.NewMessage);

			Assert.AreEqual(x.Message, SampleData.Message);
			Assert.AreEqual(x.Event, MessageEvent.NewMessage);
			Assert.AreEqual(x.Type, SampleData.Type);
			Assert.AreEqual(x.FileData, null);
			Assert.AreEqual(x.Starred, false);
			Assert.AreEqual(x.ReplyThreadId, SampleData.ReplyThreadId);
			Assert.AreEqual(x.ReceiverIds.Length, SampleData.ReceiverIds.Length);
        }
	}
}