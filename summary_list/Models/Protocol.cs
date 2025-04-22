using System.Collections.Generic;
using System.Xml.Serialization;

namespace summary_list.Models
{
    [XmlRoot("Protocols")]
    public class Protocols
    {
        [XmlElement("Protocol")]
        public List<Protocol> ProtocolList { get; set; }
    }

    public class Protocol
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Group")]
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Item")]
        public List<SummaryItem> Items { get; set; }
    }
} 