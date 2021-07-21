using System.Xml.Serialization;

namespace GirLoader.Input.Model
{
    public class ReturnValue : AnyType
    {
        [XmlAttribute("transfer-ownership")]
        public string? TransferOwnership { get; set; }

        [XmlAttribute("nullable")]
        public bool Nullable { get; set; }

        [XmlElement("doc")]
        public Doc? Doc { get; set; }

        [XmlElement("type")]
        public Type? Type { get; set; }

        [XmlElement("array")]
        public ArrayType? Array { get; set; }
    }
}
