using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace DllProject.Messages
{
    [XmlRoot(ElementName = "SolvePartialProblems", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolvePartialProblemsMessage: Message
    {

        public string ProblemType { get; set; }
        public ulong Id { get; set; }
        public byte[] CommonData { get; set; } 
        public ulong SolvingTimeout { get; set; }

        [XmlArray("PartialProblems"), XmlArrayItem(typeof(PartialProblem), ElementName = "PartialProblem")]
        public List<PartialProblem> PartialProblems { get; set; }

        public SolvePartialProblemsMessage()
        { 
        
        }

        public SolvePartialProblemsMessage(string problemType, ulong id, byte[] commonData,ulong solvingTimeout, List<PartialProblem> partialProblems = null)
        {
            ProblemType = problemType;
            Id = id;

            CommonData = new byte[commonData.Length];
            Array.Copy(commonData, CommonData, commonData.Length);

            SolvingTimeout = solvingTimeout;

            PartialProblems = partialProblems ?? new List<PartialProblem>();

        }

    }
}
