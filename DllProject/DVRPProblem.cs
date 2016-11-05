using DllProject.DVRPRequired;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DllProject
{
    public class DvrpProblem : Problem
    {
        public DvrpProblemData MData { get; set; }

        public DvrpProblem()
        {
            ProblemType = "DVRP";
        }

        public DvrpProblem(DvrpProblemData problemData)
        {
            ProblemType = "DVRP";
            MData = problemData;
        }

        protected override byte[] GetData()
        {
            return MData.Serialize();
        }

        
    }
    [Serializable]
    public class DvrpProblemData
    {
        public Client[] Clients;
        public Depot[] Depots;
        public Vehicles Fleet;
        public double CutOff = 0.5;
        public int Tag;

        public byte[] Serialize()
        {
            //TODO
            MemoryStream m = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(m, new DvrpProblemData(Clients,Depots,Fleet,Tag));
            return m.ToArray(); //or File.WriteAllBytes(filename, m.ToArray())            
        }
        public static DvrpProblemData Deserialize(byte[] data)
        {
            var m = new MemoryStream(data);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(m) as DvrpProblemData;
        }

        public DvrpProblemData(Client[] c, Depot[] d, Vehicles f, int _tag=0)
        {
            Clients = c;
            Depots = d;
            Fleet = f;
            Tag = _tag;
        }
        public DvrpProblemData(string paramsPath)
        {
            DvrpFilesReader reader = new DvrpFilesReader(paramsPath);
            uint count = reader.NUM_VISITS;

            Depots = new Depot[1];
            Depots[0] = new Depot(reader.DepotLocationSection[0], reader.DepotTimeWindowSection[0]);

            //Clients = new Client[count];
            var clientsList = new List<Client>();
            var nextDayClients = new List<Client>();
            double cutOffTime = CutOff * Depots[0].endTime;
            for (int i = 0; i < count; ++i)
            {
                if (cutOffTime > reader.TimeAvailSection[i])
                {
                    clientsList.Add(new Client(reader.LocationCordSection[i + 1], reader.TimeAvailSection[i], reader.DurationSection[i], reader.Demands[i], i.ToString()));
                }
                else
                {
                    clientsList.Add(new Client(reader.LocationCordSection[i + 1], 0, reader.DurationSection[i], reader.Demands[i], i.ToString()));
                }
            }
            Clients = clientsList.ToArray();

            Fleet = new Vehicles(reader.CAPACITIES, (int)reader.NUM_VEHICLES);
        }
        
    }
}
