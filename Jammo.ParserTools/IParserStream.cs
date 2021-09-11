using System;

namespace Jammo.ParserTools
{
    public interface IParserStream : IDisposable
    {
        public void Parse();
        public void Write();
        public void WriteTo(string path);
    }
}