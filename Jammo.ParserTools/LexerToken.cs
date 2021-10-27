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
}