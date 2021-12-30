using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jammo.ParserTools.Tokenization;
using Jammo.ParserTools.Tools;

namespace Jammo.ParserTools.Lexing
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
            string input, TokenizerOptions tokenizerOptions, LexerOptions? lexerOptions = null)
        {
            return new Lexer(new Tokenizer(input, tokenizerOptions), lexerOptions);
        }

        public IEnumerator<LexerToken> GetEnumerator()
        {
            var navigator = Tokenizer.ToNavigator();

            foreach (var _ in navigator.EnumerateFromIndex())
            {
                var token = TransformToken(navigator);
                
                if (options.IgnorePredicate.Invoke(token))
                    continue;

                yield return token;
            }
        }

        private LexerToken TransformToken(EnumerableNavigator<BasicToken> navigator)
        {
            var token = navigator.Current;
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
                    
                    while (navigator.TryPeekNext(out var peekToken))
                    {
                        switch (peekToken.Type)
                        {
                            case BasicTokenType.Alphabetical:
                            case BasicTokenType.Numerical:
                            case BasicTokenType.Punctuation when peekToken.Text == "_" && options.IncludeUnderscoreAsAlphabetic:
                            {
                                relevantTokens.Add(peekToken);
                                
                                break;
                            }
                            default:
                            {
                                if (relevantTokens.ToString().Any(char.IsNumber))
                                    return new LexerToken(relevantTokens, LexerTokenId.AlphaNumeric);
                                
                                return new LexerToken(relevantTokens, LexerTokenId.Alphabetic);
                            }
                        }

                        navigator.Skip();
                    }
                    
                    if (relevantTokens.ToString().Any(char.IsNumber))
                        return new LexerToken(relevantTokens, LexerTokenId.AlphaNumeric);
                    
                    return new LexerToken(relevantTokens, LexerTokenId.Alphabetic);
                }
                case BasicTokenType.Numerical:
                {
                    while (navigator.TryPeekNext(out var peekToken))
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
                                
                                return new LexerToken(relevantTokens, LexerTokenId.Numeric);
                            }
                            default:
                                return new LexerToken(relevantTokens, LexerTokenId.Numeric);
                        }
                        
                        navigator.Skip();
                    }

                    return new LexerToken(relevantTokens, LexerTokenId.Numeric);
                }
                case BasicTokenType.Symbol:
                {
                    return new LexerToken(token, SymbolIdFromBasicToken(token));
                }
                case BasicTokenType.Newline:
                {
                    return new LexerToken(token, LexerTokenId.Newline);
                }
                case BasicTokenType.Whitespace:
                {
                    while (navigator.TryPeekNext(out var peekToken))
                    {
                        if (peekToken.Type is not BasicTokenType.Whitespace or BasicTokenType.Newline)
                            break;

                        navigator.Skip();
                        
                        relevantTokens.Add(peekToken);
                    }
                    
                    return new LexerToken(relevantTokens, LexerTokenId.Whitespace);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class LexerOptions
    {
        internal readonly Func<LexerToken, bool> IgnorePredicate;

        public bool IncludeUnderscoreAsAlphabetic;
        public bool IncludePeriodAsNumeric;

        public LexerOptions(Func<LexerToken, bool> ignorePredicate = null)
        {
            IgnorePredicate = ignorePredicate ?? delegate { return false; };
        }
    }
}