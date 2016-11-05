using Common.Abstractions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common.Models.Messages
{
    [XmlRoot(ElementName = "Status", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class StatusMessage : Message
    {    
        public ulong Id { get; set; }

        [XmlArray("Threads"), XmlArrayItem(typeof(IOThread), ElementName = "Thread")]
        public List<IOThread> Threads { get; set; }

        public StatusMessage()
        { 
        
        }

        public StatusMessage(ulong id, List <IOThread> threads = null)
        {
            Id = id;
            Threads = threads ?? new List<IOThread>();
        }


    }
}
