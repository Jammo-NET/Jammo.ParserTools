using System.Linq;
using Jammo.ParserTools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    public class Tests
    {
        [Test]
        public void TestTokenizer()
        {
            var tokenizer = new Tokenizer("Some Text");
            
            Assert.True(tokenizer.Next().Text == "Some");
        }

        [Test]
        public void TestTokenGrouper()
        {
            var nestedText = "My Grouped Text";
            var testString = $"({nestedText})";

            var groups = new Tokenizer(testString).Group();
            
            Assert.True(groups.First().ToString() == nestedText);
        }
    }
}