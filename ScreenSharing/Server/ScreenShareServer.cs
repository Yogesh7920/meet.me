/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 7/11/2021
**/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Networking;

namespace ScreenSharing 
{
	/// <summary>
	/// Server Side screen sharing class
	/// </summary>
	public class ScreenShareServer : INotificationHandler
	{
		//Store the Communicator used to send screen over the network.
		public ICommunicator _communicator;

		//Thread to share the required signal to the required machines.
		public Thread _sharingThread;

		// Queue to store the incoming frames
		public Queue<SharedScreen> frameQueue;

		// Stores an instance of the Serializer
		public ISerializer serializer;

		public bool isSharing;

		//Stores the moduleId which will be "ScreenSharing"
		public string moduleId;

		//Timer will be used to sense disconnection issues.
		public Timer timer;

		//Stores the user Id of the user currently sharing the screen.
		public string userId;

		/// <summary>
		/// Public Constructor which will initialize most of the attributes.
		/// </summary>
		public ScreenShareServer()
		{
			userId = "-";
			_communicator = CommunicationFactory.GetCommunicator();
			_communicator.Subscribe(this.GetType().Namespace, this);
			serializer = new Serializer();

			isSharing = true;
			_sharingThread = new Thread(share);
			_sharingThread.Start();
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{
            SharedScreen scrn = serializer.Deserialize<SharedScreen>(data);
            frameQueue.Enqueue(scrn);
		}

		/// <summary>
        /// This method will implement the logic of sharing the required signal.
        /// </summary>
		public void share()
        {
			while(isSharing)
            {
				while (frameQueue.Count == 0) ;
				SharedScreen currScreen = frameQueue.Dequeue();
				if(userId == "-")
                {
					// this is the case when server is idle and someone wants to share screen
					userId = currScreen.userId;
                }
				else if(currScreen.userId != userId)
                {
					// this is a case of simultaneous sharing and the user who is sharing has to be rejected
                }
                else
                {
					// Broadcasting the screen
					string data = serializer.Serialize<SharedScreen>(currScreen);
					_communicator.Send(data, MethodInfo.GetCurrentMethod().ReflectedType.Namespace);
                }
            }
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
