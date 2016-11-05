
using Common.Enumerations;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System;

namespace Common.Models
{
    [XmlRoot(ElementName = "Solution")]
    public class Solution
    {
        public virtual ulong? TaskId { get; set; }
        public bool TimeoutOccured { get; set; }

        [XmlElement(ElementName = "Type")]
        public ESolutionType SolutionType { get; set; }

        public ulong ComputationsTime { get; set; }

        public byte[] Data { get; set; }

        public Solution()
        {
            
        }

        public Solution(bool timeoutOccured, ESolutionType solutionType, ulong computationsTime, byte[] data)
        {
            TimeoutOccured = timeoutOccured;
            SolutionType = solutionType;
            ComputationsTime = computationsTime;
            Data = data;
        }

        public Solution(ulong taskId, bool timeoutOccured, ESolutionType solutionType, ulong computationsTime, byte[] data)
        {
            TaskId = taskId;
            TimeoutOccured = timeoutOccured;
            SolutionType = solutionType;
            ComputationsTime = computationsTime;
            Data = data;
        }
    }
}
