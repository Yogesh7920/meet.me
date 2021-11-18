/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 10/13/2021
**/

using System;

namespace Whiteboard
{
    public class Coordinate : IEquatable<Coordinate>
    {
        public int C;

        public int R;

        public Coordinate(int r, int c)
        {
            R = r;
            C = c;
        }

        public bool Equals(Coordinate otherCord)
        {
            if (R == otherCord.R && C == otherCord.C) return true;
            return false;
        }

        public Coordinate Clone()
        {
            return new Coordinate(R, C);
        }
    }
}