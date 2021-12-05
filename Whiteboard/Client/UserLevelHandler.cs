/**
 * Owned By: Chandan 
 * Created By: Chandan
 * Date Created: 25/11/2021
 * Date Modified: 25/11/2021
**/

namespace Whiteboard
{
    public static class UserLevelHandler
    {
        /// <summary>
        ///     To find out whether the user have the right to do the desired whiteboard operation
        /// </summary>
        /// <param name="UserLevelofUser"></param>
        /// <param name="UserLevelofShapeOwner"></param>
        /// <returns></returns>
        public static bool IsAccessible(int userLevelOfUser, int userLevelOfShapeOwner)
        {
            return userLevelOfUser >= userLevelOfShapeOwner;
        }
    }
}