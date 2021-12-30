using System.Linq;
using Jammo.ParserTools.Tokenization;

namespace Jammo.ParserTools.Lexing
{
    public class LexerToken : GenericLexerToken<LexerTokenId>
    {
        public IndexSpan Span => Token.Span;
        
        public BasicToken Token { get; }

        public LexerToken(BasicToken token, LexerTokenId id) : base(token.ToString(), token.Context, id)
        {
            Token = token;
        }
        
        public LexerToken(BasicTokenCollection tokens, LexerTokenId id) : base(tokens.ToString(), tokens.First().Context, id)
        {
            Token = new BasicToken(
                tokens.ToString(),
                BasicTokenType.Conjunction,
                new IndexSpan(tokens.First().Span.Start, tokens.Last().Span.End),
                tokens.First().Context);
        }

        public override string ToString()
        {
            return RawToken;
        }
    }
}