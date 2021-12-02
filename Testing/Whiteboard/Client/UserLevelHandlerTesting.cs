/**
 * Owned By: Chandan 
 * Created By: Chandan
 * Date Created: 25/11/2021
 * Date Modified: 25/11/2021
**/

using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    internal class UserLevelHandlerTesting
    {
        [Test]
        public void IsAccessible_Scenario1_ReturnsTrue()
        {
            var flag = UserLevelHandler.IsAccessible(1, 0);
            Assert.AreEqual(flag, true);
        }


        [Test]
        public void IsAccessible_Scenario2_ReturnsTrue()
        {
            var flag = UserLevelHandler.IsAccessible(1, 1);
            Assert.AreEqual(flag, true);
        }

        [Test]
        public void IsAccessible_Scenario3_ReturnsTrue()
        {
            var flag = UserLevelHandler.IsAccessible(0, 0);
            Assert.AreEqual(flag, true);
        }

        [Test]
        public void IsAccessible__Scenario4_ReturnsFalse()
        {
            var flag = UserLevelHandler.IsAccessible(0, 1);
            Assert.AreEqual(flag, false);
        }
    }
}