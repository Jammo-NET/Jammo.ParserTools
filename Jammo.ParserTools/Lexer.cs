using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jammo.ParserTools
{
    public class Lexer : IEnumerable<LexerToken>
    {
        private readonly LexerOptions options;

        public Tokenizer Tokenizer { get; }
        
        public Lexer(string text)
        {
            Tokenizer = new Tokenizer(text);
            options = new LexerOptions();
        }
        
        public Lexer(string text, LexerOptions lexerOptions)
        {
            Tokenizer = new Tokenizer(text);
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
            options = new LexerOptions();
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

        public static IEnumerable<LexerToken> Lex(string input, LexerOptions lexerOptions)
        {
            return new Lexer(new Tokenizer(input), lexerOptions);
        }
        
        public static IEnumerable<LexerToken> Lex(
            string input, TokenizerOptions tokenizerOptions, LexerOptions lexerOptions = null)
        {
            return new Lexer(new Tokenizer(input, tokenizerOptions), lexerOptions);
        }

        public void Reset()
        {
            Tokenizer.Reset();
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
            LexerToken token;
            while ((token = PeekNext()) != null)
            {
                if (!predicate.Invoke(token))
                    break;

                Next();
            }
        }

        public LexerToken Next()
        {
            return PeekNext();
        }

        public LexerToken PeekNext()
        {
            var token = Tokenizer.Next();
            
            if (token == null)
                return null;
            
            var relevantTokens = new BasicTokenCollection { token };
            
            switch (token.Type)
            {
                case BasicTokenType.Alphabetical:
                case BasicTokenType.Punctuation:
                {
                    if (token.Type is BasicTokenType.Punctuation)
                    {
                        if (token.Text == "_" && !options.IncludeUnderscoreAsAlphabetic)
                            return new LexerToken(token, LexerTokenId.Underscore);

                        if (token.Text != "_")
                            return new LexerToken(token, SymbolIdFromBasicToken(token));
                    }

                    BasicToken peekToken;
                    while ((peekToken = Tokenizer.PeekNext()) != null)
                    {
                        switch (peekToken.Type)
                        {
                            case BasicTokenType.Alphabetical:
                            case BasicTokenType.Numerical:
                            case BasicTokenType.Symbol when peekToken.Text == "_" && options.IncludeUnderscoreAsAlphabetic:
                            {
                                relevantTokens.Add(peekToken);

                                break;
                            }
                            default:
                            {
                                if (relevantTokens.ToString().Any(char.IsNumber))
                                    return new LexerToken(relevantTokens.ToString(), LexerTokenId.AlphaNumeric);
                                
                                return new LexerToken(relevantTokens.ToString(), LexerTokenId.Alphabetic);
                            }
                        }

                        Tokenizer.Next();
                    }
                    
                    if (relevantTokens.ToString().Any(char.IsNumber))
                        return new LexerToken(relevantTokens.ToString(), LexerTokenId.AlphaNumeric);
                    
                    return new LexerToken(token, LexerTokenId.Alphabetic);
                }
                case BasicTokenType.Numerical:
                {
                    BasicToken peekToken;
                    while ((peekToken = Tokenizer.PeekNext()) != null)
                    {
                        switch (peekToken.Type)
                        {
                            case BasicTokenType.Numerical:
                            {
                                relevantTokens.Add(peekToken);

                                break;
                            }
                            case BasicTokenType.Punctuation:
                            {
                                if (peekToken.Text == "." && options.IncludePeriodAsNumeric)
                                {
                                    relevantTokens.Add(peekToken);

                                    break;
                                }
                                
                                return new LexerToken(relevantTokens.ToString(), LexerTokenId.Numeric);
                            }
                            default:
                                return new LexerToken(relevantTokens.ToString(), LexerTokenId.Numeric);
                        }

                        Tokenizer.Next();
                    }

                    return new LexerToken(relevantTokens.ToString(), LexerTokenId.Numeric);
                }
                case BasicTokenType.Symbol:
                {
                    return new LexerToken(token, SymbolIdFromBasicToken(token));
                }
                case BasicTokenType.Newline:
                case BasicTokenType.Whitespace:
                {
                    BasicToken peekToken;
                    while ((peekToken = Tokenizer.PeekNext()) != null)
                    {
                        if (peekToken.Type is not BasicTokenType.Whitespace or BasicTokenType.Newline)
                            break;

                        Tokenizer.Next();
                        
                        relevantTokens.Add(peekToken);
                    }
                    
                    return new LexerToken(relevantTokens.ToString(), LexerTokenId.Space);
                }
                case BasicTokenType.Unhandled:
                {
                    return new LexerToken(token, LexerTokenId.Unknown);
                }
            }

            return new LexerToken(token, SymbolIdFromBasicToken(token));
        }

        public static LexerTokenId SymbolIdFromBasicToken(BasicToken token)
        {
            return token.Text switch
            {
                "=" => LexerTokenId.Equals,
                "+" => LexerTokenId.Plus,
                "-" => LexerTokenId.Dash,
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
                "!" => LexerTokenId.ExclamationMark,
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
            };
        }

        public IEnumerator<LexerToken> GetEnumerator()
        {
            LexerToken token;
            while ((token = Next()) != null)
            {
                yield return token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class LexerOptions
    {
        public bool IncludeUnderscoreAsAlphabetic;
        public bool IncludePeriodAsNumeric;
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
            return RawToken;
        }
    }

    public enum LexerTokenId
    {
        Unknown = 0,
        
        Alphabetic, AlphaNumeric, Numeric,
        
        Plus, Dash, Star, Equals, LessThan, GreaterThan,
        Slash, Backslash,
        
        NewLine, Space,
        
        LeftParenthesis, RightParenthesis,
        OpenBracket, CloseBracket,
        OpenCurlyBracket, CloseCurlyBracket,

        Tilde, Slave,
        Quote, DoubleQuote,
        Period, Comma, Colon, Semicolon,
        
        ExclamationMark, 
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