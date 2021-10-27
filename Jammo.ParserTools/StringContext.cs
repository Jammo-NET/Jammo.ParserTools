namespace Jammo.ParserTools
{
    public readonly struct StringContext
    {
        public readonly int Column;
        public readonly int Line;

        public StringContext(int column, int line)
        {
            Column = column;
            Line = line;
        }

        public StringContext MoveColumn(int count = 1) => new(Column + count, Line);
        public StringContext MoveLine(int count = 1) => new(0, Line + count);
    }
}