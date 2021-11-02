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
        
        public bool SendForAuth(string ip,int port,string username)
        {
            var response = _model.AddClient(ip,port,username);
            return response;
          
        }
    }
}