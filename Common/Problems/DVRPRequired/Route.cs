using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Problems.DVRPRequired
{
    [Serializable]
    public class Route : List<Point>
    {
        public double Distance = 0;
        public int Tag;
        public override string ToString()
        {
            string s = "\n";
            foreach (var p in this)
            {
                s += p + "\n";
            }
            s += "\nRoute Distance: " + Distance;
            return s;
        }
    }
}
