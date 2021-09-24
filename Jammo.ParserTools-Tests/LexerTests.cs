using System.Linq;
using Jammo.ParserTools;
using NUnit.Framework;

namespace Jammo.ParserTools_Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void TestAlphaNumericId()
        {
            var tokens = Lexer.Lex("abc123");
            
            Assert.True(tokens.First().Is(LexerTokenId.AlphaNumeric));
        }

        [Test]
        public void TestQuoteId()
        {
            var tokens = Lexer.Lex("'");
            
            Assert.True(tokens.First().Is(LexerTokenId.Quote));
        }
    }
}