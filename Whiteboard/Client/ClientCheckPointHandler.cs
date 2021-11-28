/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 11/1/2021
 * Date Modified: 11/1/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Whiteboard
{   
    /// <summary>
    /// Checkpoint handling at client side
    /// </summary>
    public sealed class ClientCheckPointHandler : IClientCheckPointHandler
    {
        // Instances of other class
        private IClientBoardCommunicator _clientBoardCommunicator =ClientBoardCommunicator.Instance;

        //no. of checkpoints stored on the server
        private int _checkpointNumber = 0;
        // Check if running as part of NUnit
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));


        public void SetCommunicator(IClientBoardCommunicator communicator)
        {
            if (IsRunningFromNUnit)
            {
                _clientBoardCommunicator = communicator;
            }
        }

        /// <summary>
        /// Gets and sets checkpoint number.
        /// </summary>
        public int CheckpointNumber
        {
            get => _checkpointNumber;
            set => _checkpointNumber = value;
        }


        /// <summary>
        ///  Fetches the checkpoint from server.
        /// </summary>
        /// <param name="checkpointNumber"></param>
        /// <param name="UserId"></param>
        /// <param name="currentCheckpointState"></param>
        public void FetchCheckpoint(int checkpointNumber,string UserId, int currentCheckpointState)
        {

            if (checkpointNumber > _checkpointNumber)
            {
                throw new ArgumentException("invalid checkpointNumber");
            }
            else
            {
                //creating boardServerShape object with FETCH_CHECKPOINT object
                List<BoardShape> boardShape = null;
                BoardServerShape boardServerShape = new BoardServerShape(boardShape,
                                                                        Operation.FETCH_CHECKPOINT,
                                                                        UserId,
                                                                        checkpointNumber,
                                                                        currentCheckpointState);

                //sending boardServerShape object to _clientBoardCommunicator
                _clientBoardCommunicator.Send(boardServerShape);

            }


        }

        /// <summary>
        /// creates and saves the checkpoint
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="currentCheckpointState"></param>
        public void SaveCheckpoint(string UserId, int currentCheckpointState)
        {
            try
            {
                // increasing the checkpoint number by one
                _checkpointNumber++;

                //creating boardServerShape object with CREATE_CHECKPOINT object
                List<BoardShape> boardShape = null;
                BoardServerShape boardServerShape = new BoardServerShape(boardShape,
                                                                        Operation.CREATE_CHECKPOINT,
                                                                        UserId,
                                                                        _checkpointNumber,
                                                                        currentCheckpointState);

                //sending boardServerShape object to _clientBoardCommunicator
                _clientBoardCommunicator.Send(boardServerShape);

            }
            
            catch (Exception e)
            {
                Trace.WriteLine("ClientCheckPointHandler.SaveCheckPoint: An exception occured.");
                Trace.WriteLine(e.Message);
            }
        }

            
        
    }
}
