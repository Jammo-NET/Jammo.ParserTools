using System;
using System.Linq;
using Jammo.ParserTools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    [TestFixture]
    public class EnumerableNavigatorTests
    {
        private readonly int[] array = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private EnumerableNavigator<int> navigator;

        [SetUp]
        public void SetUp()
        {
            navigator = array.ToNavigator();
        }

        [Test]
        public void TestNext()
        {
            navigator.TryMoveNext(out var next);
            
            Assert.True(next == 1);
        }

        [Test]
        public void TestLast()
        {
            navigator.TryMoveNext(out _);
            navigator.TryMoveLast(out var last);
            
            Assert.True(last == 0);
        }

        [Test]
        public void TestSkip()
        {
            navigator.Skip(5);
            
            Assert.True(navigator.Current == 5);
        }

        [Test]
        public void TestSkipWhile()
        {
            navigator.SkipWhile(i => i < 9);
            
            Assert.True(navigator.Current == 9);
        }

        [Test]
        public void TestTake()
        {
            Assert.True(navigator.Take(9).Sum() == 45);    
        }
        
        [Test]
        public void TestTakeIf()
        {
            navigator.TakeIf(i => i == 1, out var match);
            
            Assert.True(match == 1);
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
    }
}