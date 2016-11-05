using DllProject.DVRPRequired;
using DllProject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCCTaskSolver;
using DllProject;

namespace DllProject
{
    public class DvrpTaskSolver : UCCTaskSolver.TaskSolver
    {
        DvrpProblemData problem;
        public DvrpSolutionData[] partialSolutions;
        Dictionary<List<int>, int> partialDictionary;
        DvrpSolutionData solution;//to be removed
        public DvrpTaskSolver(byte[] problemData)
            : base(problemData)
        {
            this.problem = DllProject.DvrpProblemData.Deserialize(problemData);
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;
            solution = new DvrpSolutionData();
            solution.StartTime = DateTime.Now;
            List<DvrpProblemData> dvrpPartialProblems = new List<DvrpProblemData>();
            partialDictionary = new Dictionary<List<int>, int>(new ListComparer<int>());
            var subsets = Set.GenerateAllSubsets(problem.Clients.Length);
            int tag = 0;

            foreach (var subset in subsets)
            {
                List<Client> clients = new List<Client>();

                foreach (int index in subset.Elements)
                {
                    clients.Add(problem.Clients[index]);
                }

                var partialProblem = new DvrpProblemData(clients.ToArray(), problem.Depots, problem.Fleet, tag);
                partialDictionary.Add(subset.Elements, tag);
                ++tag;
                dvrpPartialProblems.Add(partialProblem);
            }

            byte[][] dividedData = new byte[dvrpPartialProblems.Count][];
            for (int i = 0; i < dvrpPartialProblems.Count; i++)
            {
                dividedData[i] = dvrpPartialProblems[i].Serialize();
            }
            PartialProblems = dividedData;
            OnProblemDividingFinished(this);
            State = TaskSolverState.Idle;
            return dividedData;
        }

        public override event UnhandledExceptionEventHandler ErrorOccured;

        public override void MergeSolution(byte[][] solutions)
        {
            //m
            State = TaskSolverState.Merging;
            var datas = new List<DvrpSolutionData>();
            for (int i = 0; i < solutions.Count(); i++) //Deserialize
            {
                datas.Add(DvrpSolutionData.Deserialize(solutions[i]));
            }
            MergeSolution(datas); //Real merge
            OnSolutionsMergingFinished(this);
            State = TaskSolverState.Idle;
        }

        private void MergeSolution(List<DvrpSolutionData> datas)
        {
            //DVRP            
            partialSolutions = datas.ToArray();
            List<Route> currentSolution = new List<Route>();
            var set = new int[problem.Clients.Length];
            for (int i = 0; i < set.Length; ++i)
            {
                set[i] = i;
            }
            var partitions = Partitioning.GetAllPartitions(set, problem.Fleet.count);
            foreach (var partition in partitions)
            {
                double distance = 0;
                currentSolution = new List<Route>();
                foreach (var p in partition)
                {
                    var key = p.ToList();
                    key.Sort();
                    int tag = partialDictionary[key];
                    //tag = datas.Count - tag -1;
                    Route r = datas.First(d => d.Routes[0].Tag == tag).Routes[0];//datas[tag].Routes[0];//
                    distance += r.Distance;
                    if (distance > solution.Distance)
                    {
                        distance = double.MaxValue;
                        break;
                    }
                    currentSolution.Add(r);
                }
                if (distance < solution.Distance)
                {
                    solution.Routes = currentSolution.ToList();
                    solution.Distance = distance;
                }
            }
            Solution = solution.Serialize();
        }

        public override string Name
        {
            get { return "DVRP"; }
        }

        public override event ComputationsFinishedEventHandler ProblemDividingFinished;

        protected virtual void OnProblemDividingFinished(TaskSolver sender)
        {
            ComputationsFinishedEventHandler handler = ProblemDividingFinished;
            if (handler != null) handler(EventArgs.Empty, sender);
        }

        public override event ComputationsFinishedEventHandler ProblemSolvingFinished;

        protected virtual void OnProblemSolvingFinished(TaskSolver sender)
        {
            ComputationsFinishedEventHandler handler = ProblemSolvingFinished;
            if (handler != null) handler(EventArgs.Empty, sender);
        }

        public override event ComputationsFinishedEventHandler SolutionsMergingFinished;

        protected virtual void OnSolutionsMergingFinished(TaskSolver sender)
        {
            ComputationsFinishedEventHandler handler = SolutionsMergingFinished;
            if (handler != null) handler(EventArgs.Empty, sender);
        }


        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            State = TaskSolverState.Solving;
            var tspSolver = new RecurentTspSolver();
            var partialProblem = DvrpProblemData.Deserialize(partialData);

            Route route = tspSolver.SolveRecurent(partialProblem);

            State = TaskSolverState.Idle;
            OnProblemSolvingFinished(this);
            return (new DvrpSolutionData()
            {
                Routes = new List<Route>() { route },
                Distance = route.Distance
            }).Serialize();
        }
    }
}
