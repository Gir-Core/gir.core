using System.Collections.Generic;
using System.Xml.Serialization;

namespace Generator.Introspection
{
    public class RecordInfo : ClassInfo
    {
        [XmlAttribute("is-gtype-struct-for", Namespace = "http://www.gtk.org/introspection/glib/1.0")]
        public string? GLibIsGTypeStructFor;

        [XmlAttribute("disguised")]
        public bool Disguised;

        [XmlAttribute("introspectable")]
        public bool Introspectable = true;
    }
}