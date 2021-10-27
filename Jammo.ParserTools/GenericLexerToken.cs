using System;

namespace Jammo.ParserTools
{
    public class GenericLexerToken<TId> where TId : Enum
    {
        public readonly string Text;
        public readonly StringContext Context;
        public readonly TId Id;

        public GenericLexerToken(string text, StringContext context, TId id)
        {
            Text = text;
            Context = context;
            Id = id;
        }

        public bool Is(TId id) => Id.Equals(id);
    }
}