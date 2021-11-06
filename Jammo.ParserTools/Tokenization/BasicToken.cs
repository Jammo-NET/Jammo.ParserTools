namespace Jammo.ParserTools.Tokenization
{
    public class BasicToken
    {
        public readonly string Text;
        public readonly BasicTokenType Type;
        public readonly IndexSpan Span;
        public readonly StringContext Context;

        public BasicToken(string text, BasicTokenType type, IndexSpan span, StringContext context)
        {
            Text = text;
            Type = type;
            Span = span;
            Context = context;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}