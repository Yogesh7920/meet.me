/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 11/12/2021
**/

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

    /// <summary>
    /// The Possible Shapes that can be drawn on whiteboard.
    /// </summary>
    public enum ShapeType
    {
        ELLIPSE,
        RECTANGLE,
        LINE,
        POLYLINE,
        NONE
    }

    /// <summary>
    /// Defines the types of Operations that UX will have to perform with the provided UXShapes.
    /// </summary>
    public enum UXOperation
    {
        CREATE,
        DELETE,
        NONE
    }

    /// <summary>
    /// Denotes the present Operation. Helps the server to identify the operation performed.
    /// </summary>
    public enum Operation
    {
        CREATE,
        DELETE,
        MODIFY,
        FETCH_STATE,
        FETCH_CHECKPOINT,
        CREATE_CHECKPOINT,
        CLEAR_STATE,
        NONE
    }

    /// <summary>
    /// Operations which should be rendered real-time on UX.
    /// </summary>
    public enum RealTimeOperation
    {
        TRANSLATE,
        ROTATE,
        CREATE
    }

    /// <summary>
    /// Latches for Resizing operation.
    /// </summary>
    public enum DragPos
    {
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
        RIGHT,
        LEFT,
        TOP,
        BOTTOM,
        NONE
    };
}