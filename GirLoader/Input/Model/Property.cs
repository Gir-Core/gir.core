using System.Xml.Serialization;

namespace GirLoader.Input.Model
{
    public class Property : AnyType
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("writable")]
        public bool Writeable { get; set; }

        [XmlAttribute("readable")]
        public bool Readable { get; set; } = true;

        [XmlAttribute("construct-only")]
        public bool ConstructOnly { get; set; }

        [XmlAttribute("construct")]
        public bool Construct { get; set; }

        [XmlAttribute("transfer-ownership")]
        public string? TransferOwnership { get; set; }

        [XmlAttribute("deprecated")]
        public bool Deprecated { get; set; }

        [XmlAttribute("deprecated-version")]
        public string? DeprecatedVersion { get; set; }

        [XmlElement("doc")]
        public Doc? Doc { get; set; }

        [XmlElement("doc-deprecated")]
        public Doc? DocDeprecated { get; set; }

        [XmlElement("type")]
        public Type? Type { get; set; }

        [XmlElement("array")]
        public ArrayType? Array { get; set; }
    }
}
