/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 15/11/2021
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

namespace ScreenSharing
{

	/// <summary>
	/// Client Side screen sharing class
	/// </summary>
	public class ScreenShareClient : INotificationHandler
	{
		// Store the Communicator instance which is used to send screen over the network.
		public ICommunicator _communicator;

		// Boolean to indicate whether other clients are screen sharing.
		public bool OtherSharing;

		// boolean to check whether current client is screen sharing.
		public bool ThisSharing;

		// boolean to control the notifying thread
		public bool IsNotifying;

		// Stores a thread which shares the images.
		public Thread _sharingThread;

		// Stores the thread which will be used to notify UX regarding the shared screen.
		public Thread _notifyingThread;

		// Stores the incoming frames
		public Queue<SharedScreen> FrameQueue;

		// Store an instance of the Serializer
		public ISerializer Serializer;

		// This will be an instance of the UX class which will subscribe for notifications
		public IScreenShare _Ux;

		// Timer will be used to sense disconnection issues.
		public System.Timers.Timer Timer;

		// stores the UserId of the client
		public string UserId;

		// stores the UserName of the client
		public string UserName;

		/// <summary>
		/// Public Constructor which will initialize most of the attributes.
		/// </summary>
		public ScreenShareClient(string uid, string uname)
		{
			UserId = uid;
			UserName = uname;
			Timer = new System.Timers.Timer(2000);
			Timer.Elapsed += OnTimeout;
			Timer.AutoReset = true;

			_communicator = CommunicationFactory.GetCommunicator();
			_communicator.Subscribe(this.GetType().Namespace, this);
			Serializer = new Serializer();

			// creating a thread to capture and send the screen
			_sharingThread = new Thread(Capture);

			// creating a thread to notify the UX and starting its execution
			IsNotifying = true;
			_notifyingThread = new Thread(NotifyUx);
			_notifyingThread.Start();
		}

		/// <summary>
		/// This method will be used by the UX to start sharing the screen.
		/// </summary>
		public void StartSharing()
		{

			ThisSharing = true;
			// starting the execution of the sharing thread
			_sharingThread.Start();

		}

		/// <summary>
		/// This method will be used by the UX to stop sharing the screen.
		/// </summary>
		public void StopSharing()
		{
			ThisSharing = false;
			SharedScreen message = new SharedScreen(UserId, UserName, 0, null);
			string scrn = Serializer.Serialize<SharedScreen>(message);
			_communicator.Send(scrn, MethodInfo.GetCurrentMethod().ReflectedType.Namespace);
			return;
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{
			SharedScreen scrn = Serializer.Deserialize<SharedScreen>(data);
			FrameQueue.Enqueue(scrn);
		}

		/// <summary>
		/// This method is used to convert a bitmap into a byte array. This method is used to facilitate serialization.
		/// </summary>
		private static byte[] GetBytes(Bitmap image)
		{
			using (var output = new MemoryStream())
			{
				image.Save(output, ImageFormat.Bmp);

				return output.ToArray();
			};
		}

		/// <summary>
		/// This method will be used to convert the byte array to bitmap. This method is used to facilitate serialization.
		/// </summary>
		private static Bitmap GetImage(byte[] data)
		{
			//Do not dispose of the stream!!
			return new Bitmap(new MemoryStream(data));
		}

		/// <summary>
		/// This method will include the logic for capturing and sending the screen.
		/// </summary>
		public void Capture()
		{
			if (OtherSharing)
			{
				ThisSharing = false;
				_Ux.OnScreenRecieved(UserId, UserName, -1, null);
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
				if (ThisSharing)
				{
					message = new SharedScreen(UserId, UserName, 1, data);
				}
				else
				{
					return;
				}
				Send(message);
				//string scrn = Serializer.Serialize<SharedScreen>(message);
				//_communicator.Send(scrn, MethodInfo.GetCurrentMethod().ReflectedType.Namespace);
				Thread.Sleep(100);
			}
		}

		public void Send(SharedScreen message)
		{
			string scrn = Serializer.Serialize<SharedScreen>(message);
			_communicator.Send(scrn, MethodInfo.GetCurrentMethod().ReflectedType.Namespace);

		}
		/// <summary>
		/// This method will be used by the UX to subscribe for notifications.
		/// </summary>
		public void Subscribe(IScreenShare listener)
		{
			_Ux = listener;
		}

		/// <summary>
		/// This method will notify the UX.
		/// </summary>
		public void NotifyUx()
		{
			while (IsNotifying)
			{
				while (FrameQueue.Count == 0) ;
				// if the queue is not empty do something
				Timer.Interval = 2000;
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
					Timer.Interval = 2000;
					OtherSharing = false;
					_Ux.OnScreenRecieved(uid, uname, mtype, null);
				}
				else
				{
					Bitmap screen = GetImage(currScreen.Screen);
					_Ux.OnScreenRecieved(uid, uname, mtype, screen);
				}
				if (ThisSharing && uid != UserId)
				{
					ThisSharing = false;
					_Ux.OnScreenRecieved(UserId, UserName, -1, null);
				}
			}
		}

		/// <summary>
		/// This method will be invoked when no updates are recieved for a certain amount of time.
		/// </summary>
		public void OnTimeout(Object source, ElapsedEventArgs e)
		{
			ThisSharing = false;
			OtherSharing = false;
			FrameQueue.Clear();
			_Ux.OnScreenRecieved(UserId, UserName, -2, null);
		}
		~ScreenShareClient()
		{
			IsNotifying = false;
			ThisSharing = false;
			Timer.Dispose();
		}
	}
}
