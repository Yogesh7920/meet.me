/// <author>Sairoop Bodepudi</author>
/// <created>19/11/2021</created>
/// <summary>
///		This is the unit testing utilities 
///		file which would be used while testing
///		the summarizer functions.
/// </summary>

using Content;
using System;
using System.Collections.Generic;

namespace Testing.Dashboard.Summary
{
	/// <summary>
	/// Helper class for the Summary Logic module testing
	/// </summary>
	class Utils
	{
		/// <summary>
		/// Based on the query the appropriate chat context is returned
		/// </summary>
		/// <param name="query"> The query for the chat context
		/// </param>
		/// <returns> The appropriate chat context array
		/// </returns>
		public static ChatContext[] GetChatContext(string query)
		{
			if (query == "Null Context")
				return null;
			else if (query == "Empty chat context")
				return Array.Empty<ChatContext>();
			else if (query == "Empty chats")
			{
				List<ChatContext> chats = new();
				for (int i = 0; i < 50; i++)
				{
					ReceiveMessageData data = new();
					// All null messages but users are initialized.
					data.Message = "";
					data.Type = MessageType.Chat;
					data.Starred = false;
					ChatContext c = new();
					List<ReceiveMessageData> receiveMessageDatas = new();
					receiveMessageDatas.Add(data);
					c.MsgList = receiveMessageDatas;
					chats.Add(c);
				}
				return chats.ToArray();
			}
			else if (query == "Fixed chat")
			{
				List<ChatContext> chats = new();
				for (int i = 0; i < 50; i++)
				{
					ChatContext c = new();
					List<ReceiveMessageData> receiveMessageDatas = new();
					for (int j = 0; j < 5; j++)
					{
						ReceiveMessageData data = new();
						// A constant message CONST is sent by all the users.
						data.Message = "CONST";
						data.Type = MessageType.Chat;
						data.Starred = false;
						receiveMessageDatas.Add(data);
					}
					c.MsgList = receiveMessageDatas;
					chats.Add(c);
				}
				return chats.ToArray();
			}
			else if (query == "Variable chat")
			{
				List<ChatContext> chats = new();
				ChatContext c = new();
				// Just to check the working of the Porter Stemmer to obtain the lemmas.
				List<ReceiveMessageData> receiveMessageDatas = new();
				ReceiveMessageData step1 = new();
				step1.Message = "caresses. plastered. troubled. happy";
				step1.Type = MessageType.Chat;
				step1.Starred = false;
				receiveMessageDatas.Add(step1);
				ReceiveMessageData step2 = new();
				step2.Message = "relational. hesitanci. vietnamization";
				step2.Type = MessageType.Chat;
				step2.Starred = false;
				receiveMessageDatas.Add(step2);
				ReceiveMessageData step3 = new();
				step3.Message = "triplicate. formalize. electrical";
				step3.Type = MessageType.Chat;
				step3.Starred = true;
				receiveMessageDatas.Add(step3);
				ReceiveMessageData step4 = new();
				step4.Message = "allowance. defensible. homologous";
				step4.Type = MessageType.Chat;
				step4.Starred = false;
				receiveMessageDatas.Add(step4);
				ReceiveMessageData step5 = new();
				step5.Message = "probate. controll. roll";
				step5.Type = MessageType.Chat;
				step5.Starred = false;
				receiveMessageDatas.Add(step5);
				c.MsgList = receiveMessageDatas;
				chats.Add(c);
				return chats.ToArray();
			}
			else
			{
				List<ChatContext> chats = new();
				for (int i = 0; i < 50; i++)
				{
					// General chat context resembling real application cases.
					ChatContext c = new();
					List<ReceiveMessageData> receiveMessageDatas = new();
					for (int j = 0; j < 5; j++)
					{
						ReceiveMessageData data = new();
						data.Message = "Hi from " + (i + j).ToString();
						if (i % 5 == 0)
							data.Message += ".This is special";
						data.Type = MessageType.Chat;
						data.Starred = (i % 5) == 0;
						receiveMessageDatas.Add(data);
					}
					c.MsgList = receiveMessageDatas;
					chats.Add(c);
				}
				return chats.ToArray();
			}
		}
	}
}
