using System;
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
            
            Assert.True(lexer.Count() == 3);
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
            var options = new LexerOptions { IncludeUnderscoreAsAlphabetic = true };
            var tokens = Lexer.Lex("_abc123=AAAAAAAAAAAAAa 123_=...", options);
            
            Assert.True(tokens.First().RawToken == "_abc123");
        }


        [Test]
        public void TestDecimal()
        {
            var options = new LexerOptions { IncludePeriodAsNumeric = true };
            var tokens = Lexer.Lex("12.34", options);
            
            Assert.True(tokens.First().RawToken == "12.34");
        }
    }
}