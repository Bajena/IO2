using System;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using DllProject.Enumerations;

namespace DllProject
{
    
    [XmlRoot(ElementName = "Thread")] 
    public class IOThread
    {
        [XmlElement(ElementName = "State")]
        public EState State { get; set; }

        public ulong HowLong //in ms
        {
            get { return (ulong) (DateTime.Now - _creationTime).Milliseconds; }
            set { _howLong = value; }
        }
        public virtual ulong ProblemInstanceId {get;set;}  
        public virtual ulong TaskId { get; set; }
        public string ProblemType { get; set; }

        [XmlIgnore]
        public Thread RealThread;

        [XmlIgnore]
        private ulong _howLong;

        [XmlIgnore]
        private DateTime _creationTime;

        public IOThread()
        {
            _creationTime = DateTime.Now;
        }
    }
}