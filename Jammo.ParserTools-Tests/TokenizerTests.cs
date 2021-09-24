using System;
using System.Linq;
using Jammo.ParserTools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    [TestFixture]
    public class TokenizerTests
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

        [Test]
        public void TestAlphaNumeric()
        {
            var testString = "abc123";
            var tokenizer = new Tokenizer(testString);
            
            Assert.True(tokenizer.Next().Type == BasicTokenType.Alphabetical);
        }

        [Test]
        public void TestAlphabetic()
        {
            var testString = "abc";
            var tokenizer = new Tokenizer(testString);
            
            Assert.True(tokenizer.Next().Type == BasicTokenType.Alphabetical);
        }

        [Test]
        public void TestNumeric()
        {
            var testString = "aaa 123";
            var tokenizer = Tokenizer.Tokenize(testString).ToArray();
            
            Assert.True(tokenizer[2].Type == BasicTokenType.Numerical);
        }
    }
}