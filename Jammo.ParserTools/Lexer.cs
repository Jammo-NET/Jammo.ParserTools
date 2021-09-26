using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools
{
    public class Lexer : IEnumerable<LexerToken>
    {
        private LexerOptions options;
        
        public Tokenizer Tokenizer { get; }
        
        public Lexer(string text)
        {
            Tokenizer = new Tokenizer(text);
            options = new LexerOptions();
        }
        
        public Lexer(string text, LexerOptions lexerOptions)
        {
            Tokenizer = new Tokenizer(text, null);
            options = lexerOptions ?? new LexerOptions();
        }

        public Lexer(string text, TokenizerOptions tokenizerOptions, LexerOptions lexerOptions = null)
        {
            Tokenizer = new Tokenizer(text, tokenizerOptions);
            options = lexerOptions ?? new LexerOptions();
        }
        
        public Lexer(Tokenizer tokenizer)
        {
            Tokenizer = tokenizer;
            options =  new LexerOptions();
        }
        
        public Lexer(Tokenizer tokenizer, LexerOptions options)
        {
            Tokenizer = tokenizer;
            this.options = options ?? new LexerOptions();
        }
        
        public static IEnumerable<LexerToken> Lex(string input)
        {
            return new Lexer(new Tokenizer(input));
        }

        public static IEnumerable<LexerToken> Lex(string input, LexerOptions lexerOptions = null)
        {
            return new Lexer(new Tokenizer(input), lexerOptions);
        }
        
        public static IEnumerable<LexerToken> Lex(
            string input, TokenizerOptions tokenizerOptions = null, LexerOptions lexerOptions = null)
        {
            return new Lexer(new Tokenizer(input, tokenizerOptions), lexerOptions);
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
            var token = Tokenizer.Next();

            return token == null ? null : new LexerToken(token, IdFromBasicToken(token));
        }

        public LexerToken PeekNext()
        {
            var token = Tokenizer.PeekNext();

            return token == null ? null : new LexerToken(token, IdFromBasicToken(token));
        }

        public static LexerTokenId IdFromBasicToken(BasicToken token)
        {
            return token.Type switch
            {
                BasicTokenType.Alphabetical when char.IsNumber(token.Text.Last()) => LexerTokenId.AlphaNumeric,
                BasicTokenType.Alphabetical => LexerTokenId.Alphabetic,
                BasicTokenType.Numerical => LexerTokenId.Numeric,
                BasicTokenType.Whitespace => LexerTokenId.Space,
                BasicTokenType.Newline => LexerTokenId.NewLine,
                
                _ => token.Text switch
                {
                    "=" => LexerTokenId.Equals,
                    "+" => LexerTokenId.Plus,
                    "-" => LexerTokenId.Minus,
                    "*" => LexerTokenId.Star,
                    "<" => LexerTokenId.LessThan,
                    ">" => LexerTokenId.GreaterThan,
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
                    "|" => LexerTokenId.Vertical,
                    "_" => LexerTokenId.Underscore,
                    "." => LexerTokenId.Period,
                    "," => LexerTokenId.Comma,
                    ":" => LexerTokenId.Colon,
                    ";" => LexerTokenId.Semicolon,
                    "'" => LexerTokenId.Quote,
                    "\"" => LexerTokenId.DoubleQuote,
                    "?" => LexerTokenId.QuestionMark,
                    _ => LexerTokenId.Unknown
                }
            };
        }

        public IEnumerator<LexerToken> GetEnumerator()
        {
            LexerToken token;
            while ((token = Next()) != null)
            {
                if (options.TokenizeIdentifiers)
                {
                    if (options.IdentifierStarts.Contains(token.Id))
                    {
                        var identifierTokens = new BasicTokenCollection();
                        
                        while ((token = Next()) != null)
                        {
                            if (options.IdentifierIds.Contains(token.Id))
                                identifierTokens.Add(token.Token);
                        }

                        yield return new LexerToken(identifierTokens.ToString(), LexerTokenId.Identifier);
                    }
                }
                else
                {
                    yield return token;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class LexerOptions
    {
        public bool TokenizeIdentifiers;
        public IEnumerable<LexerTokenId> IdentifierIds;
        public IEnumerable<LexerTokenId> IdentifierStarts;
    }

    public class LexerToken
    {
        public readonly string RawToken;
        public readonly BasicToken Token;
        public readonly LexerTokenId Id;

        public LexerToken(BasicToken token, LexerTokenId id)
        {
            RawToken = token.ToString();
            Token = token;
            Id = id;
        }
        
        public LexerToken(string raw, LexerTokenId id)
        {
            RawToken = raw;
            Id = id;
        }

        public bool Is(LexerTokenId id) => Id == id;

        public override string ToString()
        {
            return Token.ToString();
        }
    }

    public enum LexerTokenId
    {
        Unknown = 0,
        
        Identifier,
        Alphabetic, AlphaNumeric, Numeric,
        
        Plus, Minus, Star, Equals, LessThan, GreaterThan,
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
        Underscore,
        Vertical,
        QuestionMark
    }
}