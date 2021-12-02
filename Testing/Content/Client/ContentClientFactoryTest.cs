using System.Threading;
using Content;
using NUnit.Framework;

namespace Testing.Content
{
    [TestFixture]
    internal class ContentClientFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetInstance_MustReturnSingletonInstance()
        {
            var ref1 = ContentClientFactory.GetInstance();
            var ref2 = ContentClientFactory.GetInstance();

            Assert.That(ReferenceEquals(ref1, ref2));
        }

        [Test]
        public void GetInstance_MustReturnSingletonInstanceMultithreaded()
        {
            IContentClient ref1 = null;
            IContentClient ref2 = null;

            // make two separate threads and run (almost) simultaneously and check if the same reference is returned
            var process1 = new Thread(() => { ref1 = ContentClientFactory.GetInstance(); });

            var process2 = new Thread(() => { ref2 = ContentClientFactory.GetInstance(); });

            process1.Start();
            process2.Start();

            process1.Join();
            process2.Join();

            Assert.That(ReferenceEquals(ref1, ref2));
        }

        [Test]
        public void SetUser_MustSetUserIdAcrossAllReturnedInstances()
        {
            var ref1 = ContentClientFactory.GetInstance();
            var ref2 = ContentClientFactory.GetInstance();

            Assert.That(ReferenceEquals(ref1, ref2));

            var userId1 = ref1.GetUserId();
            var userId2 = ref2.GetUserId();
            Assert.AreEqual(userId1, userId2);
        }
    }
}