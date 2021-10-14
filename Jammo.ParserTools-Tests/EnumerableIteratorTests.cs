using Jammo.ParserTools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    [TestFixture]
    public class EnumerableIteratorTests
    {
        private static readonly int[] TestArray = { 0, 1 };
        private readonly EnumerableIterator<int> iterator = TestArray.ToIterator();

        [SetUp]
        public void SetUp()
        {
            iterator.Reset();
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
    }
}