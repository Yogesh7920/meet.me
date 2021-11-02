using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    class ClientCheckPointHandler : IClientCheckPointHandler
    {
        public int CheckpointNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void FetchCheckpoint(int checkpointNumber)
        {
            throw new NotImplementedException();
        }

        public void SaveCheckpoint()
        {
            throw new NotImplementedException();
        }
    }
}
