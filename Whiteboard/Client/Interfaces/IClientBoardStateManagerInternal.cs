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
    /// Internal interface to be used by OperationsHandler to update state, select a shape and do undo-redo. 
    /// </summary>
    public interface IClientBoardStateManagerInternal
    {
        /// <summary>
        /// Saves the update on a shape in the state and sends it to server for broadcast. 
        /// </summary>
        /// <param name="boardShape">The object describing shape.</param>
        /// <returns>Boolean to indicate success status of update.</returns>
        bool SaveOperation(BoardShape boardShape);

        /// <summary>
        /// Fetches the BoardShape object from the map.  
        /// </summary>
        /// <param name="id">Unique identifier for a BoardShape object.</param>
        /// <returns>BoardShape object with unique id equal to id.</returns>
        BoardShape GetBoardShape(string id);

        /// <summary>
        /// Does the undo operation for client. 
        /// </summary>
        /// <returns>List of UXShapes for UX to render.</returns>
        List<UXShape> DoUndo();

        /// <summary>
        /// Does the redo operation for client. 
        /// </summary>
        /// <returns>List of UXShapes for UX to render.</returns>
        List<UXShape> DoRedo();

        /// <summary>
        /// Provides the current user's id. 
        /// </summary>
        /// <returns>The user id of current user.</returns>
        string GetUser();
    }
}
