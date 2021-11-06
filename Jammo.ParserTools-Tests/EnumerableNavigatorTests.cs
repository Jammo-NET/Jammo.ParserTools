using System;
using System.Linq;
using Jammo.ParserTools;
using Jammo.ParserTools.Tools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    [TestFixture]
    public class EnumerableNavigatorTests
    {
        private static readonly int[] TestArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly EnumerableNavigator<int> iterator = TestArray.ToNavigator();

        [SetUp]
        public void SetUp()
        {
            iterator.Reset();
        }

        [Test]
        public void TestNext()
        {
            iterator.TryMoveNext(out var next);
            
            Assert.True(next == 0);
        }

        [Test]
        public void TestLast()
        {
            iterator.TryMoveNext(out _);
            iterator.TryMoveNext(out _);
            iterator.TryMoveLast(out var last);
            
            Assert.True(last == 0);
        }

        [Test]
        public void TestSkip()
        {
            iterator.Skip(5);
            
            Assert.True(iterator.Current == 4);
        }

        [Test]
        public void TestSkipWhile()
        {
            iterator.SkipWhile(i => i < 9);
            
            Assert.True(iterator.Current == 9);
        }

        [Test]
        public void TestTake()
        {
            Assert.True(iterator.Take(10).Sum() == 45);    
        }
        
        [Test]
        public void TestTakeIf()
        {
            iterator.TakeIf(i => i == 0, out var match);
            
            Assert.True(match == 0);
        }

        [Test]
        public void TestTakeWhile()
        {
            Assert.True(iterator.TakeWhile(i => i < 10).Sum() == 45);
        }

        [Test]
        public void TestEnumerateOnce()
        {
            Assert.True(iterator.EnumerateOnce().Sum() == 45);
        }
        
        [Test]
        public void TestInvalidIndex()
        {
            Assert.Catch<InvalidOperationException>(delegate { var unused = iterator.Current; });
        }
    }
}