/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 11/12/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Networking;
using Timer = System.Timers.Timer;

namespace ScreenSharing
{
	/// <summary>
	///     Server Side screen sharing class
	/// </summary>
	public class ScreenShareServer : INotificationHandler,IDisposable
    {
        //Store the _communicator used to send screen over the network.
        private ICommunicator _communicator;

        // Queue to store the incoming frames
        private Queue<SharedScreen> frameQueue;

        //public bool IsSharing;

        // Stores an instance of the _serializer
        private ISerializer _serializer;

        //Thread to share the required signal to the required machines.
        private Thread sharingThread;

        private CancellationTokenSource isSharing;

        //timer will be used to sense disconnection issues.
        public Timer timer;

        //Stores the user Id of the user currently sharing the screen.
        public string userId;

        /// <summary>
        ///     Public Constructor which will initialize most of the attributes.
        /// </summary>
        public ScreenShareServer()
        {
            userId = "-";
            timer = new Timer(10000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            frameQueue = new Queue<SharedScreen>();

            _communicator = CommunicationFactory.GetCommunicator(false);
            _communicator.Subscribe("ScreenSharing", this);
            _serializer = new Serializer();

            isSharing = new CancellationTokenSource();
            sharingThread = new Thread(Share);
            sharingThread.Start();
        }

        /// <summary>
        ///     Public Constructor which is used for unit testing.
        /// </summary>
        public ScreenShareServer(ICommunicator communicator)
        {
            userId = "-";
            timer = new Timer(10000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            frameQueue = new Queue<SharedScreen>();

            _communicator = communicator;
            _serializer = new Serializer(); ;

            isSharing = new CancellationTokenSource();
            sharingThread = new Thread(Share);
            sharingThread.Start();
        }

        /// <summary>
        ///     This method will be triggered by the Networking team whenever a screen is sent.
        /// </summary>
        public void OnDataReceived(string data)
        {
            
            var scrn = _serializer.Deserialize<SharedScreen>(data);
            frameQueue.Enqueue(scrn);
            Trace.WriteLine("[ScreenSharingServer] Data received from Networking");
        }

        /// <summary>
        ///     This method will implement the logic of sharing the required signal.
        /// </summary>
        public void Share()
        {
                while (!isSharing.Token.IsCancellationRequested)
                {
                    while (frameQueue.Count > 0)
                    { 
                        timer.Interval = 10000;
                        if (timer.Enabled == false)
                            timer.Start();
                        var currScreen = frameQueue.Dequeue();
                        if (userId == "-")
                        {
                            // this is the case when server is idle and someone wants to share screen
                            userId = currScreen.userId;
                            if (currScreen.messageType == 0)
                            {
                                userId = "-";
                                timer.Stop();
                                timer.Interval = 10000;
                            }

                            // Broadcasting the screen
                            var data = _serializer.Serialize(currScreen);
                            _communicator.Send(data, "ScreenSharing");
                            Trace.WriteLine("[ScreenSharingServer] Data sent to Networking");
                        }
                        else if (currScreen.userId != userId)
                        {
                            // this is a case of simultaneous sharing and the user who is trying to share has to be rejected
                        }
                        else
                        {
                            if (currScreen.messageType == 0)
                            {
                                userId = "-";
                                timer.Stop();
                                timer.Interval = 10000;
                            }

                            // Broadcasting the screen
                            var data = _serializer.Serialize(currScreen);
                            _communicator.Send(data, "ScreenSharing");
                            Trace.WriteLine("[ScreenSharingServer] Data sent to Networking");
                        }
                    }
                }
   
        }

        /// <summary>
        ///     This method will be invoked when no updates are recieved for a certain amount of time.
        /// </summary>
        public void OnTimeout(object source, ElapsedEventArgs e)
        {
        
                userId = "-";
                frameQueue.Clear();
                timer.Stop();
                timer.Interval = 10000;
        }

        /// <summary>
        ///     Used to Dispose the object.
        /// </summary>
        public void Dispose()
        {
            frameQueue.Clear();
            isSharing.Cancel();
            timer.Dispose();
        }
    }
}