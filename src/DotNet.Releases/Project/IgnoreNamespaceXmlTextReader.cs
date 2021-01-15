using System.IO;
using System.Xml;

namespace DotNet.Versions
{
    internal class IgnoreNamespaceXmlTextReader : XmlTextReader
    {
        public override string NamespaceURI => string.Empty;

        public IgnoreNamespaceXmlTextReader(TextReader reader) : base(reader)
        {
            Namespaces = false;
        }
    }
}
