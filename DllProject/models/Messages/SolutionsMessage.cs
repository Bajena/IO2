using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
namespace DllProject.Messages
{
    [XmlRoot(ElementName = "Solutions", Namespace = "http://www.mini.pw.edu.pl/ucc/")]
    public class SolutionsMessage : Message
    {
        public string ProblemType { get; set; }
        public ulong Id { get; set; }
        public byte[] CommonData {get;set;} 

        [XmlArray("Solutions"), XmlArrayItem(typeof(Solution), ElementName = "Solution")]
        public List<Solution> Solutions { get; set; }

        public SolutionsMessage()
        { 
        
        }

        public byte[][] SerializeSolutions()
        {
            byte[][] data = new byte[Solutions.Count()][];
            for (int i = 0; i < Solutions.Count(); ++i)
            {
                MemoryStream m = new MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Serialize(m,Solutions[i]);
                data[i] = m.ToArray();
            }
            return data;
        }
        public SolutionsMessage(string problemType, ulong id, byte[] commonData, List<Solution> solutions = null)
        {
            Id = id;
            ProblemType = problemType;

            CommonData = new byte[commonData.Length];
            Array.Copy(commonData, CommonData, commonData.Length);

            Solutions = solutions ?? new List<Solution>();
        }
      

    }
}
