using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Common.Abstractions;

namespace Common.Problems
{
    public class MultiplyProblem : Problem
    {
        private MultiplyProblemData MData {get;set;}

        protected override byte[] GetData()
        {
            return MData.Serialize();
        }
        public MultiplyProblem()
        {
            ProblemType = "MultiplyProblem";
        }
        public MultiplyProblem(int number, int multiplier,ulong timeout)
        {
            ProblemType = "MultiplyProblem";
            SolvingTimeout = timeout;
            MData = new MultiplyProblemData(number,multiplier);
        }
    }
    [Serializable]
    public class MultiplyProblemData
    {
        public int Number { get; set; }
        public int Multiplier { get; set; }

        public byte[] Serialize()
        {
            //TODO
            MemoryStream m = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(m, new MultiplyProblemData(Number, Multiplier));
            return m.ToArray(); //or File.WriteAllBytes(filename, m.ToArray())            
        }
        public static MultiplyProblemData Deserialize(byte[] data)
        {
            var m = new MemoryStream(data);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(m) as MultiplyProblemData;
        }
        public MultiplyProblemData()
        {

        }
        public MultiplyProblemData(int number, int multiplier)
        {
            Number = number;
            Multiplier = multiplier;
        }
    }
}
