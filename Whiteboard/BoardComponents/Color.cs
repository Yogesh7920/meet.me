/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 10/13/2021
**/

using System;

namespace Whiteboard
{
    public class BoardColor : IEquatable<BoardColor>
    {
        public int B;
        public int G;
        public int R;

        public BoardColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public bool Equals(BoardColor otherColor)
        {
            if (R == otherColor.R && B == otherColor.B && G == otherColor.G) return true;
            return false;
        }

        public BoardColor Clone()
        {
            return new BoardColor(R, G, B);
        }
    }
}