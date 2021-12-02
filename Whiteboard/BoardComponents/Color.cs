/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 11/12/2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Color for WhiteBoard.
    /// </summary>
    public class BoardColor
    {
        private int _b;
        private int _g;
        private int _r;

        /// <summary>
        ///     Constructor for BoardColor.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        public BoardColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        ///     The Red component.
        /// </summary>
        public int R
        {
            get => _r;
            set => _r = value > 255 ? 255 : value;
        }

        /// <summary>
        ///     The green component.
        /// </summary>
        public int G
        {
            get => _g;
            set => _g = value > 255 ? 255 : value;
        }

        /// <summary>
        ///     The blue component.
        /// </summary>
        public int B
        {
            get => _b;
            set => _b = value > 255 ? 255 : value;
        }

        /// <summary>
        ///     Comparator method.
        /// </summary>
        /// <param name="otherColor">BoardColor to compare.</param>
        /// <returns>True if equal, else false.</returns>
        public bool Equals(BoardColor otherColor)
        {
            return _r == otherColor.R && _b == otherColor.B && _g == otherColor.G;
        }

        /// <summary>
        ///     Clone the object of this class.
        /// </summary>
        /// <returns>Clone of object of this class.</returns>
        public BoardColor Clone()
        {
            return new BoardColor(_r, _g, _b);
        }
    }
}