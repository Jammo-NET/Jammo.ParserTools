using System;

namespace Jammo.ParserTools
{
    public interface IParserStream : IDisposable
    {
        public bool IsInitialized { get; }
        public string FilePath { get; }
    
        public void Parse();
        public void Write();
        public void WriteTo(string path);
    }
}