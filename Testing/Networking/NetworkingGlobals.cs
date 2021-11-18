using System;
using System.Dynamic;
using System.Threading;
using Networking;

namespace Testing.Networking
{
    /// <summary>
    ///     This class represents an abstract machine.
    ///     clients and server inherit from this machine.
    /// </summary>
    public class Machine
    {
        public readonly FakeNotificationHandler SsHandler;

        public readonly FakeNotificationHandler WbHandler;
        public readonly FakeNotificationHandler SsHandler;

        protected Machine()
        {
            WbHandler = new FakeNotificationHandler();
            SsHandler = new FakeNotificationHandler();
        }

        public ICommunicator Communicator { get; set; }

        public void Subscribe()
        {
            Communicator.Subscribe(Modules.WhiteBoard, WbHandler, Priorities.WhiteBoard);
            Communicator.Subscribe(Modules.ScreenShare, SsHandler, Priorities.ScreenShare);
        }

        public void Reset()
        {
            WbHandler.Reset();
            SsHandler.Reset();
        }
    }

    public class FakeClientA : Machine
    {
        public string Id = "A";

        public FakeClientA()
        {
            Communicator = NetworkingGlobals.NewClientCommunicator;
        }
    }

    public class FakeClientB : Machine
    {
        public string Id = "B";

        public FakeClientB()
        {
            Communicator = NetworkingGlobals.NewClientCommunicator;
        }
    }

    public class FakeServer : Machine
    {
        public FakeServer()
        {
            Communicator = NetworkingGlobals.NewServerCommunicator;
        }
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

    /// <summary>
    ///     A fake notification handler to receive notifications, mimicking
    ///     other modules.
    /// </summary>
    public class FakeNotificationHandler : INotificationHandler
    {
        private readonly AutoResetEvent _autoResetEvent = new(false);
        private int _timeOutCount;
        public dynamic Data = new ExpandoObject();
        public dynamic Event = new ExpandoObject();
        public dynamic Data = new ExpandoObject();
        private int _timeOutCount;
        private readonly AutoResetEvent _autoResetEvent = new(false);

        /// <summary>
        /// Calling this function will block the thread until a message from the network
        /// is received or it has timed out.
        /// </summary>
        /// <param name="timeOut">
        /// Double value indicating the number of seconds to wait before timing out.
        /// </param>
        /// <exception cref="TimeoutException"></exception>
        public void Wait(double timeOut = 15)
        {
            // wait for a maximum of timeOut seconds
            bool signalReceived = _autoResetEvent.WaitOne(TimeSpan.FromSeconds(timeOut));
            if (!signalReceived)
            {
                /*
                 * If the wait has timed out, increase the number of timeouts
                 * this allows us to ignore messages that were previously timed out.
                 */
                _timeOutCount++;
                _autoResetEvent.Reset();
                throw new TimeoutException("Wait failed due to timeout!");
            }
        }

        public void OnDataReceived(string data)
        {
            Event = NotificationEvents.OnDataReceived;
            Data = data;
            /*
             * Ignore this message since this test already failed due to timout
             * and the current test needs the next data.
             */
            if (_timeOutCount-- > 0) return;
            _autoResetEvent.Set();
        }

        public void OnClientJoined<T>(T socketObject)
        {
            Event = NotificationEvents.OnClientJoined;
            Data = socketObject;
            if (_timeOutCount-- > 0) return;
            _autoResetEvent.Set();
        }

        public void OnClientLeft(string clientId)
        {
            Event = NotificationEvents.OnClientLeft;
            Data = clientId;
            if (_timeOutCount-- > 0) return;
            _autoResetEvent.Set();
        }

        /// <summary>
        ///     Calling this function will block the thread until a message from the network
        ///     is received or it has timed out.
        /// </summary>
        /// <param name="timeOut">
        ///     Double value indicating the number of seconds to wait before timing out.
        /// </param>
        /// <exception cref="TimeoutException"></exception>
        public void Wait(double timeOut = 15)
        {
            // wait for a maximum of timeOut seconds
            var signalReceived = _autoResetEvent.WaitOne(TimeSpan.FromSeconds(timeOut));
            if (!signalReceived)
            {
                /*
                 * If the wait has timed out, increase the number of timeouts
                 * this allows us to ignore messages that were previously timed out.
                 */
                _timeOutCount++;
                _autoResetEvent.Reset();
                throw new TimeoutException("Wait failed due to timeout!");
            }
        }

        public void Reset()
        {
            Event = null;
            Data = null;
            _autoResetEvent.Reset();
        }
    }

    public class FakeChat
    {
        public string Message { get; set; }
        public string Timestamp { get; set; }

        public static FakeChat GetFakeChat()
        {
            var fakeChat = new FakeChat();
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

            for (var i = 0; i < stringChars.Length; i++) stringChars[i] = chars[random.Next(chars.Length)];

            return new string(stringChars);
        }
    }
}