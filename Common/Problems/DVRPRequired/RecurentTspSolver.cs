using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Problems.DVRPRequired
{
    class RecurentTspSolver
    {
        Route Solution;
        double bestFound = Double.MaxValue;
        bool[] visitedClients;
        ///problem variables:
        double capacity;
        double speed = 1;
        Client[] clients;
        Depot depot;

        public Route SolveRecurent(DvrpProblemData problem)
        {
            Solution = new Route();
            bestFound = Double.MaxValue;
            ///problem variables:
            capacity = problem.Fleet.capacity;
            speed = problem.Fleet.speed;
            clients = problem.Clients;
            visitedClients = new bool[clients.Length];
            depot = problem.Depots[0];

            for (int i = 0; i < clients.Length; ++i)
            {
                visitedClients[i] = true;
                var nextPath = NextNode(visitedClients, clients[i], depot.location, 0, 0, capacity,1);
                if (nextPath != null)
                {
                    Solution = nextPath;
                }
                visitedClients[i] = false;
            }

            Solution.Distance = bestFound;
            Solution.Insert(0, depot.location);
            Solution.Tag = problem.Tag;
            return Solution;
        }
        private Route NextNode(bool[] visitedClients, Client stop, Point currentLocation, double time, double currentDistance, double cargo,int depotStopsCount)
        {
            Route path = new Route();
            Route bestPath = new Route();
            double distance = 0;
            //process the current stop
            time = Math.Max(time, stop.requestTime);
            if (cargo < -stop.requestSize)
            {
                path.Add(depot.location);
                distance = Point.GetDistance(currentLocation, depot.location);
                time += distance / speed;
                currentDistance += distance;
                currentLocation = depot.location;
                cargo = capacity;
            }
            distance = Point.GetDistance(currentLocation, stop.location);
            currentDistance += distance;
            time += distance / speed;
            time += stop.unloadTime;
            //cut if worse then current bound
            if (currentDistance >= bestFound || time > depot.endTime)
            {
                return null;
            }
            cargo += stop.requestSize;
            path.Add(stop.location);

            //find the next stop
            bool isEnd = true;
            bool reUp = false;
            Route nextPath=null;
            for (int i = 0; i < clients.Length; ++i)
            {
                if (!visitedClients[i])
                {
                    isEnd = false;
                    visitedClients[i] = true;
                    if (capacity >= -clients[i].requestSize)
                    {
                        nextPath = NextNode(visitedClients, clients[i], stop.location, time, currentDistance, cargo,depotStopsCount);
                        if (nextPath != null)
                        {
                            bestPath = nextPath;
                            reUp = false;
                        }
                    }
                    //going back for re-up now?
                    distance = Point.GetDistance(stop.location, depot.location);
                    if (depotStopsCount < clients.Length)
                        nextPath = NextNode(visitedClients, clients[i], depot.location, time +(distance/speed), currentDistance + distance, capacity,depotStopsCount+1);
                    if (nextPath != null)
                    {
                        bestPath = nextPath;
                        reUp = true;
                    }
                    visitedClients[i] = false;
                }
            }
            if (reUp)
            {
                path.Add(depot.location);
            }
            if (isEnd)
            {
                distance = Point.GetDistance(stop.location, depot.location);
                time += distance / speed;
                currentDistance += distance;
                if (currentDistance < bestFound && time < depot.endTime)
                {
                    path.Add(depot.location);
                    bestFound = currentDistance;
                    return path;
                }
                else
                    return null;
            }

            if (bestPath.Count > 1) //min 2, as the last path is: (->location->depot)
            {
                path.AddRange(bestPath);
                return path;
            }
            else
                return null;
        }
    }
}
