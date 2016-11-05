using Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common.Models.Messages
{
    [XmlRoot(ElementName = "SolveRequestResponse", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolveRequestResponseMessage : Message
    {
        public ulong Id { get; set; }

        public SolveRequestResponseMessage()
        { 
        
        }

        public SolveRequestResponseMessage(ulong id)
        {
            Id = id;
        }


    }
}
