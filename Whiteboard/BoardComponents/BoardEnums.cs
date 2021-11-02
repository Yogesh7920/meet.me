/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 11/01/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Denotes the current state of the WhiteBoard.
    /// </summary>
    public enum BoardState
    {
        ACTIVE,
        INACTIVE
    }

    public enum ShapeType
    {
        ELLIPSE,
        RECTANGLE,
        LINE,
        POLYLINE
    }

    public enum UXOperation
    {
        CREATE,
        DELETE
    }

    public enum Operation
    {
        CREATE,
        DELETE,
        MODIFY,
        FETCH_STATE,
        FETCH_CHECKPOINT,
        CREATE_CHECKPOINT,
        NONE
    }

}