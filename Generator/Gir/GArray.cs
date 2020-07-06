using System.Xml.Serialization;

namespace Gir
{
    public class GArray
    {
        [XmlAttribute("length")]
        public string? Length { get; set; }

        [XmlAttribute("zero-terminated")]
        public bool ZeroTerminated { get; set; }

        [XmlAttribute("type", Namespace="http://www.gtk.org/introspection/c/1.0")]
        public string? CType { get; set; }

        [XmlElement("type")]
        public GType? Type { get; set; }
    }
}
