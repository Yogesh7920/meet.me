/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/12/2021
 * Date Modified: 11/25/2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Class containing constants.
    /// </summary>
    public static class BoardConstants
    {
        /// <summary>
        ///     undo-redo capacity
        /// </summary>
        public const int UndoRedoStackSize = 7;

        /// <summary>
        ///     Min possible size for data structure to be considered empty
        /// </summary>
        public const int EmptySize = 0;

        /// <summary>
        ///     The no. of update (Create, Modify, Delete) a BoardServerShape can contain.
        /// </summary>
        public const int SingleUpdateSize = 1;

        /// <summary>
        ///     The higher user level. User who has full control.
        /// </summary>
        public const int HighUserLevel = 1;

        /// <summary>
        ///     The lower user level. User who has partial control.
        /// </summary>
        public const int LowUserLevel = 0;

        /// <summary>
        ///     The initial state when whiteboard is clear.
        /// </summary>
        public const int InitialCheckpointState = 0;

        /// <summary>
        ///     Minimum Allowed Height of a shape.
        /// </summary>
        public const float MinHeight = (float) 0.1;

        /// <summary>
        ///     Maximum Allowed Width for a shape.
        /// </summary>
        public const float MinWidth = (float) 0.1;

        /// <summary>
        ///     Allowed variation in floats to be declared as equal.
        /// </summary>
        public const float AllowedDelta = (float) 0.02;
    }
}