/// <author>Irene Casmir</author>
/// <created>25/10/2021</created>

using Dashboard;
using Dashboard.Client.SessionManagement;
using System;

namespace Client.ViewModel
{
    public class AuthViewModel
    {
        private IUXClientSessionManager _model;

        public AuthViewModel()
        {
            _model = SessionManagerFactory.GetClientSessionManager();
        }
        
        /// <summary>
        /// Constructor for testing purpose
        /// </summary>
        /// <param name="model"> Instance of type IUXClientSessionManager. </param>
        public AuthViewModel(IUXClientSessionManager model)
        {
            _model = model;
        }
        
        /// <summary>
        /// Sends the credentials entered by user to join a room to the respective method implemented by Session Manager
        /// </summary>
        /// <param name="ip"> IP Address of the server that started the meeting. </param>
        /// <param name="port"> port number. </param>
        /// <param name="username"> Name of the user. </param>
        /// <returns> Boolean denoting the success or failure of whether the login attempt is valid </returns>
        public bool SendForAuth(string ip,int port,string username)
        {
            
            try
            {
                var response = _model.AddClient(ip, port, username);
                return response;
            }
            catch(Exception _)
            {
                return false;
            }
        }
    }
}