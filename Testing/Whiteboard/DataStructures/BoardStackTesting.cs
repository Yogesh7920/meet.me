/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/13/2021
 * Date Modified: 11/13/2021
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
    class BoardStackTesting
    {
        private BoardStack _boardStack;
        private int _capacity;

        [SetUp]
        public void SetUp()
        {
            _capacity = BoardConstants.UNDO_REDO_STACK_SIZE;
            _boardStack = new(_capacity);
        }

        [TearDown]
        public void TearDown()
        {
            _boardStack = null;
            GC.Collect();
        }

        [Test]
        public void IsEmpty_EmptyStack_ReturnsTrue()
        {
            // Act and Assert
            Assert.IsTrue(_boardStack.IsEmpty());
            Assert.AreEqual(0, _boardStack.GetSize());
        }

        [Test]
        [TestCase(1, 1)]
        public void IsEmpty_FilledStack_ReturnsFalse(int noOfInsertions, int expected)
        {
            // Arrange
            PushNUniqueElements(noOfInsertions);

            // Act and Assert
            Assert.IsFalse(_boardStack.IsEmpty());
            Assert.AreEqual(expected, _boardStack.GetSize());
        }

        [Test]
        public void Clear_EmptyStack_SizeZero()
        {
            // Act
            _boardStack.Clear();

            // Assert
            Assert.AreEqual(0, _boardStack.GetSize());
        }

        [Test]
        [TestCase(2)]
        public void Clear_FilledStack_SizeZero(int noOfInsertions)
        {
            // Arrange
            PushNUniqueElements(noOfInsertions);

            // Act
            _boardStack.Clear();

            // Assert
            Assert.AreEqual(0, _boardStack.GetSize());
        }

        [Test]
        public void Pop_EmptyStack_ThrowIndexOutOfRangeException()
        {
            // Act and Assert
            Assert.Throws<IndexOutOfRangeException>(() => _boardStack.Pop());
        }

        [Test]
        public void Pop_FilledStack_SizeDecrease()
        {
            // Arrange
            List<Tuple<string, string>> listOfIds = PushNUniqueElements(2);

            // Act and Assert
            _boardStack.Pop();

            Assert.AreEqual(listOfIds[0].Item1, _boardStack.Top().Item1.Uid);
            Assert.AreEqual(listOfIds[0].Item2, _boardStack.Top().Item2.Uid);
            Assert.AreEqual(1, _boardStack.GetSize());

            _boardStack.Pop();
            Assert.AreEqual(0, _boardStack.GetSize());
        }

        [Test]
        public void Top_EmptyStack_ThrowIndexOutOfRangeException()
        {
            // Act and Assert
            Assert.Throws<IndexOutOfRangeException>(() => _boardStack.Top());
        }

        [Test]
        [TestCase(2)]
        public void Top_FilledStack_TopElement(int noOfInsertions)
        {
            // Arrange
            List<Tuple<string, string>> listOfIds = PushNUniqueElements(noOfInsertions);

            // Act and Assert
            Assert.AreEqual(listOfIds[noOfInsertions - 1].Item1, _boardStack.Top().Item1.Uid);
            Assert.AreEqual(listOfIds[noOfInsertions - 1].Item2, _boardStack.Top().Item2.Uid);
        }

        [Test]
        [TestCase(2)]
        public void Push_SizeIncrease(int noOfInsertions)
        {
            // Act
            PushNUniqueElements(noOfInsertions);

            // Assert
            Assert.AreEqual(noOfInsertions, _boardStack.GetSize());
        }

        [Test]
        [TestCase(BoardConstants.UNDO_REDO_STACK_SIZE + 5)]
        [TestCase(BoardConstants.UNDO_REDO_STACK_SIZE + 10)]
        public void Push_StackFull_SizeConstant(int noOfInsertions)
        {
            // Act
            List<Tuple<string, string>> listOfIds = PushNUniqueElements(noOfInsertions);

            // Assert
            Assert.AreEqual(_capacity, _boardStack.GetSize());

            for (int i = noOfInsertions - 1; i >= noOfInsertions - _capacity; i--)
            {
                Assert.AreEqual(listOfIds[i].Item1, _boardStack.Top().Item1.Uid);
                Assert.AreEqual(listOfIds[i].Item2, _boardStack.Top().Item2.Uid);
                _boardStack.Pop();
            }

            Assert.AreEqual(0, _boardStack.GetSize());
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(5, 5)]
        [TestCase(BoardConstants.UNDO_REDO_STACK_SIZE, BoardConstants.UNDO_REDO_STACK_SIZE)]
        [TestCase(BoardConstants.UNDO_REDO_STACK_SIZE + 5, BoardConstants.UNDO_REDO_STACK_SIZE)]
        public void GetSize_AllCases_SizeCheck(int noOfInsertions, int expected)
        {
            // Arrange
            PushNUniqueElements(noOfInsertions);

            // Act and assert
            Assert.AreEqual(expected, _boardStack.GetSize());
        }

        private List<Tuple<string, string>> PushNUniqueElements(int n)
        {
            List<Tuple<string, string>> listOfIds = new();
            for (int i = 0; i < n; i++)
            {
                BoardShape first = new();
                BoardShape second = new();

                string firstId = i.ToString() + "_first";
                string secondId = i.ToString() + "_second";
                first.Uid = firstId;
                second.Uid = secondId;

                _boardStack.Push(first, second);
                listOfIds.Add(new(firstId, secondId));
            }
            return listOfIds;
        }
    }
}
