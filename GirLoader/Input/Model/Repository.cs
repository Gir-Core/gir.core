﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace GirLoader.Input.Model
{
    [XmlRoot(ElementName = "repository")]
    public class Repository
    {
        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlElement("namespace")]
        public Namespace? Namespace { get; set; }

        [XmlElement("include")]
        public List<Include> Includes { get; set; } = default!;
    }
}
