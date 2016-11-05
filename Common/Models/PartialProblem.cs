
using System.Xml.Serialization;

namespace Common.Models
{
    [XmlRoot(ElementName = "PartialProblem")]
    public class PartialProblem
    {
        public ulong TaskId { get; set; }
        public byte[] Data { get; set; }
    }
}
