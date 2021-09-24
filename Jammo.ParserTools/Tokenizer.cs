using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Jammo.ParserTools
{
    public class Tokenizer : IEnumerable<BasicToken>
    {
        private readonly string text;
        private readonly TokenizerOptions options;

        public int Index { get; private set; }

        public Tokenizer(string input, TokenizerOptions options = null)
        {
            text = input;
            this.options = options ?? new TokenizerOptions();
        }

        public static IEnumerable<BasicToken> Tokenize(string input, TokenizerOptions options = null)
        {
            return new Tokenizer(input, options);
        }
        
        public IEnumerable<BasicTokenGroup> Group()
        {
            var groups = new List<BasicTokenGroup>();

            BasicToken currentToken;
            while ((currentToken = Next()) != null)
            {
                BasicTokenGroup group = new TextGroup();
                string closingText = null;

                switch (currentToken.Text)
                {
                    case "(":
                        group = new ParenthesisGroup();
                        closingText = ")";
                        break;
                    case "[":
                        group = new BracketGroup();
                        closingText = "]";
                        break;
                    case "{":
                        group = new CurlyBracketGroup();
                        closingText = "}";
                        break;
                    case "<":
                        group = new AngleBracketGroup();
                        closingText = ">";
                        break;
                    case "\"":
                        group = new DoubleQuoteGroup();
                        closingText = "\"";
                        break;
                    case "\'":
                        group = new SingleQuoteGroup();
                        closingText = "\'";
                        break;
                    default:
                        group.Add(currentToken);
                        break;
                }

                if (closingText == null)
                    continue;
            
                while ((currentToken = Next()) != null)
                {
                    if (currentToken.Text == closingText)
                        break;
                        
                    group.Add(currentToken);
                }
            
                groups.Add(group);
            }

            return groups;
        }

        public void Skip(int count = 1)
        {
            for (var c = 0; c < count; c++)
            {
                if (Next() == null)
                    break;
            }
        }

        public void SkipWhile(Func<BasicToken, bool> predicate)
        {
            foreach (var token in this)
            {
                if (!predicate.Invoke(token))
                    break;
            }
        }

        public BasicToken Next()
        {
            var token = PeekNext();

            Index += token?.Text.Length ?? 0;
            
            return token;
        }

        public BasicToken PeekNext()
        {
            if (text.Length == Index)
                return null;
            
            var currentRead = string.Empty;
            var currentTokenType = BasicTokenType.Unhandled;

            for (var charIndex = Index; charIndex < text.Length; charIndex++)
            {
                var character = text[charIndex];

                if (char.IsWhiteSpace(character) &&
                    (char.IsWhiteSpace(currentRead.LastOrDefault()) || string.IsNullOrEmpty(currentRead)))
                {
                    currentTokenType = BasicTokenType.Whitespace;
                    currentRead += character;

                    if (currentRead == Environment.NewLine)
                        return new BasicToken(
                            currentRead,
                            BasicTokenType.Newline,
                            new IndexSpan(Index, charIndex));
                }
                else if (char.IsPunctuation(character) && string.IsNullOrEmpty(currentRead))
                {
                    return new BasicToken(
                        character.ToString(),
                        BasicTokenType.Punctuation,
                        new IndexSpan(Index, charIndex));
                }
                else if (char.IsSymbol(character) && string.IsNullOrEmpty(currentRead))
                {
                    return new BasicToken(
                        character.ToString(),
                        BasicTokenType.Symbol,
                        new IndexSpan(Index, charIndex));
                }
                else if (char.IsLetter(character) &&
                    (char.IsLetter(currentRead.LastOrDefault()) || string.IsNullOrEmpty(currentRead)))
                {
                    currentTokenType = BasicTokenType.Alphabetical;
                    currentRead += character;
                }
                else if (char.IsNumber(character) &&
                         (char.IsLetter(currentRead.LastOrDefault()) || 
                         char.IsNumber(currentRead.LastOrDefault()) ||
                         string.IsNullOrEmpty(currentRead)) &&
                         !char.IsPunctuation(character))
                { // Allow abc123
                    if (char.IsLetter(currentRead.LastOrDefault()) ||
                        char.IsNumber(currentRead.LastOrDefault()) && currentTokenType == BasicTokenType.Alphabetical)
                        currentTokenType = BasicTokenType.Alphabetical;
                    else
                        currentTokenType = BasicTokenType.Numerical;
                    
                    currentRead += character;
                }
                else
                {
                    if (options.Ignorable.Contains(currentTokenType))
                    {
                        Index += currentRead.Last();
                        
                        return PeekNext();
                    }

                    return new BasicToken(currentRead, currentTokenType, new IndexSpan(Index, charIndex));
                }
            }

            if (string.IsNullOrEmpty(currentRead))
                return null;
            
            if (options.Ignorable.Contains(currentTokenType))
            {
                Index += currentRead.Last();
                        
                return PeekNext();
            }
            
            return new BasicToken(currentRead, currentTokenType, new IndexSpan(Index, text.Length - 1));
        }

        public IEnumerator<BasicToken> GetEnumerator()
        {
            BasicToken token;
            while ((token = Next()) != null)
                yield return token;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    public class BasicTokenCollection : Collection<BasicToken>
    {
        public override string ToString()
        {
            return string.Concat(this.Select(token => token.Text));
        }
    }

    public class BasicToken
    {
        public readonly string Text;
        public readonly BasicTokenType Type;
        public readonly IndexSpan Span;

        public BasicToken(string text, BasicTokenType type, IndexSpan span)
        {
            Text = text;
            Type = type;
            Span = span;
        }

        public override string ToString()
        {
            return Text;
        }
    }
    
    public class TokenizerOptions
    {
        public readonly BasicTokenType[] Ignorable;

        public TokenizerOptions(params BasicTokenType[] ignorable)
        {
            Ignorable = ignorable;
        }
    }

    public enum BasicTokenType
    {
        Unhandled = 0,
        
        Alphabetical,
        Numerical,
        Symbol,
        Punctuation,
        Whitespace,
        Newline
    }
}