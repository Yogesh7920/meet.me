/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 26 Nov 2021
**/

using System;
using System.Diagnostics;
using Networking;

namespace Whiteboard
{
    /// <summary>
    ///     Bridge the gap between Server side White Board Modules and Networking module
    /// </summary>
    public sealed class ServerBoardCommunicator : INotificationHandler, IServerBoardCommunicator
    {
        private static ServerBoardCommunicator instance;
        private static ISerializer serializer;
        private static ICommunicator communicator;
        private static readonly string moduleIdentifier = "Whiteboard";
        private static ServerBoardStateManager stateManager;

        /// <summary>
        ///     private constructor for a singleton
        /// </summary>
        private ServerBoardCommunicator()
        {
        }

        /// <summary>
        ///     instance getter
        /// </summary>
        public static ServerBoardCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerBoardCommunicator();
                    serializer = new Serializer();
                    communicator = CommunicationFactory.GetCommunicator(false);
                    communicator.Subscribe(moduleIdentifier, instance);
                    stateManager = new ServerBoardStateManager();
                }

                return instance;
            }
        }

        public void OnDataReceived(string data)
        {
            try
            {
                Trace.WriteLine("ServerBoardCommunicator.onDataReceived: Receiving the XML string");
                var deserializedObject = serializer.Deserialize<BoardServerShape>(data);
                var userId = deserializedObject.RequesterId;
                if (deserializedObject.OperationFlag == Operation.FetchState)
                {
                    var shapes = stateManager.FetchState(userId);
                    Send(shapes, userId);
                }
                else if (deserializedObject.OperationFlag == Operation.FetchCheckpoint)
                {
                    var checkPointNumber = deserializedObject.CheckpointNumber;
                    var shapes = stateManager.FetchCheckpoint(checkPointNumber, userId);
                    Send(shapes);
                }
                else if (deserializedObject.OperationFlag == Operation.CreateCheckpoint)
                {
                    var shape = stateManager.SaveCheckpoint(userId);
                    Send(shape);
                }
                else if (deserializedObject.OperationFlag == Operation.Create ||
                         deserializedObject.OperationFlag == Operation.Delete ||
                         deserializedObject.OperationFlag == Operation.Modify ||
                         deserializedObject.OperationFlag == Operation.ClearState
                )
                {
                    var resp = stateManager.SaveUpdate(deserializedObject);
                    if (resp) communicator.Send(data, moduleIdentifier);
                }
                else
                {
                    Console.WriteLine("Unidentified Operation at ServerBoardCommunicator");
                }

                Trace.WriteLine("ServerBoardCommunicator.OnDataReceived: Took necessary actions on received object");
            }
            catch (Exception e)
            {
                Trace.WriteLine("ServerBoardCommunicator.onDataReceived: Exception Occured");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     serializes the shape objects and passes it to communicator.send()
        /// </summary>
        /// <param name="clientUpdate"> the object to be passed to client</param>
        public void Send(BoardServerShape clientUpdate, string clientId = "all")
        {
            try
            {
                Trace.WriteLine("ServerBoardCommunicator.Send: Sending BoardServerShape object");
                var xml_obj = serializer.Serialize(clientUpdate);
                if (clientId == "all")
                    communicator.Send(xml_obj, moduleIdentifier);
                else
                    communicator.Send(xml_obj, moduleIdentifier, clientId);
                Trace.WriteLine("ServerBoardCommunicator.Send: Sent BoardServerShape object");
            }
            catch (Exception e)
            {
                Trace.WriteLine("ServerBoardCommunicator.Send: Exception Occured");
                Trace.WriteLine(e.Message);
            }
        }
    }
}