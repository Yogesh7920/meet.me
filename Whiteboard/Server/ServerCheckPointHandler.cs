/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 11/1/2021
 * Date Modified: 11/1/2021
**/

using System;
using System.Collections.Generic;

namespace Whiteboard
{
    internal class ServerCheckPointHandler : IServerCheckPointHandler
    {
        public List<BoardShape> FetchCheckpoint(int checkpointNumber)
        {
            throw new NotImplementedException();
        }

        public int GetCheckpointsNumber()
        {
            throw new NotImplementedException();
        }

        public int SaveCheckpoint(List<BoardShape> boardShapes, string userId)
        {
            throw new NotImplementedException();
        }
    }
}