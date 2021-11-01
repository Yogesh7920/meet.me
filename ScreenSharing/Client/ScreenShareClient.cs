/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 14/10/2021
**/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Networking;

namespace ScreenSharing
{

	/// <summary>
	/// Client Side screen sharing class
	/// </summary>
	public class ScreenShareClient : INotificationHandler
	{
		//Store the Communicator instance which is used to send screen over the network.
		public ICommunicator _communicator;
		//Boolean to indicate whether screen sharing is currently on or not.
		public bool isShared;
		//Stores a thread which shares the images.
		public Thread _sharingThread;
		//Stores the thread which will be used to notify UX regarding the shared screen.
		public Thread _notifyingThread;
		//Stores the incoming frames
		public Queue<SharedScreen> frameQueue;
		// Store an instance of the Serializer
		public ISerializer serializer;
		//Stores the module Id which will be "ScreenSharing"
		public string moduleId;
		// This will be an instance of the UX class which will subscribe for notifications
		public IScreenShare _Ux;
		//Timer will be used to sense disconnection issues.
		public Timer timer;

		/// <summary>
		/// Public Constructor which will initialize most of the attributes.
		/// </summary>
		public ScreenShareClient()
        {
			_communicator = CommunicationFactory.GetCommunicator();
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be used by the UX to start sharing the screen.
		/// </summary>
		/// <param name="userId">the user Id of the user</param>
		/// /// <param name="userName">The user name of the user</param>
		public void startSharing(int userId, string userName) 
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be used by the UX to stop sharing the screen.
		/// </summary>
		public void stopSharing()
        {
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will include the logic for capturing and sending the screen.
		/// </summary>
		public void captureAndSend()
        {
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be used by the UX to subscribe for notifications.
		/// </summary>
		public void subscribe(IScreenShare listener)
        {
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will notify the UX.
		/// </summary>
		public void notifyUx()
        {
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be invoked when no updates are recieved for a certain amount of time.
		/// </summary>
		public void onTimeout()
        {
			throw new NotImplementedException();
		}
	}
}
