using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DllProject.Messages
{
    [XmlRoot(ElementName = "SolutionRequest", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolutionRequestMessage : Message
    {
        public ulong Id { get; set; }

        public SolutionRequestMessage() 
        {
        
        }

        public SolutionRequestMessage(ulong id)
        {
            Id = id;
        }


    }
}
