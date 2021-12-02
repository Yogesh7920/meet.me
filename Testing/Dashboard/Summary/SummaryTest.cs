/// <author>Sairoop Bodepudi</author>
/// <created>19/11/2021</created>
/// <summary>
///		This is the unit testing file
///		for the summary logic module and
///		tests the summarizer functions.
/// </summary>

using System.IO;
using Dashboard.Server.Summary;
using NUnit.Framework;

namespace Testing.Dashboard.Summary
{
	/// <summary>
	///     The test class for the Summary logic module
	/// </summary>
	public class SummaryTest
    {
        private ISummarizer _summarizer;

        /// <summary>
        ///     Setup the summarizer from the Summarizer factory
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _summarizer = SummarizerFactory.GetSummarizer();
        }

        /// <summary>
        ///     Null chat should give empty string
        /// </summary>
        [Test]
        public void GetSummary_NullChatContext_EmptyString()
        {
            var chats = Utils.GetChatContext("Null Context");
            Assert.AreEqual(_summarizer.GetSummary(chats), "");
        }

        /// <summary>
        ///     Empty chat context should give empty string
        /// </summary>
        [Test]
        public void GetSummary_EmptyChatContext_EmptyString()
        {
            var chats = Utils.GetChatContext("Empty chat context");
            Assert.AreEqual(_summarizer.GetSummary(chats), "");
        }

        /// <summary>
        ///     Empty chat context should give empty string
        /// </summary>
        [Test]
        public void GetSummary_EmptyChats_EmptyString()
        {
            var chats = Utils.GetChatContext("Empty chats");
            Assert.AreEqual(_summarizer.GetSummary(chats), "");
        }

        /// <summary>
        ///     We only check on constant string since it is a probabilistic algorithm to give a string of constants.
        /// </summary>
        [Test]
        public void GetSummary_ValidChatsFixedLength_ConstString()
        {
            var chats = Utils.GetChatContext("Fixed chat");
            var output = "CONST. CONST. CONST. CONST. CONST. CONST. CONST. CONST. CONST. CONST";
            Assert.AreEqual(_summarizer.GetSummary(chats), output);
        }

        /// <summary>
        ///     On the general string we can only check fi the output summary is not null
        /// </summary>
        [Test]
        public void GetSummary_ValidChatsSmall_NonEmptyString()
        {
            var chats = Utils.GetChatContext("Variable chat");
            Assert.IsTrue(_summarizer.GetSummary(chats).Length > 0);
        }

        /// <summary>
        ///     On the general string we can only check fi the output summary is not null
        /// </summary>
        [Test]
        public void GetSummary_ValidChatsGeneral_NonEmptyString()
        {
            var chats = Utils.GetChatContext("General chat");
            Assert.IsTrue(_summarizer.GetSummary(chats).Length > 0);
        }

        /// <summary>
        ///     Test the save summary function on a general chat
        /// </summary>
        [Test]
        public void SaveSummary_ValidChatsGeneral_ReturnsTrue()
        {
            var chats = Utils.GetChatContext("General chat");
            Assert.AreEqual(_summarizer.SaveSummary(chats), true);
            Directory.Delete("../../../Persistence", true);
        }
    }
}