/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 26/11/2021
**/

using ScreenSharing;
using NUnit.Framework;

namespace Testing.ScreenSharing
{
    internal class ScreenShareFactoryTesting
    {
        [Test]
        public void GetScreenSharer_MustReturnReferenceToSameObject()
        {
            var clientScreenSharer1 = ScreenShareFactory.GetScreenShareClient();
            var clientScreenSharer2 = ScreenShareFactory.GetScreenShareClient();

            var serverScreenSharer1 = ScreenShareFactory.GetScreenShareServer();
            var serverScreenSharer2 = ScreenShareFactory.GetScreenShareServer();

            Assert.That(ReferenceEquals(clientScreenSharer1, clientScreenSharer2));
            Assert.That(ReferenceEquals(serverScreenSharer1, serverScreenSharer2));
        }
    }
}