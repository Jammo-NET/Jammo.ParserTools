using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools
{
    public class Lexer : IEnumerable<LexerToken>
    {
        private Tokenizer tokenizer;
        
        public Lexer(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public static IEnumerable<LexerToken> Lex(string input, TokenizerOptions options = null)
        {
            return new Lexer(new Tokenizer(input, options));
        }

        public void Skip(int count = 1)
        {
            for (var c = 0; c < count; c++)
            {
                if (Next() == null)
                    break;
            }
        }

        public void SkipWhile(Func<LexerToken, bool> predicate)
        {
            foreach (var token in this)
            {
                if (!predicate.Invoke(token))
                    break;
            }
        }

        public LexerToken Next()
        {
            return PeekNext();
        }

        public LexerToken PeekNext()
        {
            var token = tokenizer.PeekNext();

            return token == null ? null : new LexerToken(token, GetIdFromToken(token));
        }

        private LexerTokenId GetIdFromToken(BasicToken token)
        {
            if (token.Type == BasicTokenType.Alphabetical)
            {
                if (char.IsNumber(token.Text.Last()))
                    return LexerTokenId.AlphaNumeric;

                return LexerTokenId.Alphabetic;
            }

            if (token.Type == BasicTokenType.Numerical)
                return LexerTokenId.Numeric;
            
            if (token.Type == BasicTokenType.Symbol)
            {
                return token.Text switch
                {
                    "+" => LexerTokenId.Plus,
                    "-" => LexerTokenId.Minus,
                    "*" => LexerTokenId.Star,
                    "(" => LexerTokenId.LeftParenthesis,
                    ")" => LexerTokenId.RightParenthesis,
                    "[" => LexerTokenId.OpenBracket,
                    "]" => LexerTokenId.CloseBracket,
                    "{" => LexerTokenId.OpenCurlyBracket,
                    "}" => LexerTokenId.CloseCurlyBracket,
                    "/" => LexerTokenId.Slash,
                    "\\" => LexerTokenId.Backslash,
                    "~" => LexerTokenId.Tilde,
                    "`" => LexerTokenId.Slave,
                    "!" => LexerTokenId.Exclamation,
                    "@" => LexerTokenId.At,
                    "#" => LexerTokenId.Octothorpe,
                    "$" => LexerTokenId.Dollar,
                    "%" => LexerTokenId.Percent,
                    "^" => LexerTokenId.Caret,
                    "&" => LexerTokenId.Amphersand,
                    "|" => LexerTokenId.Horizontal,
                    _ => LexerTokenId.Unknown
                };
            }
            
            if (token.Type == BasicTokenType.Punctuation)
            {
                return token.Text switch
                {
                    "." => LexerTokenId.Period,
                    "," => LexerTokenId.Comma,
                    ":" => LexerTokenId.Colon,
                    ";" => LexerTokenId.Semicolon,
                    "'" => LexerTokenId.Quote,
                    "\"" => LexerTokenId.DoubleQuote,
                    _ => LexerTokenId.Unknown
                };
            }

            if (token.Type == BasicTokenType.Whitespace)
                return LexerTokenId.Space;
            
            if (token.Type == BasicTokenType.Newline)
                return LexerTokenId.NewLine;

            return LexerTokenId.Unknown;
        }

        public IEnumerator<LexerToken> GetEnumerator()
        {
            LexerToken token;
            while ((token = Next()) != null)
                yield return token;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class LexerToken
    {
        public readonly BasicToken Token;
        public readonly LexerTokenId Id;

        public LexerToken(BasicToken token, LexerTokenId id)
        {
            Token = token;
            Id = id;
        }

        public bool Is(LexerTokenId id) => Id == id;
    }

    public enum LexerTokenId
    {
        Unknown = 0,
        
        Alphabetic, AlphaNumeric, Numeric,
        
        Plus, Minus, Star,
        Slash, Backslash,
        
        NewLine, Space,
        
        LeftParenthesis, RightParenthesis,
        OpenBracket, CloseBracket,
        OpenCurlyBracket, CloseCurlyBracket,

        Tilde, Slave,
        Quote, DoubleQuote,
        Period, Comma, Colon, Semicolon,
        
        Exclamation, 
        At,
        Octothorpe,
        Dollar,
        Percent,
        Caret,
        Amphersand,
        Horizontal,
    }
}