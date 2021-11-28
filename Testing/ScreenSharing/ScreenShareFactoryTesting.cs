using ScreenSharing;
using NUnit.Framework;

namespace Testing.ScreenSharing
{
    internal class ScreenShareFactoryTesting
    {
        [Test]
        public void GetScreenSharer_MustReturnReferenceToSameObject()
        {
            var screenSharer1 = ScreenShareFactory.GetScreenSharer();
            var screenSharer2 = ScreenShareFactory.GetScreenSharer();

            Assert.That(ReferenceEquals(screenSharer1, screenSharer2));
        }
    }
}