using NUnit.Framework;
using Content;
using Networking;

namespace Testing.Content
{
	///<summary>
	/// This file will contain required sample datastructures to test files and modules
	/// </summary>
	public class Utils
    {
		public static SendMessageData GetSendMessageData1()
		{
			var toconvert1 = new SendMessageData();
			toconvert1.Message = "Apple";
			toconvert1.Type = MessageType.Chat;
			toconvert1.ReplyThreadId = -1;
			toconvert1.ReceiverIds = new int[0];
			return toconvert1;
		}

		public static MessageData GetMessageData1()
        {
			SendMessageData SampleData = GetSendMessageData1();
			ChatClient conch = new ChatClient();
			MessageData MsgData = conch.SendToMessage(SampleData, MessageEvent.NewMessage);
			return MsgData;
		}

		public static string GetSerializedMsg1()
        {
			ISerializer _serializer = new Serializer();
			var SerializedMsg = _serializer.Serialize(GetMessageData1());
			return SerializedMsg;
		}

		public static SendMessageData GetSendMessageData2()
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
