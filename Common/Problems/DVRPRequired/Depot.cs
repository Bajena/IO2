using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Problems.DVRPRequired
{
    [Serializable]
    public class Depot
    {
        public Point location;
        public double startTime;
        public double endTime;

        public Depot(Point loc, double end)
        {
            location = loc;
            startTime = 0;
            endTime = end;
        }
        public Depot(Tuple<int, int> xy, Tuple<int, int> startEnd)
        {
            location = new Point(xy);
            startTime = startEnd.Item1;
            endTime = startEnd.Item2;
        }
    }
}
