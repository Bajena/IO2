using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllProject.DVRPRequired
{
    [Serializable]
    class Set
    {
        public List<int> Elements;

        public static List<Set> GenerateAllSubsets(int n)
        {
            List<Set> Subsets = new List<Set>();
            int count = (int)Math.Pow(2, n);
            for (int i = 1; i < count; ++i)
            {
                Set set = new Set();
                BitArray bits = new BitArray(new int[] { i });

                for (int e = 0; e < bits.Count; e++)
                {
                    if (bits[e])
                    {
                        set.Add(e);
                    }
                }
                Subsets.Add(set);
            }
            return Subsets;
        }

        public void Add(int e)
        {
            Elements.Add(e);
        }
        public void Remove(int e)
        {
            Elements.Remove(e);
        }
        public override string ToString()
        {
            Elements.Sort();
            string elementsString = "";
            foreach (int elem in Elements)
            {
                elementsString += elem + " ";
            }
            return "{ " + elementsString + "}";
        }
        public Set()
        {
            Elements = new List<int>();
        }
        public Set(int e)
        {
            Elements = new List<int>();
            Elements.Add(e);
        }
        public Set(List<int> elems)
        {
            Elements = new List<int>();
            Elements.AddRange(elems);
        }
    }
}
