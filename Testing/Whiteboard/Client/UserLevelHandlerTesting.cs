/**
 * Owned By: Chandan 
 * Created By: Chandan
 * Date Created: 25/11/2021
 * Date Modified: 25/11/2021
**/

using System;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{

    class UserLevelHandlerTesting
    {
        [Test]
        public void IsAccessible_Scenario1_ReturnsTrue()
        {
            bool flag= UserLevelHandler.IsAccessible(1, 0);
            Assert.AreEqual(flag, true);
            
        }


        [Test]
        public void IsAccessible_Scenario2_ReturnsTrue()
        {
            bool flag = UserLevelHandler.IsAccessible(1, 1);
            Assert.AreEqual(flag, true);

        }
        [Test]
        public void IsAccessible_Scenario3_ReturnsTrue()
        {
            bool flag = UserLevelHandler.IsAccessible(0, 0);
            Assert.AreEqual(flag, true);

        }

        [Test]
        public void IsAccessible__Scenario4_ReturnsFalse()
        {
            bool flag = UserLevelHandler.IsAccessible(0, 1);
            Assert.AreEqual(flag, false);

        }


    }

}
