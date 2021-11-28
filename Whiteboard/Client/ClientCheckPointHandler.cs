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

namespace Whiteboard
{   
    /// <summary>
    /// Checkpoint handling at client side
    /// </summary>
    internal class ClientCheckPointHandler : IClientCheckPointHandler
    {
        // Instances of other class
        private IClientBoardCommunicator _clientBoardCommunicator =ClientBoardCommunicator.Instance;

        //no. of checkpoints stored on the server
        private int _checkpointNumber = 0;

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
            try
            {
                if (checkpointNumber <= CheckpointNumber)
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
                else
                {
                    throw new ArgumentException("invalid checkpointNumber");
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine("ClientCheckPointHandler.FetchCheckPoint: An exception occured.");
                Trace.WriteLine(e.Message);
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
