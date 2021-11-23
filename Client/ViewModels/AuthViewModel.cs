using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;
using Dashboard.Client.SessionManagement;

namespace Client.ViewModels
{
    public class AuthViewModel
    {
        private IUXClientSessionManager _model;
        public AuthViewModel()
        {
           _model = SessionManagerFactory.GetClientSessionManager();
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
                return true;
            }
        }
    }
}