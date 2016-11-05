using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllProject.DVRPRequired
{
    [Serializable]
    public struct Point
    {
        public double x, y;
        public string Name;
        public static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }
        public Point(double _x, double _y, string name = "")
        {
            x = _x;
            y = _y;
            Name = name;
        }
        public Point(Tuple<int, int> xy, string name = "")
        {
            x = xy.Item1;
            y = xy.Item2;
            Name = name;
        }
        public override string ToString()
        {
            return "X: " + x + " Y: " + y;
        }
    }
}
