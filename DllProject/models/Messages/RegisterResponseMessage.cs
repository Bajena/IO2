using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DllProject.Messages
{
    [XmlRoot(ElementName = "RegisterResponse", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class RegisterResponseMessage : Message
    {
        public ulong Id { get; set; }

        [XmlElement(DataType = "time")]
        public DateTime Timeout { get; set; }

        public RegisterResponseMessage()
        { 
        
        }

        public RegisterResponseMessage(ulong id, DateTime timeout)
        {
            Id = id;
            Timeout = timeout; // DateTime is a struct
        }


    }
}
