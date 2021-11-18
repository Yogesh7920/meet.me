/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 11/01/2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Denotes the current state of the WhiteBoard.
    /// </summary>
    public enum BoardState
    {
        ACTIVE,
        INACTIVE
    }

    /// <summary>
    ///     The Possible Shapes that can be drawn on whiteboard.
    /// </summary>
    public enum ShapeType
    {
        ELLIPSE,
        RECTANGLE,
        LINE,
        POLYLINE
    }

    /// <summary>
    ///     Defines the types of Operations that UX will have to perform with the provided UXShapes.
    /// </summary>
    public enum UXOperation
    {
        CREATE,
        DELETE
    }

    /// <summary>
    ///     Denotes the present Operation. Helps the server to identify the operation performed.
    /// </summary>
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