/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/13/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    class BoardPriorityQueueTesting
    {
        private BoardPriorityQueue _boardPriorityQueue;

        [SetUp]
        public void SetUp()
        {
            _boardPriorityQueue = new();
        }

        [TearDown]
        public void TearDown()
        {
            _boardPriorityQueue = null;
            GC.Collect();
        }

        [Test]
        public void Top_PriorityQueueEmpty_ReturnsNull()
        {
            // Act and Assert
            Assert.IsNull(_boardPriorityQueue.Top());
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void Top_PriorityQueueFilled_ReturnsLatest(int noOfInsertions)
        {
            // Arrange
            List<QueueElement> queueElements = InsertNRandomUniqueElements(noOfInsertions);

            // Act and Assert
            Assert.AreEqual(queueElements[noOfInsertions - 1], _boardPriorityQueue.Top());
            Assert.AreEqual(noOfInsertions, _boardPriorityQueue.GetSize());
            Assert.AreEqual(0, queueElements[noOfInsertions - 1].Index);
        }

        [Test]
        public void Insert_DecreasingOrder_SizeIncrease()
        {
            // Arrange
            QueueElement e1 = new("1", DateTime.Now.AddMinutes(2));
            QueueElement e2 = new("2", DateTime.Now);

            // Act
            _boardPriorityQueue.Insert(e1);
            _boardPriorityQueue.Insert(e2);

            // Assert
            Assert.AreEqual(2, _boardPriorityQueue.GetSize());
            Assert.AreEqual(e1, _boardPriorityQueue.Top());
            Assert.AreEqual(0, e1.Index);
            Assert.AreEqual(1, e2.Index);
        }

        [Test]
        public void Insert_IncreasingOrder_SizeIncrease()
        {
            // Arrange
            QueueElement e1 = new("1", DateTime.Now);
            QueueElement e2 = new("2", DateTime.Now.AddMinutes(2));

            // Act
            _boardPriorityQueue.Insert(e1);
            _boardPriorityQueue.Insert(e2);

            // Assert
            Assert.AreEqual(2, _boardPriorityQueue.GetSize());
            Assert.AreEqual(e2, _boardPriorityQueue.Top());
            Assert.AreEqual(0, e2.Index);
            Assert.AreEqual(1, e1.Index);
        }

        [Test]
        public void Insert_InsertList_SizeIncrease()
        {
            // Arrange
            List<QueueElement> list = new() { GetRandomQueueElement(10, "1", "second"), GetRandomQueueElement(10, "2", "second"), GetRandomQueueElement(10, "3", "second") };

            // Act
            _boardPriorityQueue.Insert(list);

            // Assert
            Assert.AreEqual(3, _boardPriorityQueue.GetSize());
        }

        [Test]
        public void Extract_EmptyQueue_ReturnsNull()
        {
            // Act and Assert
            Assert.IsNull(_boardPriorityQueue.Extract());
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(4, 3)]
        [TestCase(10, 9)]
        public void Extract_FilledQueue_ReturnsTop(int noOfInsertions, int expected)
        {
            // Arrange
            var queueElements = InsertNRandomUniqueElements(noOfInsertions);

            // Act and Assert
            Assert.AreEqual(queueElements[noOfInsertions - 1], _boardPriorityQueue.Extract());
            Assert.AreEqual(expected, _boardPriorityQueue.GetSize());
        }

        [Test]
        [TestCase(2, -1)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(0, 0)]
        public void IncreaseTimestamp_QueueElementIndexOutOfRange_ThrowsIndexOutOfRangeException(int noOfInsertions, int index)
        {
            // Arrange
            var list = InsertNRandomUniqueElements(noOfInsertions);
            QueueElement queueElement = new("1", DateTime.Now, index);

            // Act and Assert
            Assert.Throws<IndexOutOfRangeException>(() => _boardPriorityQueue.IncreaseTimestamp(queueElement, DateTime.Now));
        }

        [Test]
        public void IncreaseTimestamp_TimestampDecrease_ThrowsInvalidOperationException()
        {
            // Arrange
            var list = InsertNRandomUniqueElements(1);

            // Act
            Assert.Throws<InvalidOperationException>(() => _boardPriorityQueue.IncreaseTimestamp(list[0], list[0].Timestamp.AddSeconds(-100)));
        }

        [Test]
        public void IncreaseTimestamp_TimestampIncrease_DecreasesIndex()
        {
            // Arrange
            // q3 q1 q2
            DateTime dateTime = DateTime.Now;
            QueueElement q1 = new("1", dateTime);
            QueueElement q2 = new("2", dateTime.AddSeconds(10));
            QueueElement q3 = new("3", dateTime.AddSeconds(20));
            _boardPriorityQueue.Insert(new List<QueueElement>() { q1, q2, q3 });

            // Act`
            _boardPriorityQueue.IncreaseTimestamp(q1, dateTime.AddMinutes(1));

            // Assert
            Assert.AreEqual(3, _boardPriorityQueue.GetSize());
            Assert.AreEqual(0, q1.Index);
            Assert.AreEqual(1, q3.Index);
            Assert.AreEqual(2, q2.Index);
        }

        [Test]
        [TestCase(2, -1)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(0, 0)]
        public void DeleteElement_ElementIndexOutOfRange_ThrowsIndexOutOfRange(int noOfInsertions, int index)
        {
            // Arrange
            var list = InsertNRandomUniqueElements(noOfInsertions);
            QueueElement queueElement = new("1", DateTime.Now, index);

            // Act and Assert
            Assert.Throws<IndexOutOfRangeException>(() => _boardPriorityQueue.DeleteElement(queueElement));
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(5, 4)]
        public void DeleteElement_ValidIndex_SizeDecreaase(int noOfInsertions, int expected)
        {
            // Arrange
            List<QueueElement> list = InsertNRandomUniqueElements(noOfInsertions);

            // Act 
            _boardPriorityQueue.DeleteElement(list[0]);

            // Assert
            Assert.AreEqual(expected, _boardPriorityQueue.GetSize());
        }

        [Test]
        public void Clear_EmptyQueue_SizeZero()
        {
            // Act
            _boardPriorityQueue.Clear();

            // Assert
            Assert.AreEqual(0, _boardPriorityQueue.GetSize());
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void Clear_FilledQueue_SizeZero(int noOfInsertions)
        {
            // Arrange
            InsertNRandomUniqueElements(noOfInsertions);

            // Act
            _boardPriorityQueue.Clear();

            // Assert
            Assert.AreEqual(0, _boardPriorityQueue.GetSize());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GetSize_AllCases_ReturnsSize(int noOfInsertions)
        {
            // Arrange
            InsertNRandomUniqueElements(noOfInsertions);

            // Act and Assert
            Assert.AreEqual(noOfInsertions, _boardPriorityQueue.GetSize());
        }


        private static QueueElement GetRandomQueueElement(int randomVal, string id, string specifier)
        {
            if (specifier == "second")
            {
                return new(id, DateTime.Now.AddSeconds(new Random().Next(randomVal)));
            }
            else if (specifier == "minutes")
            {
                return new(id, DateTime.Now.AddMinutes(new Random().Next(randomVal)));
            }
            else
            {
                return null;
            }
        }

        private List<QueueElement> InsertNRandomUniqueElements(int n)
        {
            List<QueueElement> queueElements = new();
            for (int i = 0; i < n; i++)
            {
                QueueElement queueElement = GetRandomQueueElement(100, i.ToString(), "minutes");
                queueElements.Add(queueElement);
                _boardPriorityQueue.Insert(queueElement);
            }
            queueElements.Sort(delegate (QueueElement e1, QueueElement e2) { return e1.Timestamp.CompareTo(e2.Timestamp); });
            return queueElements;

        }
    }
}
