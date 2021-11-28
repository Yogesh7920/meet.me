/// <author>Sameer Dhiman</author>
/// <created>7/11/2021</created>
/// <summary>
///     This file contains tests for ContentServerFactory
/// </summary>

using System.Threading;
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
        public void GetInstance_MustReturnSingletonInstance()
        {
            var ref1 = ContentServerFactory.GetInstance();
            var ref2 = ContentServerFactory.GetInstance();

            Assert.That(ReferenceEquals(ref1, ref2));
        }

        [Test]
        public void GetInstance_MustReturnSingletonInstanceMultithreaded()
        {
            IContentServer ref1 = null;
            IContentServer ref2 = null;

            // make two separate threads and run (almost) simultaneously and check if the same reference is returned
            var process1 = new Thread(() => { ref1 = ContentServerFactory.GetInstance(); });

            var process2 = new Thread(() => { ref2 = ContentServerFactory.GetInstance(); });

            process1.Start();
            process2.Start();

            process1.Join();
            process2.Join();

            Assert.That(ReferenceEquals(ref1, ref2));
        }
    }
}