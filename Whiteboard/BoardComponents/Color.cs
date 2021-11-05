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
        public int R;
        public int G;
        public int B;

        public BoardColor(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public bool Equals(BoardColor otherColor)
        {
            if (this.R == otherColor.R && this.B == otherColor.B && this.G == otherColor.G)
            {
                return true;
            }
            return false;
        }

        public BoardColor Clone()
        {
            return new BoardColor(R, G, B);
        }
    }
}
