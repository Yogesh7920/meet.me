/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 10/28/2021
**/

using System;

namespace Whiteboard
{
    /// <summary>
    ///     Represent Board Coordinate with origin at Left-Bottom Corner.
    /// </summary>
    public class Coordinate : IEquatable<Coordinate>
    {
        /// <summary>
        ///     Column value of coordinate.
        /// </summary>
        public float C;

        /// <summary>
        ///     Row value of coordinate.
        /// </summary>
        public float R;

        /// <summary>
        ///     Constructor for Coordinate.
        /// </summary>
        /// <param name="r">Row value.</param>
        /// <param name="c">Column value.</param>
        public Coordinate(float r, float c)
        {
            R = r;
            C = c;
        }

        /// <summary>
        ///     Comparator method.
        /// </summary>
        /// <param name="otherCord">Coordinate to be compared with.</param>
        /// <returns>True if equal, else false.</returns>
        public bool Equals(Coordinate otherCord)
        {
            return Math.Abs(R - otherCord.R) < BoardConstants.AllowedDelta &&
                   Math.Abs(C - otherCord.C) < BoardConstants.AllowedDelta;
        }


        /// <summary>
        ///     Creates clone for object.
        /// </summary>
        /// <returns>Clone of object,</returns>
        public Coordinate Clone()
        {
            return new Coordinate(R, C);
        }

        /// <summary>
        ///     Comparator for Coordinate.
        /// </summary>
        /// <param name="coordinate">Coordinate to perform comparison with.</param>
        /// <returns>True if caller is less than parameter.</returns>
        public bool IsLessThan(Coordinate coordinate)
        {
            return R < coordinate.R && C < coordinate.C;
        }

        /// <summary>
        ///     Subtraction method for coordinate.
        /// </summary>
        /// <param name="coordinate">Coordinate to be subtracted.</param>
        public void Subtract(Coordinate coordinate)
        {
            R -= coordinate.R;
            C -= coordinate.C;
        }

        /// <summary>
        ///     Addition method for coordinate.
        /// </summary>
        /// <param name="coordinate">Coordinate to be added.</param>
        public void Add(Coordinate coordinate)
        {
            R += coordinate.R;
            C += coordinate.C;
        }

        /// <summary>
        ///     Overload method for + operator.
        /// </summary>
        /// <param name="x">First coordinate.</param>
        /// <param name="y">Second coordinate.</param>
        /// <returns>Result of Addition.</returns>
        public static Coordinate operator +(Coordinate x, Coordinate y)
        {
            return new Coordinate(x.R + y.R, x.C + y.C);
        }

        /// <summary>
        ///     Overload method for - operator.
        /// </summary>
        /// <param name="x">First coordinate.</param>
        /// <param name="y">Second coordinate.</param>
        /// <returns>Result of subtraction.</returns>
        public static Coordinate operator -(Coordinate x, Coordinate y)
        {
            return new Coordinate(x.R - y.R, x.C - y.C);
        }

        /// <summary>
        ///     Overload method for / operator.
        /// </summary>
        /// <param name="x">First coordinate.</param>
        /// <param name="y">Second coordinate.</param>
        /// <returns>Result of Division.</returns>
        public static Coordinate operator /(Coordinate x, int y)
        {
            if (y == 0) throw new Exception("Division of coordinate by 0.");
            return new Coordinate(x.R / y, x.C / y);
        }
    }
}