/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 10/13/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Converts a non=serialisable shape to send to serialised shape to send to server and vice-versa.
    /// </summary>
    public static class Convertor
    {
        /// <summary>
        /// Converts BoardShape to BoardServerShape.
        /// </summary>
        /// <param name="boardShape"> BoardShape Object. </param>
        /// <returns> Serialisable BoardServerObject. </returns>
        public static BoardServerShape ToBoardServerShape(BoardShape boardShape)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts BoardServerShape to BoardShape.
        /// </summary>
        /// <param name="boardServerShape"> BoardServerShape Object. </param>
        /// <returns> BoardShape Object. </returns>
        public static BoardShape ToBoardShape(BoardServerShape boardServerShape)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update an existing boardShape using another BoardServerShape.
        /// </summary>
        /// <param name="boardShape"> BoardShape Object to be modified. </param>
        /// <param name="updater"> BoardServerShape Object to be used for modification. </param>
        /// <returns> Modified BoardShape Object. </returns>
        public static BoardShape UpdateBoardShape(BoardShape boardShape, BoardServerShape updater)
        {
            throw new NotImplementedException();
        }
    }
}
