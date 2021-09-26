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

        [Test]
        public void TestEnumeration()
        {
            var lexer = new Lexer(new Tokenizer("Some Text"));
            var count = lexer.Count();
            
            Assert.True(count == 3);
        }

        [Test]
        public void TestLeftParenthesisId()
        {
            var tokens = Lexer.Lex("(");
            
            Assert.True(tokens.First().Is(LexerTokenId.LeftParenthesis));
        }

        [Test]
        public void TestIdentifier()
        {
            var options = new LexerOptions
            {
                TokenizeIdentifiers = true,

                IdentifierStarts = new[]
                {
                    LexerTokenId.AlphaNumeric, LexerTokenId.Alphabetic, LexerTokenId.Underscore
                },

                IdentifierIds = new[]
                {
                    LexerTokenId.AlphaNumeric, LexerTokenId.Alphabetic, LexerTokenId.Numeric, LexerTokenId.Underscore
                }
            };

            var tokens = Lexer.Lex("_abc123=AAAAAAAAAAAAAa 123_=...", options);
            
            Assert.True(tokens.Count(t => t.Is(LexerTokenId.Identifier)) == 1);
        }
    }
}