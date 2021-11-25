/**
 * Owned By: Chandan 
 * Created By: Chandan
 * Date Created: 25/11/2021
 * Date Modified: 25/11/2021
**/

using System;


namespace Whiteboard
{   
    
    public static class UserLevelHandler
    {   
        /// <summary>
        /// to find out whether the user have the right to do the desired whiteboard operation
        /// </summary>
        /// <param name="UserLevelofUser"></param>
        /// <param name="UserLevelofShapeOwner"></param>
        /// <returns></returns>
        public static bool is_accessible(int UserLevelofUser, int UserLevelofShapeOwner)
        {
            return (UserLevelofUser >= UserLevelofShapeOwner) ;
            
        }
    }
}
