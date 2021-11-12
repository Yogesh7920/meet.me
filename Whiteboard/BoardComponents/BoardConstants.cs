/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/12/2021
 * Date Modified: 11/12/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Class containing constants.
    /// </summary>
    public static class BoardConstants
    {
        /// <summary>
        /// undo-redo capacity
        /// </summary>
        public const int UNDO_REDO_STACK_SIZE = 7;

        /// <summary>
        /// Min possible size for data structure to be considered empty
        /// </summary>
        public const int EMPTY_SIZE = 0;

        /// <summary>
        /// The no. of update (CREATE, MODIFY, DELETE) a BoardServerShape can contain.
        /// </summary>
        public const int SINGLE_UPDATE_SIZE = 1;
    }
}
