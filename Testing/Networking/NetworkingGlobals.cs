/// <author>Subhash S</author>
/// <created>12/11/2021</created>
/// <summary>
///     This file contains some mock objects which can
///     be used to simulate tests for the networking module.
/// </summary>

using System;
using System.Dynamic;
using System.Threading;
using Networking;

namespace Testing.Networking
{
    /// <summary>
    ///     This is a mock class used to represent a machine.
    /// </summary>
    public class Machine
    {
        // Each machine has two handlers, one for whiteboard and one for screen-share
        public readonly FakeNotificationHandler SsHandler;

        public readonly FakeNotificationHandler WbHandler;

        /// <summary>
        ///     Initialize handlers for both whiteboard and screen-share
        /// </summary>
        protected Machine()
        {
            WbHandler = new FakeNotificationHandler();
            SsHandler = new FakeNotificationHandler();
        }

        public ICommunicator Communicator { get; set; }

        /// <summary>
        ///     Subscribes both the modules on the machine to the networking module
        /// </summary>
        public void Subscribe()
        {
            Communicator.Subscribe(Modules.WhiteBoard, WbHandler, Priorities.WhiteBoard);
            Communicator.Subscribe(Modules.ScreenShare, SsHandler, Priorities.ScreenShare);
        }

        /// <summary>
        ///     Reset states of both handlers, this includes their local data which
        ///     is used to identify the type of event and the data
        /// </summary>
        public void Reset()
        {
            WbHandler.Reset();
            SsHandler.Reset();
        }
    }

    /// <summary>
    ///     A client with ID "A"
    /// </summary>
    public class FakeClientA : Machine
    {
        public const string Id = "A";

        public FakeClientA()
        {
            Communicator = NetworkingGlobals.NewClientCommunicator;
        }
    }

    /// <summary>
    ///     A client with ID "B"
    /// </summary>
    public class FakeClientB : Machine
    {
        public const string Id = "B";

        public FakeClientB()
        {
            Communicator = NetworkingGlobals.NewClientCommunicator;
        }
    }

    /// <summary>
    ///     A mock server
    /// </summary>
    public class FakeServer : Machine
    {
        public FakeServer()
        {
            Communicator = NetworkingGlobals.NewServerCommunicator;
        }
    }


    /// <summary>
    ///     The list of all modules that can be used for testing.
    /// </summary>
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

    /// <summary>
    ///     The priorities of all modules that are used for testing.
    /// </summary>
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

    /// <summary>
    ///     These are the events that the networking module uses to notify modules.
    /// </summary>
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

        public void OnDataReceived(string data)
        {
            Event = NotificationEvents.OnDataReceived;
            Data = data;

            // Ignore this message since this test already failed due to timout
            // and the current test needs the new data.
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
        /// <exception cref="TimeoutException">
        ///     Indicates that no events have been received for longer than the timeOut period.
        /// </exception>
        public void Wait(double timeOut = 15)
        {
            // wait for a maximum of timeOut seconds
            var signalReceived = _autoResetEvent.WaitOne(TimeSpan.FromSeconds(timeOut));
            if (signalReceived) return;
            // If the wait has timed out, increase the number of timeouts
            // this allows us to ignore messages that were previously timed out.
            _timeOutCount++;
            _autoResetEvent.Reset();
            throw new TimeoutException("Wait failed due to timeout!");
        }

        /// <summary>
        ///     Reset Data and Event to null and also reset the AutoResetEvent
        /// </summary>
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

        /// <summary>
        ///     Creates a fake chat object.
        /// </summary>
        /// <returns>A FakeChat object.</returns>
        public static FakeChat GetFakeChat()
        {
            var fakeChat = new FakeChat
            {
                Message = NetworkingGlobals.GetRandomString(),
                Timestamp = DateTime.Now.ToString()
            };
            return fakeChat;
        }
    }

    public static class NetworkingGlobals
    {
        public static ICommunicator NewClientCommunicator => CommunicationFactory.GetCommunicator(true, true);

        public static ICommunicator NewServerCommunicator => CommunicationFactory.GetCommunicator(false, true);

        /// <summary>
        ///     Returns a random string of given length. Makes sure "EOF" does not
        ///     appear in the string.
        /// </summary>
        /// <param name="length">Length of the randomly generated string.</param>
        /// <returns>A string.</returns>
        public static string GetRandomString(int length = 10)
        {
            const string chars = "ABCDEGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++) stringChars[i] = chars[random.Next(chars.Length)];

            return new string(stringChars);
        }
    }
}