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
    public interface IMessageListener
    {

        /// <summary>
        /// Receives XML strings from ICommunicator
        /// </summary>
        /// <param name="data"> Serialized Object in form of 
        /// XML string that was sent from server </param>
        void OnMessageReceived(string data);
    }
}