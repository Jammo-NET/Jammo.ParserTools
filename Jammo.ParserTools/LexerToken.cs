using System.Linq;

namespace Jammo.ParserTools
{
    public class LexerToken
    {
        public StringContext Context => Token.Context;
        public IndexSpan Span => Token.Span;
        
        public readonly string RawToken;
        public readonly BasicToken Token;
        public readonly LexerTokenId Id;

        public LexerToken(BasicToken token, LexerTokenId id)
        {
            
            RawToken = token.ToString();
            Token = token;
            Id = id;
        }
        
        public LexerToken(BasicTokenCollection tokens, LexerTokenId id)
        {
            Token = new BasicToken(
                tokens.ToString(),
                BasicTokenType.Unhandled,
                new IndexSpan(tokens.First().Span.Start, tokens.Last().Span.End),
                tokens.First().Context);
            
            RawToken = Token.ToString();
            Id = id;
        }

        public bool Is(LexerTokenId id) => Id == id;

        public override string ToString()
        {
            return RawToken;
        }
    }
}