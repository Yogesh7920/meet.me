using ScreenSharing;
using NUnit.Framework;

namespace Testing.ScreenSharing
{
    [TestFixture]
    internal class ScreenShareFactoryTesting
    {
        [Test]
        public void GetScreenSharer_MustReturnReferenceToSameObject()
        {
            ScreenShareClient comm1 = ScreenShareFactory.GetScreenSharer();
            ScreenShareClient comm2 = ScreenShareFactory.GetScreenSharer();

            Assert.That(ReferenceEquals(comm1, comm2));
        }
    }
}