using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Common.Abstractions;
using Common.Enumerations;
using Common.Models;

namespace Common.Solutions
{
    public class MultiplySolution : Solution
    {
        private MultiplySolutionData data;

        public MultiplySolution(int result, ulong id, bool timeout_occured, ESolutionType type, ulong computations_time)
        {
            data = new MultiplySolutionData(result);
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
    public class MultiplySolutionData
    {
        public int result;

        public byte[] Serialize()
        {
            MemoryStream m = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(m, new MultiplySolutionData(result));
            return m.ToArray();
        }
        public static MultiplySolutionData Deserialize(byte[] data)
        {
            var m = new MemoryStream(data);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(m) as MultiplySolutionData;
        }
        public MultiplySolutionData(int _result)
        {
            result = _result;
        }
    }
}
