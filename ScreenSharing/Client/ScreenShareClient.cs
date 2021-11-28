/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 26/11/2021
**/

using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using Networking;
using System.Reflection;
using System.Diagnostics;

namespace ScreenSharing
{

	/// <summary>
	/// Client Side screen sharing class
	/// </summary>
	public class ScreenShareClient : INotificationHandler
	{
		// Store the Communicator instance which is used to send screen over the network.
		public ICommunicator Communicator;

		// Boolean to indicate whether other clients are screen sharing.
		public bool OtherSharing;

		// boolean to check whether current client is screen sharing.
		public bool ThisSharing;

		// boolean to control the notifying thread
		public bool IsNotifying;

		// Stores a thread which shares the images.
		public Thread SharingThread;

		// Stores the thread which will be used to notify UX regarding the shared screen.
		public Thread NotifyingThread;

		// Stores the incoming frames
		public Queue<SharedScreen> FrameQueue;

		// Store an instance of the Serializer
		public ISerializer Serializer;

		// This will be an instance of the UX class which will subscribe for notifications
		public IScreenShare Ux;


		// Timer will be used to sense disconnection issues.
		public System.Timers.Timer Timer;

		// stores the UserId of the client
		public string UserId;

		// stores the UserName of the client
		public string UserName;

		/// <summary>
		/// Public Constructor which will initialize most of the attributes.
		/// </summary>
		public ScreenShareClient()
		{
            Timer = new System.Timers.Timer(10000);
            Timer.Elapsed += OnTimeout;
			Timer.AutoReset = true;
            FrameQueue = new Queue<SharedScreen>();
            Communicator = CommunicationFactory.GetCommunicator();
			Communicator.Subscribe("ScreenSharing", this);
			Serializer = new Serializer();

			// creating a thread to capture and send the screen
			SharingThread = new Thread(Capture);

			// creating a thread to notify the UX and starting its execution
			IsNotifying = true;
			NotifyingThread = new Thread(NotifyUx);
			NotifyingThread.Start();
		}

		/// <summary>
		/// This method will be used by the session manager to set the UserID and User name.
		/// </summary>
		public void SetUser(string uid, string uname)
        {
			UserId = uid;
			UserName = uname;
		}

		/// <summary>
		/// This method will be used by the UX to start sharing the screen.
		/// </summary>
		public void StartSharing()
		{
			try
            {
				ThisSharing = true;
				// starting the execution of the sharing thread
				SharingThread.Start();
			}
			catch(Exception e)
            {
				Trace.WriteLine("ScreenSharing:start sharing not working");
                Trace.WriteLine(e.Message);
            }

		}

		/// <summary>
		/// This method will be used by the UX to stop sharing the screen.
		/// </summary>
		public void StopSharing()
		{
			try
            {
				ThisSharing = false;
				SharedScreen message = new SharedScreen(UserId, UserName, 0, null);
				Send(message);
				return;
			}
			catch(Exception e)
            {
				Trace.WriteLine("ScreenSharing:stop sharing not working");
                Trace.WriteLine(e.Message);
            }
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{
			try
			{
				SharedScreen scrn = Serializer.Deserialize<SharedScreen>(data);
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
		/// This method is used to convert a bitmap into a byte array. This method is used to facilitate serialization.
		/// </summary>
		private static byte[] GetBytes(Bitmap image)
		{
			try
			{
				using (var output = new MemoryStream())
				{
					image.Save(output, ImageFormat.Bmp);
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
		/// This method will be used to convert the byte array to bitmap. This method is used to facilitate serialization.
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
		/// This method will include the logic for capturing and sending the screen.
		/// </summary>
		public void Capture()
		{
			try
            {
				if (OtherSharing)
				{
					ThisSharing = false;
					Ux.OnScreenRecieved(UserId, UserName, -1, null);
					return;
				}
				while (ThisSharing)
				{

					Bitmap bitmap = new Bitmap(
						Screen.PrimaryScreen.Bounds.Width,
						Screen.PrimaryScreen.Bounds.Height
						);

					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
					Size curSize = new Size(32, 32);
					Cursors.Default.Draw(graphics, new Rectangle(Cursor.Position, curSize));
					Bitmap bitmap480p = new Bitmap(720, 480);
					Graphics graphics480p = Graphics.FromImage(bitmap480p);
					graphics480p.DrawImage(bitmap, 0, 0, 720, 480);

					byte[] data = GetBytes(bitmap480p);

					SharedScreen message;
					
					message = new SharedScreen(UserId, UserName, 1, data);
					if(ThisSharing)
                    { 
						Send(message);
					}
					else
                    {
						return;
                    }

					Thread.Sleep(1000);
				}
			}
			catch(Exception e)
            {
				Trace.WriteLine("ScreenSharing:Capturing thread issue");
                Trace.WriteLine(e.Message);
            }
		}

		/// <summary>
		/// This method will be used to send the message
		/// </summary>
		public void Send(SharedScreen message)
		{
			try
            {
				string scrn = Serializer.Serialize<SharedScreen>(message);
				Communicator.Send(scrn, "ScreenSharing");
				Trace.WriteLine("[ScreenSharingClient] Data sent to Networking");
			}
			catch(Exception e)
            {
				Trace.WriteLine("ScreenSharing:Unable to send the message");
                Trace.WriteLine(e.Message);
            }
		}

		/// <summary>
		/// This method will be used by the UX to subscribe for notifications.
		/// </summary>
		public void Subscribe(IScreenShare listener)
		{
            try
			{
				Ux = listener;
				Trace.WriteLine("[ScreenSharingClient] Ux has subscribed");
			}
			catch(Exception e)
            {
				Trace.WriteLine("ScreenSharing:UX unable to subscribe");
                Trace.WriteLine(e.Message);
            }
		}

		/// <summary>
		/// This method will notify the UX.
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
						SharedScreen currScreen = FrameQueue.Dequeue();
						int mtype = currScreen.MessageType;
						string uid = currScreen.UserId;
						string uname = currScreen.Username;
						if (mtype == 0)
						{
							Timer.Stop();
							Timer.Interval = 10000;
							OtherSharing = false;
							Ux.OnScreenRecieved(uid, uname, mtype, null);
						}
						else
						{
							Bitmap screen = GetImage(currScreen.Screen);
							Ux.OnScreenRecieved(uid, uname, mtype, screen);
						}
						if (ThisSharing && uid != UserId)
						{
							ThisSharing = false;
							Ux.OnScreenRecieved(UserId, UserName, -1, null);
						}
						Trace.WriteLine("[ScreenSharingClient] Ux has notified");
				}
			}
			catch(Exception e)
			{
				Trace.WriteLine("ScreenSharing: Unable to Notify UX");
				Trace.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// This method will be invoked when no updates are recieved for a certain amount of time.
		/// </summary>
		public void OnTimeout(Object source, ElapsedEventArgs e)
		{
			try
			{
				ThisSharing = false;
				OtherSharing = false;
				FrameQueue.Clear();
				Ux.OnScreenRecieved(UserId, UserName, -2, null);
			}
			catch(Exception ex)
            {
				Trace.WriteLine("ScreenSharing:Timeout event not working properly");
                Trace.WriteLine(ex.Message);
            }
		}

		/// <summary>
		/// This is the destructor.
		/// </summary>
		~ScreenShareClient()
		{
			IsNotifying = false;
			ThisSharing = false;
			Timer.Dispose();
		}
	}
}
