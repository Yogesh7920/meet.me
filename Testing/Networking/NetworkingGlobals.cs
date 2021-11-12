using System;
using System.Dynamic;
using System.Threading;
using Networking;

namespace Testing.Networking
{
    public static class FakeMachineB
    {
        public static string ClientID = "B";
        public static ICommunicator Communicator;
        public static FakeNotificationHandler WbHandler = new(), SsHandler = new();
    }

    public static class FakeServer
    {
        public static ICommunicator Communicator;
        public static FakeNotificationHandler WbHandler = new(), SsHandler = new();
    }

    public static class FakeMachineA
    {
        public static string ClientID = "A";
        public static ICommunicator Communicator;
        public static FakeNotificationHandler WbHandler = new(), SsHandler = new();
    }

    public static class Modules
    {
        public const string
            ScreenShare = "ScreenShare",
            WhiteBoard = "WhiteBoard",
            Chat = "Chat",
            File = "File",
            Networking = "Networking",
            Invalid = "Invalid";
    }

    public static class Priorities
    {
        public const int
            ScreenShare = 4,
            WhiteBoard = 3,
            Chat = 2,
            File = 1,
            Networking = 1,
            Invalid = -1;
    }

    public enum NotificationEvents
    {
        OnDataReceived,
        OnClientJoined,
        OnClientLeft
    }

    public class FakeNotificationHandler : INotificationHandler
    {
        public readonly dynamic ReceivedData = new ExpandoObject();

        public void Wait(int waitTime=10)
        {
            while (ReceivedData.Event is null) { Thread.Sleep(waitTime); }
        }

        public void OnDataReceived(string data)
        {
            ReceivedData.Event = NotificationEvents.OnDataReceived;
            ReceivedData.Data = data;
        }

        public void OnClientJoined<T>(T socketObject)
        {
            ReceivedData.Event = NotificationEvents.OnClientJoined;
            ReceivedData.Data = socketObject;
        }

        public void OnClientLeft(string clientId)
        {
            ReceivedData.Event = NotificationEvents.OnClientLeft;
            ReceivedData.Data = clientId;
        }

        public void Reset()
        {
            ReceivedData.Event = null;
            ReceivedData.Data = null;
        }
    }

    public class FakeChat
    {
        public string Message { get; set; }
        public string Timestamp { get; set; }

        public static FakeChat GetFakeChat()
        {
            FakeChat fakeChat = new FakeChat();
            fakeChat.Message = NetworkingGlobals.GetRandomString();
            fakeChat.Timestamp = DateTime.Now.ToString();
            return fakeChat;
        }
    }

    public static class NetworkingGlobals
    {
        public static ICommunicator NewClientCommunicator => CommunicationFactory.GetCommunicator(true, true);

        public static ICommunicator NewServerCommunicator => CommunicationFactory.GetCommunicator(false, true);

        public static string GetRandomString(int length = 10)
        {
            var chars = "ABCDEGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}