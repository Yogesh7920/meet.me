/// <author>Siddharth Sha</author>
/// <created>15/11/2021</created>
/// <summary>
///		This file contains the test whiteboard
///		for testing purpose
/// </summary>
/// 
using System;
using Whiteboard;

namespace Testing.Dashboard.TestModels
{
    class TestWhiteBoard : IClientBoardStateManager
    {
        public TestWhiteBoard()
        {
            isWhiteBoardInitialised = false;
        }
        public void ClearWhiteBoard()
        {
            throw new NotImplementedException();
        }

        public void FetchCheckpoint(int checkpointNumber)
        {
            throw new NotImplementedException();
        }

        public void SaveCheckpoint()
        {
            throw new NotImplementedException();
        }

        public void SetUser(string userId)
        {
            this.userId = userId;
        }

        public void Start()
        {
            isWhiteBoardInitialised = true;
        }

        public void Subscribe(IClientBoardStateListener listener, string identifier)
        {
            throw new NotImplementedException();
        }

        bool IClientBoardStateManager.ClearWhiteBoard()
        {
            throw new NotImplementedException();
        }

        public bool isWhiteBoardInitialised;
        public string userId;
    }
}
