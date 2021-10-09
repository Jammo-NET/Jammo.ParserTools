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

        public void Reset()
        {
            Index = 0;
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
            BasicToken token;
            while ((token = PeekNext()) != null)
            {
                if (!predicate.Invoke(token))
                    break;

                Next();
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

            var trimmed = string.Concat(text.Skip(Index));
            var first = trimmed.First();
            var currentRead = string.Empty;

            if (char.IsPunctuation(first))
            {
                return new BasicToken(first.ToString(),
                    BasicTokenType.Punctuation, new IndexSpan(Index, Index + 1));
            }
            
            if (char.IsSymbol(first))
            {
                return new BasicToken(first.ToString(),
                    BasicTokenType.Symbol, new IndexSpan(Index, Index + 1));
            }

            if (char.IsWhiteSpace(first))
            {
                foreach (var character in trimmed)
                {
                    if (!char.IsWhiteSpace(character))
                        break;
                    
                    currentRead += character;
                    
                    if (currentRead == Environment.NewLine)
                        return new BasicToken(
                            currentRead,
                            BasicTokenType.Newline,
                            new IndexSpan(Index, Index + currentRead.Length));
                }
                
                return new BasicToken(
                    currentRead,
                    BasicTokenType.Whitespace, 
                    new IndexSpan(Index, Index + currentRead.Length));
            } 
            
            if (char.IsLetter(first))
            {
                foreach (var character in trimmed)
                {
                    if (char.IsLetter(character))
                        currentRead += character;
                    else if (char.IsNumber(character) && !options.SeparateAlphanumerical)
                        currentRead += character;
                    else
                        break;
                }
                
                return new BasicToken(
                    currentRead,
                    BasicTokenType.Alphabetical, 
                    new IndexSpan(Index, Index + currentRead.Length));
            }
            
            if (char.IsNumber(first))
            {
                currentRead += string.Concat(trimmed.TakeWhile(char.IsNumber));

                return new BasicToken(
                    currentRead,
                    BasicTokenType.Numerical, 
                    new IndexSpan(Index, Index + currentRead.Length));
            }

            return new BasicToken(first.ToString(), BasicTokenType.Unhandled, new IndexSpan(Index, Index + 1));
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
        public bool SeparateAlphanumerical;
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