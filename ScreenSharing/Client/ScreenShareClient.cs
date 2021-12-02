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
using System.Timers;
using System.Windows.Forms;
using Networking;
using Timer = System.Timers.Timer;

namespace ScreenSharing
{
	/// <summary>
	///     Client Side screen sharing class
	/// </summary>
	public class ScreenShareClient : INotificationHandler
    {
        // Store the Communicator instance which is used to send screen over the network.
        public ICommunicator Communicator;

        // Stores the incoming frames
        public Queue<SharedScreen> FrameQueue;

        // boolean to control the notifying thread
        public bool IsNotifying;

        public bool IsSharing;

        // Stores the thread which will be used to notify UX regarding the shared screen.
        public Thread NotifyingThread;

        // Boolean to indicate whether other clients are screen sharing.
        public bool OtherSharing;

        // Store an instance of the Serializer
        public ISerializer Serializer;

        // Stores a thread which shares the images.
        public Thread SharingThread;

        // boolean to check whether current client is screen sharing.
        public bool ThisSharing;


        // Timer will be used to sense disconnection issues.
        public Timer Timer;

        // stores the UserId of the client
        public string UserId;

        // stores the UserName of the client
        public string UserName;

        // This will be an instance of the UX class which will subscribe for notifications
        public IScreenShare Ux;


        /// <summary>
        ///     Public Constructor which will initialize most of the attributes.
        /// </summary>
        public ScreenShareClient()
        {
            Timer = new Timer(10000);
            Timer.Elapsed += OnTimeout;
            Timer.AutoReset = true;
            FrameQueue = new Queue<SharedScreen>();
            Communicator = CommunicationFactory.GetCommunicator();
            Communicator.Subscribe("ScreenSharing", this);
            Serializer = new Serializer();

            OtherSharing = false;
            IsSharing = true;
            ThisSharing = false;
            // creating a thread to capture and send the screen
            SharingThread = new Thread(Capture);
            // starting the execution of the sharing thread
            SharingThread.Start();
            // creating a thread to notify the UX and starting its execution
            IsNotifying = true;
            NotifyingThread = new Thread(NotifyUx);
            NotifyingThread.Start();
        }

        /// <summary>
        ///     This method will be triggered by the Networking team whenever a screen is sent.
        /// </summary>
        public void OnDataReceived(string data)
        {
            try
            {
                var scrn = Serializer.Deserialize<SharedScreen>(data);
                FrameQueue.Enqueue(scrn);
                Trace.WriteLine("[ScreenSharingClient] Recieved data from networking team");
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing: Unable to recieve data");
                Trace.WriteLine(e.Message);
            }
        }


        /// <summary>
        ///     This method will be used by the session manager to set the UserID and User name.
        /// </summary>
        public void SetUser(string uid, string uname)
        {
            UserId = uid;
            UserName = uname;
        }

        /// <summary>
        ///     This method will be used by the UX to start sharing the screen.
        /// </summary>
        public void StartSharing()
        {
            try
            {
                ThisSharing = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing:start sharing not working");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     This method will be used by the UX to stop sharing the screen.
        /// </summary>
        public void StopSharing()
        {
            try
            {
                ThisSharing = false;
                var message = new SharedScreen(UserId, UserName, 0, null);
                Send(message);
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing:stop sharing not working");
                Trace.WriteLine(e.Message);
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
                }

                ;
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
        public void Capture()
        {
            try
            {
                while (IsSharing)
                while (ThisSharing)
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

                    SharedScreen message;

                    message = new SharedScreen(UserId, UserName, 1, data);
                    if (ThisSharing)
                        Send(message);
                    else
                        continue;

                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing:Capturing thread issue");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     This method will be used to send the message
        /// </summary>
        public void Send(SharedScreen message)
        {
            try
            {
                var scrn = Serializer.Serialize(message);
                Communicator.Send(scrn, "ScreenSharing");
                Trace.WriteLine("[ScreenSharingClient] Data sent to Networking");
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing:Unable to send the message");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     This method will be used by the UX to subscribe for notifications.
        /// </summary>
        public void Subscribe(IScreenShare listener)
        {
            try
            {
                Ux = listener;
                Trace.WriteLine("[ScreenSharingClient] Ux has subscribed");
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing:UX unable to subscribe");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     This method will notify the UX.
        /// </summary>
        public void NotifyUx()
        {
            try
            {
                while (IsNotifying)
                {
                    while (FrameQueue.Count == 0) ;

                    // if the queue is not empty take the screen from the queue and pass it to the ux
                    Timer.Interval = 10000;
                    if (Timer.Enabled == false)
                        Timer.Start();
                    OtherSharing = true;
                    var currScreen = FrameQueue.Dequeue();
                    var mtype = currScreen.MessageType;
                    var uid = currScreen.UserId;
                    var uname = currScreen.Username;
                    if (mtype == 0)
                    {
                        Timer.Stop();
                        Timer.Interval = 10000;
                        OtherSharing = false;
                        Ux.OnScreenRecieved(uid, uname, mtype, null);
                    }
                    else
                    {
                        var screen = GetImage(currScreen.Screen);
                        Ux.OnScreenRecieved(uid, uname, mtype, screen);
                    }

                    if (ThisSharing && uid != UserId)
                    {
                        ThisSharing = false;
                        Ux.OnScreenRecieved(UserId, UserName, -1, null);
                    }

                    Trace.WriteLine("[ScreenSharingClient] Ux has been notified");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ScreenSharing: Unable to Notify UX");
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
                ThisSharing = false;
                OtherSharing = false;
                FrameQueue.Clear();
                Ux.OnScreenRecieved(UserId, UserName, -2, null);
                Timer.Stop();
                Timer.Interval = 10000;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ScreenSharing:Timeout event not working properly");
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        ///     This is the destructor.
        /// </summary>
        ~ScreenShareClient()
        {
            IsNotifying = false;
            IsSharing = false;
            ThisSharing = false;
            Timer.Dispose();
        }
    }
}