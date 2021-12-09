/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 28/11/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Networking;
using Timer = System.Timers.Timer;

namespace ScreenSharing
{
	/// <summary>
	///     Client Side screen sharing class
	/// </summary>
	public class ScreenShareClient : INotificationHandler,IDisposable
    {
        // Store the _communicator instance which is used to send screen over the network.
        private ICommunicator _communicator;

        // Stores the incoming frames
        private Queue<SharedScreen> frameQueue;

        // boolean to control the notifying thread
        //public bool IsNotifying;

        // boolean to run the sharing thread infinitely
        //public bool IsSharing;

        // Stores a thread which shares the images.
        private Task sharingThread;

        private CancellationTokenSource isSharing;

        // Stores the thread which will be used to notify _ux regarding the shared screen.
        private Task notifyingThread;

        private CancellationTokenSource isNotifying;


        // Store an instance of the _serializer
        private ISerializer _serializer;

        // boolean to check whether current client is screen sharing.
        public bool thisSharing;


        // timer will be used to sense disconnection issues.
        public Timer timer;

        // stores the userId of the client
        public string userId;

        // stores the userName of the client
        public string userName;

        // This will be an instance of the _ux class which will subscribe for notifications
        private IScreenShare _ux;


        /// <summary>
        ///     Public Constructor which will initialize most of the attributes.
        /// </summary>
        public ScreenShareClient()
        {
            timer = new Timer(10000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            frameQueue = new Queue<SharedScreen>();

            _communicator = CommunicationFactory.GetCommunicator();
            _communicator.Subscribe("ScreenSharing", this);
            _serializer = new Serializer();

            isSharing = new CancellationTokenSource();
            thisSharing = false;
            // creating a thread to capture and send the screen
            sharingThread = new Task(CaptureAndSend,isSharing.Token, TaskCreationOptions.LongRunning);
            // starting the execution of the sharing thread
            sharingThread.Start();
            // creating a thread to notify the _ux and starting its execution
            isNotifying = new CancellationTokenSource();
            notifyingThread = new Task(NotifyUx,isNotifying.Token,TaskCreationOptions.LongRunning);
            notifyingThread.Start();
        }

        public ScreenShareClient(ICommunicator communicator)
        {

            timer = new Timer(10000);
            timer.Elapsed += OnTimeout;
            timer.AutoReset = true;
            frameQueue = new Queue<SharedScreen>();

            _communicator = communicator;
            _serializer = new Serializer();

            thisSharing = false;
            try
            {
                isSharing = new CancellationTokenSource();
                sharingThread = new Task(CaptureAndSend, isSharing.Token, TaskCreationOptions.LongRunning);
                sharingThread.Start();
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharingClent : Problem in creating Sharing thread");
                Trace.WriteLine(e.Message);
            }
            // creating a thread to notify the _ux and starting its execution
            try
            {
                isNotifying = new CancellationTokenSource();
                notifyingThread = new Task(NotifyUx, isNotifying.Token, TaskCreationOptions.LongRunning);
                notifyingThread.Start();
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharingClient: Problem in creating notifying thread");
                Trace.WriteLine(e.Message);
            }

        }

        /// <summary>
        ///     This method will be triggered by the Networking team whenever a screen is sent.
        /// </summary>
        public void OnDataReceived(string data)
        {
            
                var scrn = _serializer.Deserialize<SharedScreen>(data);
                frameQueue.Enqueue(scrn);
                Trace.WriteLine("[ScreenSharingClient] Recieved data from networking team");
            
            
        }


        /// <summary>
        ///     This method will be used by the session manager to set the userId and User name.
        /// </summary>
        public void SetUser(string uid, string uname)
        {
            userId = uid;
            userName = uname;
        }

        /// <summary>
        ///     This method will be used by the _ux to start sharing the screen.
        /// </summary>
        public void StartSharing()
        {
            thisSharing = true;
        }

        /// <summary>
        ///     This method will be used by the _ux to stop sharing the screen.
        /// </summary>
        public void StopSharing()
        {
            if (thisSharing)
            {
                thisSharing = false;
                var message = new SharedScreen(userId, userName, 0, null);
                Send(message);
            }
        }

        /// <summary>
        ///     This method is used to convert a bitmap into a byte array. This method is used to facilitate serialization.
        /// </summary>
        private static byte[] GetBytes(Bitmap image)
        {
            try
            {
                using (var output = new MemoryStream())
                {
                    image.Save(output, ImageFormat.Jpeg);
                    return output.ToArray();
                };
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing:Unable to convert Bitmap to byte array");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     This method will be used to convert the byte array to bitmap. This method is used to facilitate serialization.
        /// </summary>
        private static Bitmap GetImage(byte[] data)
        {
            try
            {
                return new Bitmap(new MemoryStream(data));
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing: Unable to convert byte array to bitmap");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     This method will include the logic for capturing and sending the screen.
        /// </summary>
        public void CaptureAndSend()
        {
            
                while (!isSharing.Token.IsCancellationRequested)
                {
                    while (thisSharing)
                    {
                        SharedScreen message = Capture();
                        if (thisSharing)
                            Send(message);
                        else
                            continue;

                        Thread.Sleep(1000);
                    }
                }            
        }

        public SharedScreen Capture()
        {
            try
            {
                var bitmap = new Bitmap(
                               Screen.PrimaryScreen.Bounds.Width,
                               Screen.PrimaryScreen.Bounds.Height
                           );

                var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                var curSize = new Size(32, 32);
                Cursors.Default.Draw(graphics, new Rectangle(Cursor.Position, curSize));
                var bitmap480p = new Bitmap(720, 480);
                var graphics480p = Graphics.FromImage(bitmap480p);
                graphics480p.DrawImage(bitmap, 0, 0, 720, 480);

                var data = GetBytes(bitmap480p);

                SharedScreen message = new SharedScreen(userId, userName, 1, data);
                return message;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharingClient: Unable to Capture");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     This method will be used to send the message
        /// </summary>
        public void Send(SharedScreen message)
        {
            
                var scrn = _serializer.Serialize(message);
                _communicator.Send(scrn, "ScreenSharing");
                Trace.WriteLine("[ScreenSharingClient] Data sent to Networking");
            
        }

        /// <summary>
        ///     This method will be used by the _ux to subscribe for notifications.
        /// </summary>
        public void Subscribe(IScreenShare listener)
        {
            
            _ux = listener;
            Trace.WriteLine("[ScreenSharingClient] _ux has subscribed");
            
           
        }

        /// <summary>
        ///     This method will notify the _ux.
        /// </summary>
        public void NotifyUx()
        {
            try
            {
                while (!isNotifying.Token.IsCancellationRequested)
                {
                    while (frameQueue.Count > 0)
                    { 
                        // if the queue is not empty take the screen from the queue and pass it to the _ux
                        timer.Interval = 10000;
                        if (timer.Enabled == false)
                            timer.Start();
                        var currScreen = frameQueue.Dequeue();
                        var mtype = currScreen.messageType;
                        var uid = currScreen.userId;
                        var uname = currScreen.userName;
                        if (mtype == 0)
                        {
                            timer.Stop();
                            timer.Interval = 10000;
                            _ux.OnScreenRecieved(uid, uname, mtype, null);
                        }
                        else
                        {
                            var screen = GetImage(currScreen.screen);
                            _ux.OnScreenRecieved(uid, uname, mtype, screen);
                        }

                        //if (thisSharing && uid != userId)
                        //{
                        //    thisSharing = false;
                        //    _ux.OnScreenRecieved(userId, userName, -1, null);
                        //}

                        Trace.WriteLine("[ScreenSharingClient] _ux has been notified");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing: Unable to Notify _ux");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     This method will be invoked when no updates are recieved for a certain amount of time.
        /// </summary>
        public void OnTimeout(object source, ElapsedEventArgs e)
        {
            try
            {
                thisSharing = false;
                frameQueue.Clear();
                _ux.OnScreenRecieved(userId, userName, -2, null);
                timer.Stop();
                timer.Interval = 10000;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ScreenSharing:Timeout event not working properly");
                Trace.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            thisSharing=false;
            frameQueue.Clear();
            isSharing.Cancel();
            isNotifying.Cancel();
            timer.Dispose();
        }
    }
}