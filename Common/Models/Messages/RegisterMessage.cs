using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Common.Abstractions;
using Common.Enumerations;

namespace Common.Models.Messages
{
    [XmlRoot(ElementName = "Register", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class RegisterMessage : Message
    {
        [XmlElement(ElementName = "Type")]
        public EComponentType ComponentType { get; set; }

        [XmlArray("SolvableProblems"), XmlArrayItem(typeof(string), ElementName = "ProblemName")]
        public List<string> SolvableProblems { get; set; } 

        public int ParallelThreads { get; set; }

        public RegisterMessage()
        { 
        
        }

        public RegisterMessage(EComponentType componentType, int paralellThreads = 1, List<string> solvableProblems = null)
        {
            ComponentType = componentType;
            ParallelThreads = paralellThreads;
            SolvableProblems = solvableProblems ?? new List<string>();
        }

    }
}
