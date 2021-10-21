using System;
using System.Linq;
using Jammo.ParserTools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    [TestFixture]
    public class EnumerableIteratorTests
    {
        private static readonly int[] TestArray = { 0, 1, 2, 3 };
        private EnumerableIterator<int> iterator;

        [SetUp]
        public void SetUp()
        {
            iterator = TestArray.ToIterator();
        }
        
        [Test]
        public void TestNext()
        {
            iterator.TryMoveNext(out var result);
            
            Assert.True(result == 0);
        }

        [Test]
        public void TextPrevious()
        {
            iterator.TryMoveNext(out _);
            iterator.TryMoveNext(out _);
            iterator.TryGetPrevious(out var previous);
            
            Assert.True(previous == 0);
        }

        [Test]
        public void TestAtEnd()
        {
            foreach (var unused in TestArray)
                iterator.TryMoveNext(out _);
            
            iterator.TryMoveNext(out _);
            
            Assert.True(iterator.AtEnd);
        }
        
        [Test]
        public void TestSkip()
        {
            iterator.Skip(4);
            
            Assert.True(iterator.Current == 3);
        }

        [Test]
        public void TestSkipWhile()
        {
            iterator.SkipWhile(i => i < 3);
            
            Assert.True(iterator.Current == 3);
        }

        [Test]
        public void TestTake()
        {
            Assert.True(iterator.Take(4).Sum() == 6);    
        }

        [Test]
        public void TestTakeWhile()
        {
            Assert.True(iterator.TakeWhile(i => i < 4).Sum() == 6);
        }
    }
}