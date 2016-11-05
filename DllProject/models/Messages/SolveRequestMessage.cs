using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace DllProject.Messages
{
    [XmlRoot(ElementName = "SolveRequest", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolveRequestMessage : Message
    {
        public string ProblemType { get; set; }
        public ulong SolvingTimeout { get; set; } // in ms
        public byte[] Data { get; set; }

        public SolveRequestMessage()
        { 
        
        }

        public SolveRequestMessage(Problem problem)
        {
            ProblemType = problem.ProblemType;
            SolvingTimeout = problem.SolvingTimeout;
            
            Data = new byte[problem.Data.Length];
            Array.Copy(problem.Data, Data, problem.Data.Length); 
        }
        public SolveRequestMessage(string problemType,ulong solvingTimeout, byte[] data)
        {
            ProblemType = problemType;
            SolvingTimeout = solvingTimeout;

            Data = new byte[data.Length];
            Array.Copy(data, Data,data.Length);
        }


    }
}
