using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllProject.DVRPRequired
{
    class DvrpFilesReader
    {
        string _filename;
        bool depots_flag;
        bool demands_flag;
        bool locationCordSection_flag;
        bool depotLocationSection_flag;
        bool visitLocationSection_flag;
        bool depotTimeWindowSection_flag;
        bool timeAvailSection_flag;
        bool durationSection_flag;

        public String VRPTEST { get; set; }
        public List<String> COMMENT { get; set; }
        public String NAME { get; set; }
        public uint NUM_DEPOTS { get; set; }
        public uint NUM_CAPACITIES { get; set; }
        public uint NUM_VISITS { get; set; }
        public uint NUM_LOCATIONS { get; set; }
        public uint NUM_VEHICLES { get; set; }
        public uint CAPACITIES { get; set; }
        public List<int> Depots { get; set; }
        public List<int> Demands { get; set; }
        public List<Tuple<int, int>> LocationCordSection { get; set; }
        public List<Tuple<int, int>> DepotLocationSection { get; set; }
        public List<Tuple<int, int>> VisitLocationSection { get; set; }
        public List<int> DurationSection { get; set; }
        public List<Tuple<int, int>> DepotTimeWindowSection { get; set; }
        public List<int> TimeAvailSection { get; set; }

        public DvrpFilesReader(string Filename)
        {
            this._filename = Filename;
            demands_flag = false;
            depots_flag = false;
            locationCordSection_flag = false;
            depotLocationSection_flag = false;
            visitLocationSection_flag = false;
            depotTimeWindowSection_flag = false;
            timeAvailSection_flag = false;
            durationSection_flag = false;


            COMMENT = new List<string>();
            Depots = new List<int>();
            Demands = new List<int>();
            LocationCordSection = new List<Tuple<int, int>>();
            DepotLocationSection = new List<Tuple<int, int>>();
            VisitLocationSection = new List<Tuple<int, int>>();
            DurationSection = new List<int>();
            DepotTimeWindowSection = new List<Tuple< int, int>>();
            TimeAvailSection = new List<int>();
            Init();
        }

        private void Init()
        {
            StreamReader objReader = new StreamReader(_filename);
            string sLine = "";
            int numberLine = 0;
            while (sLine != "EOF")
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    HandleLine(sLine, numberLine++);
                }
            }

            //data section
            //string data = objReader.ReadToEnd();


            objReader.Close();
        }

        private bool SetFlags(string sLine)
        {
            switch (sLine.Trim())
            {
                case "DEPOTS":
                    depots_flag = true;
                    return true;
                case "DEMAND_SECTION":
                    depots_flag = false;
                    demands_flag = true;
                    return true;
                case "LOCATION_COORD_SECTION":
                    demands_flag = false;
                    locationCordSection_flag = true;
                    return true;
                case "DEPOT_LOCATION_SECTION":
                    locationCordSection_flag = false;
                    depotLocationSection_flag = true;
                    return true;
                case "VISIT_LOCATION_SECTION":
                    depotLocationSection_flag = false;
                    visitLocationSection_flag = true;
                    return true;
                case "DURATION_SECTION":
                    visitLocationSection_flag = false;
                    durationSection_flag = true;
                    return true;
                case "DEPOT_TIME_WINDOW_SECTION":
                    durationSection_flag = false;
                    depotTimeWindowSection_flag = true;
                    return true;
                case "TIME_AVAIL_SECTION":
                    depotTimeWindowSection_flag = false;
                    timeAvailSection_flag = true;
                    return true;
            }

            return false;
        }

        private bool SetSingleValues(String begin, String rest)
        {
            switch (begin)
            {
                case "VRPTEST": VRPTEST = rest;
                    return true;
                case "COMMENT:": COMMENT.Add(rest);
                    return true;
                case "NAME:": NAME = rest;
                    return true;
                case "NUM_DEPOTS:": NUM_DEPOTS = uint.Parse(rest);
                    return true;
                case "NUM_CAPACITIES:": NUM_CAPACITIES = uint.Parse(rest);
                    return true;
                case "NUM_VISITS:": NUM_VISITS = uint.Parse(rest);
                    return true;
                case "NUM_LOCATIONS:": NUM_LOCATIONS = uint.Parse(rest);
                    return true;
                case "NUM_VEHICLES:": NUM_VEHICLES = uint.Parse(rest);
                    return true;
                case "CAPACITIES:": CAPACITIES = uint.Parse(rest);
                    return true;
            }

            return false;
        }

        private void HandleLine(string sLine, int number)
        {
            if (sLine == "EOF") return;

            var collection = sLine.Trim().Split();
            var rest = sLine.Substring(collection[0].Length).Trim();

            if (SetFlags(sLine)) return; ;

            if (SetSingleValues(collection[0], rest)) return;

            if (depots_flag)
            {
                Depots.Add(int.Parse(collection[0]));
            }
            else if (demands_flag)
            {
                Demands.Add(int.Parse(collection[1]));
            }
            else if (locationCordSection_flag)
            {
                LocationCordSection.Add(new Tuple<int, int>(int.Parse(collection[1]), int.Parse(collection[2])));
            }
            else if (depotLocationSection_flag)
            {
                DepotLocationSection.Add(new Tuple<int, int>(int.Parse(collection[0]), int.Parse(collection[1])));
            }
            else if (durationSection_flag)
            {
                DurationSection.Add(int.Parse(collection[1]));
            }
            else if (visitLocationSection_flag)
            {
                VisitLocationSection.Add(new Tuple<int, int>(int.Parse(collection[0]), int.Parse(collection[1])));
            }
            else if (depotTimeWindowSection_flag)
            {
                DepotTimeWindowSection.Add(new Tuple< int, int>(int.Parse(collection[1]), int.Parse(collection[2])));
            }
            else if (timeAvailSection_flag)
            {
                TimeAvailSection.Add(int.Parse(collection[1]));
            }

        }

    }
}
