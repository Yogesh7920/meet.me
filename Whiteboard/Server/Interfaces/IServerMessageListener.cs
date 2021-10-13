/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 13 Oct 2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Listens to publications from IServerCommunicator
    /// </summary>
    public interface IServerMessageListener
    {
        /// <summary>
        /// Receives XML strings from IServerCommunicator
        /// </summary>
        /// <param name="data"> Serialized Object in form of 
        /// XML string that was sent from some client </param>
        void OnMessageReceived(string data);
    }
}