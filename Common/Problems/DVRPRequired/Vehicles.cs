using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Problems.DVRPRequired
{
    [Serializable]
    public class Vehicles
    {
        public double speed;
        public double capacity;
        public int count;

        public Vehicles(double cap, int n, double spd = 1)
        {
            speed = spd;
            count = n;
            capacity = cap;
        }
    }
}
