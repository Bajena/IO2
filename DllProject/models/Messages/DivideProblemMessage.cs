using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DllProject.Messages
{
    [XmlRoot(ElementName = "DivideProblem", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class DivideProblemMessage : Message
    {
        public string ProblemType { get; set; }
        public ulong Id { get; set; }
        public byte[] Data {get;set;} 
        public ulong ComputationalNodes { get; set; }

        public DivideProblemMessage() 
        { 
            
        }
        
        public DivideProblemMessage(string problemType,ulong id, byte[] data, 
            ulong computationalNodes)
        {
            ProblemType = problemType;
            Id = id;

            Data = new byte[data.Length];
            Array.Copy(data, Data,data.Length); 
            
            ComputationalNodes = computationalNodes;
        }

    }
}