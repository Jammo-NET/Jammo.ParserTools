using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammo.ParserTools.Tokenization
{
    public class Tokenizer : IEnumerable<BasicToken>
    {
        private readonly string text;
        private readonly TokenizerOptions options;
        private StringContext context;

        public Tokenizer(string input, TokenizerOptions? options = null)
        {
            text = input;
            this.options = options ?? new TokenizerOptions();
        }

        public static IEnumerable<BasicToken> Tokenize(string input, TokenizerOptions? options = null)
        {
            return new Tokenizer(input, options);
        }

        public IEnumerator<BasicToken> GetEnumerator()
        {
            context = new StringContext(0, 0);

            foreach (var line in text.Split(Environment.NewLine))
            {
                var index = 0;
                
                BasicToken? token;
                while ((token = GetNext(line, index)) != null)
                {
                    context = context.MoveColumn(token.Span.Size);

                    index += token.Span.Size;

                    yield return token;
                }
                
                context = context.MoveLine();
            }
        }

        private BasicToken? GetNext(string partial, int index)
        {
            if (index == partial.Length)
                return null;

            var trimmed = string.Concat(partial.Skip(index));
            var first = trimmed.First();
            var currentRead = string.Empty;

            if (char.IsPunctuation(first))
            {
                return new BasicToken(first.ToString(),
                    BasicTokenType.Punctuation, new IndexSpan(index, index + 1), context);
            }
            
            if (char.IsSymbol(first))
            {
                return new BasicToken(first.ToString(),
                    BasicTokenType.Symbol, new IndexSpan(index, index + 1), context);
            }

            if (char.IsWhiteSpace(first))
            {
                foreach (var character in trimmed)
                {
                    if (!char.IsWhiteSpace(character))
                        break;
                    
                    currentRead += character;
                }
                
                return new BasicToken(
                    currentRead,
                    BasicTokenType.Whitespace, 
                    new IndexSpan(index, index + currentRead.Length), context);
            } 
            
            if (char.IsLetter(first))
            {
                currentRead += string.Concat(trimmed.TakeWhile(c => char.IsLetter(c) || char.IsNumber(c)));
                
                return new BasicToken(
                    currentRead,
                    BasicTokenType.Alphabetical, 
                    new IndexSpan(index, index + currentRead.Length), context);
            }
            
            if (char.IsNumber(first))
            {
                currentRead += string.Concat(trimmed.TakeWhile(char.IsNumber));

                return new BasicToken(
                    currentRead,
                    BasicTokenType.Numerical, 
                    new IndexSpan(index, index + currentRead.Length), context);
            }

            return new BasicToken(
                first.ToString(), 
                BasicTokenType.Unhandled, 
                new IndexSpan(index, index + 1), 
                context);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TokenizerOptions
    {
        
    }

    public enum BasicTokenType
    {
        Unhandled = 0,

        Alphabetical,
        Numerical,
        Symbol,
        Punctuation,
        Whitespace,
        Newline,
        
        Conjunction,
    }
}