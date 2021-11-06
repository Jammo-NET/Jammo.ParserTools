using System;

namespace Jammo.ParserTools.Lexing
{
    public class GenericLexerToken<TId> where TId : Enum
    {
        public readonly string RawToken;
        public readonly StringContext Context;
        public readonly TId Id;

        public GenericLexerToken(string rawToken, StringContext context, TId id)
        {
            RawToken = rawToken;
            Context = context;
            Id = id;
        }

        public bool Is(TId id) => Id.Equals(id);
    }
}