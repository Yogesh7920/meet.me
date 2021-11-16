using System;

namespace Whiteboard
{
    internal class ClientCheckPointHandler : IClientCheckPointHandler
    {
        public int CheckpointNumber
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

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