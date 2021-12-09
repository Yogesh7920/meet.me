/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/13/2021
 * Date Modified: 11/13/2021
**/

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    internal class BoardStackTesting
    {
        [SetUp]
        public void SetUp()
        {
            _capacity = BoardConstants.UndoRedoStackSize;
            _boardStack = new BoardStack(_capacity);
        }

        [TearDown]
        public void TearDown()
        {
            _boardStack = null;
            GC.Collect();
        }

        private BoardStack _boardStack;
        private int _capacity;

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
            var listOfIds = PushNUniqueElements(2);

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
            var listOfIds = PushNUniqueElements(noOfInsertions);

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
        [TestCase(BoardConstants.UndoRedoStackSize + 5)]
        [TestCase(BoardConstants.UndoRedoStackSize + 10)]
        public void Push_StackFull_SizeConstant(int noOfInsertions)
        {
            // Act
            var listOfIds = PushNUniqueElements(noOfInsertions);

            // Assert
            Assert.AreEqual(_capacity, _boardStack.GetSize());

            for (var i = noOfInsertions - 1; i >= noOfInsertions - _capacity; i--)
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
        [TestCase(BoardConstants.UndoRedoStackSize, BoardConstants.UndoRedoStackSize)]
        [TestCase(BoardConstants.UndoRedoStackSize + 5, BoardConstants.UndoRedoStackSize)]
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
            for (var i = 0; i < n; i++)
            {
                BoardShape first = new();
                BoardShape second = new();

                var firstId = i + "_first";
                var secondId = i + "_second";
                first.Uid = firstId;
                second.Uid = secondId;

                _boardStack.Push(first, second);
                listOfIds.Add(new Tuple<string, string>(firstId, secondId));
            }

            return listOfIds;
        }
    }
}