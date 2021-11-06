namespace Jammo.ParserTools.Lexing
{
    public enum LexerTokenId
    {
        Unknown = 0,
        
        Alphabetic, AlphaNumeric, Numeric,
        
        Plus, Dash, Star, Equals, LessThan, GreaterThan,
        Slash, Backslash,
        
        Newline, Whitespace,
        
        LeftParenthesis, RightParenthesis,
        OpenBracket, CloseBracket,
        OpenCurlyBracket, CloseCurlyBracket,

        Tilde, Slave,
        Quote, DoubleQuote,
        Period, Comma, Colon, Semicolon,
        
        ExclamationMark, 
        At,
        Octothorpe,
        Dollar,
        Percent,
        Caret,
        Amphersand,
        Underscore,
        Vertical,
        QuestionMark
    }
}