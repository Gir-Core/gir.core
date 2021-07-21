using System.Xml.Serialization;

namespace GirLoader.Input.Model
{
    public class Parameter : AnyType
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("transfer-ownership")]
        public string? TransferOwnership { get; set; }

        [XmlAttribute("direction")]
        public string? Direction { get; set; }

        [XmlAttribute("caller-allocates")]
        public bool CallerAllocates;

        [XmlElement("doc")]
        public Doc? Doc { get; set; }

        [XmlElement("type")]
        public Type? Type { get; set; }

        [XmlElement("array")]
        public ArrayType? Array { get; set; }

        [XmlElement("varargs")]
        public VarArgs? VarArgs { get; set; }

        [XmlAttribute("nullable")]
        public bool Nullable;

        [XmlAttribute("closure")]
        public int Closure = -1;

        [XmlAttribute("destroy")]
        public int Destroy = -1;

        [XmlAttribute("scope")]
        public string? Scope { get; set; }
    }
}
