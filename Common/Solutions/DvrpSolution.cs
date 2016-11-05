using Common.Enumerations;
using Common.Models;
using Common.Problems.DVRPRequired;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common.Solutions
{
    public class DvrpSolution : Solution
    {
        private DvrpSolutionData data;

        public DvrpSolution(DvrpSolutionData result, ulong id, bool timeout_occured, ESolutionType type, ulong computations_time)
        {
            data = result;
            TaskId = id;
            TimeoutOccured = timeout_occured;
            SolutionType = type;
            ComputationsTime = computations_time;
        }
        protected byte[] GetData()
        {
            return data.Serialize();
        }
    }

    [Serializable]
    public class DvrpSolutionData
    {
        public List<Route> Routes;
        public double Distance = double.MaxValue;
        public DateTime StartTime, EndTime;
        public int Tag;

        public byte[] Serialize()
        {
            MemoryStream m = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(m, this);
            return m.ToArray();
        }
        public static DvrpSolutionData Deserialize(byte[] data)
        {
            var m = new MemoryStream(data);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(m) as DvrpSolutionData;
        }

        public override string ToString()
        {
            string s = "\nRoute:\n";
            foreach (var r in Routes)
            {
                s += r;
            }
            s += "\nTotal Distance: " + Distance;
            s += "\nStarted computing at: " + StartTime;
            s += "\nFinished computing at: " + EndTime;
            return s;
        }
    }
}
