using Content;
using NUnit.Framework;

namespace Testing.Content
{
    public class ContentServerFactoryTests
    {
        private IContentServer contentServer;

        [SetUp]
        public void Setup()
        {
            contentServer = ContentServerFactory.GetInstance();
        }

        [Test]
        public void GetInstanceTest()
        {
            Assert.AreEqual(contentServer, ContentServerFactory.GetInstance());
        }
    }
}