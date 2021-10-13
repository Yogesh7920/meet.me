/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/11/2021
 * Date Modified: 10/11/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Interface to be implemented by subscribers of ClientBoardStateManager.
    /// </summary>
    public interface IClientBoardStateListener
    {
        /// <summary>
        /// Handles the reception of update from state manager. 
        /// </summary>
        /// <param name="shapeUpdates">List of UXShapes for UX to render.</param>
        void OnUpdateFromStateManager(List<UXShape> shapeUpdates); 
    }
}
