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
        private readonly EnumerableNavigator<int> navigator = TestArray.ToNavigator();

        [SetUp]
        public void SetUp()
        {
            navigator.Reset();
        }

        [Test]
        public void TestNext()
        {
            navigator.TryMoveNext(out var next);
            
            Assert.True(next == 0);
        }

        [Test]
        public void TestLast()
        {
            navigator.TryMoveNext(out _);
            navigator.TryMoveNext(out _);
            navigator.TryMoveLast(out var last);
            
            Assert.True(last == 0);
        }

        [Test]
        public void TestSkip()
        {
            navigator.Skip(5);
            
            Assert.True(navigator.Current == 4);
        }

        [Test]
        public void TestSkipWhile()
        {
            navigator.SkipWhile(i => i < 5);
            var first = navigator.EnumerateFromIndex().First();
            
            Assert.True(first == 5);
        }
        
        [Test]
        public void TestStartingSkipWhile()
        {
            var first = navigator.EnumerateFromIndex().First();
            
            Assert.True(first == 0);
        }

        [Test]
        public void TestTake()
        {
            Assert.True(navigator.Take(10).Sum() == 45);    
        }
        
        [Test]
        public void TestTakeIf()
        {
            navigator.TakeIf(i => i == 0, out var match);
            
            Assert.True(match == 0);
        }

        [Test]
        public void TestTakeWhile()
        {
            Assert.True(navigator.TakeWhile(i => i < 10).Sum() == 45);
        }

        [Test]
        public void TestEnumerateOnce()
        {
            Assert.True(navigator.EnumerateOnce().Sum() == 45);
        }
        
        [Test]
        public void TestInvalidIndex()
        {
            Assert.Catch<InvalidOperationException>(delegate { var unused = navigator.Current; });
        }
    }
}