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
    public class Coordinate : IEquatable<Coordinate>
    {

        public int R;
        public int C;

        public Coordinate(int r, int c)
        {
            this.R = r;
            this.C = c;
        }

        public bool Equals( Coordinate otherCord)
        { 
            if (this.R== otherCord.R && this.C == otherCord.C)
            {
                return true;
            }
            return false;
        }

        public Coordinate Clone()
        {
            return new Coordinate(R,C);
        }

        public bool IsLessThan(Coordinate coordinate)
        {
            return (R < coordinate.R && C < coordinate.C) ? true : false;
        }

        public void Subtract(Coordinate coordinate)
        {
            R -= coordinate.R;
            C -= coordinate.C;
        }

        public void Add(Coordinate coordinate)
        {
            R += coordinate.R;
            C += coordinate.C;
        }

        public static Coordinate operator + (Coordinate x, Coordinate y)
        {
            return new Coordinate(x.R + y.R, x.C + y.C);
        }

        public static Coordinate operator -(Coordinate x, Coordinate y)
        {
            return new Coordinate(x.R - y.R, x.C - y.C);
        }

        public static Coordinate operator /(Coordinate x, int y)
        {
            return new Coordinate(x.R/2, x.C/2);
        }
    }
}
