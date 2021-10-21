/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 10/13/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    public class Color : IEquatable<Color>
    {
        public int R;
        public int G;
        public int B;

        public Color(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public bool Equals(Color otherColor)
        {
            if (this.R == otherColor.R && this.B == otherColor.B && this.G == otherColor.G)
            {
                return true;
            }
            return false;
        }
    }
}
