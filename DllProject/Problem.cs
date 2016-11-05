using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllProject
{
    [Serializable]
    public abstract class Problem
    {
        public string ProblemType;
        public ulong SolvingTimeout;
        public byte[] Data
        {
            get
            {
                return GetData();
            }
        }

        protected abstract byte[] GetData();
    }
}
