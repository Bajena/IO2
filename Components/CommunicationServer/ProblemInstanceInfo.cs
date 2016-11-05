using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.CommunicationServer
{
    public class ProblemInstanceInfo
    {
        public ulong Id { get; set; }
        public byte[] Data { get; set; }
        public string ProblemType { get; set; }
        public ulong SolvingTimeout { get; set; }
        public ComponentsInfo TaskManager { get; set; }
        public List<ComponentsInfo> ComputationalNodes { get; set; }
        public ComponentsInfo ComputationalClient { get; set; }

        public ProblemInstanceInfo(ulong id,string problemType)
        {
            Id = id;
            ProblemType = problemType;
            ComputationalNodes = new List<ComponentsInfo>();
        }
    }
}
