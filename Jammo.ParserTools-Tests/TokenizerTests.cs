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
            
            Assert.True(tokenizer.First().Text == "Some");
        }

        [Test]
        public void TestAlphaNumeric()
        {
            var testString = "abc123";
            var tokenizer = new Tokenizer(testString);
            
            Assert.True(tokenizer.First().Type == BasicTokenType.Alphabetical);
        }

        [Test]
        public void TestAlphabetic()
        {
            var testString = "abc";
            var tokenizer = new Tokenizer(testString);
            
            Assert.True(tokenizer.First().Type == BasicTokenType.Alphabetical);
        }

        [Test]
        public void TestNumeric()
        {
            var testString = "aaa 123 bbb 456";
            var tokenizer = Tokenizer.Tokenize(testString);
            
            Assert.True(tokenizer.ElementAt(2).Type == BasicTokenType.Numerical);
        }

        [Test]
        public void TestContext()
        {
            var testString = $"HELLO {Environment.NewLine} WORLD";
            var tokenizer = Tokenizer.Tokenize(testString);
            
            Assert.True(tokenizer.ElementAt(2).Context.Line == 1);
        }
    }
}