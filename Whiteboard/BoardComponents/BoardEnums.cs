/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 11/12/2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Denotes the current state of the WhiteBoard.
    /// </summary>
    public enum BoardState
    {
        Active,
        Inactive
    }

    /// <summary>
    ///     The Possible Shapes that can be drawn on whiteboard.
    /// </summary>
    public enum ShapeType
    {
        Ellipse,
        Rectangle,
        Line,
        Polyline,
        None
    }

    /// <summary>
    ///     Defines the types of Operations that UX will have to perform with the provided UXShapes.
    /// </summary>
    public enum UXOperation
    {
        Create,
        Delete,
        None
    }

    /// <summary>
    ///     Denotes the present Operation. Helps the server to identify the operation performed.
    /// </summary>
    public enum Operation
    {
        Create,
        Delete,
        Modify,
        FetchState,
        FetchCheckpoint,
        CreateCheckpoint,
        ClearState,
        None
    }

    /// <summary>
    ///     Operations which should be rendered real-time on UX.
    /// </summary>
    public enum RealTimeOperation
    {
        Translate,
        Rotate,
        Create,
        Resize
    }

    /// <summary>
    ///     Latches for Resizing operation.
    /// </summary>
    public enum DragPos
    {
        TopRight,
        TopLeft,
        BottomLeft,
        BottomRight,
        Right,
        Left,
        Top,
        Bottom,
        None
    }
}