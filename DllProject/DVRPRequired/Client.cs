using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllProject.DVRPRequired
{
    [Serializable]
    public class Client
    {
        public Point location;
        public double requestTime;
        public double unloadTime;
        public double requestSize;
        public string Name;

        public Client(Point loc, double reqTime = 0, double unload = 1, double size = 1)
        {
            location = loc;
            requestTime = reqTime;
            unloadTime = unload;
            requestSize = size;
        }
        public Client(Tuple<int, int> xy, double reqTime = 0, double unload = 1, double size = 1, string _name = "")
        {
            location = new Point(xy);
            requestTime = reqTime;
            unloadTime = unload;
            requestSize = size;
            Name = _name;
        }
    }
}
