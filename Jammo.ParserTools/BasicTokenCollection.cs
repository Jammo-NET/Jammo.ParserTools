using System.Collections.ObjectModel;
using System.Linq;

namespace Jammo.ParserTools
{
    public class BasicTokenCollection : Collection<BasicToken>
    {
        public override string ToString()
        {
            return string.Concat(this.Select(token => token.Text));
        }
    }
}