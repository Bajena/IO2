using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Abstractions;
using UCCTaskSolver;
using Common.Enumerations;
using Common.Models.Messages;
using Common.Solvers;
using Common.Models;
namespace Components
{
    public class PartialProblemsHelper
    {
        public ulong ProblemId {get { return Message.Id; }}
        public string ProblemType {get { return Message.ProblemType; }}
        public List<Solution> Solutions { get; set; } 
        public List<PartialProblem> Problems{get { return Message.PartialProblems; }} 
        public SolvePartialProblemsMessage Message { get; private set; }

        public PartialProblemsHelper(SolvePartialProblemsMessage message)
        {
            Message = message;
            Solutions = new List<Solution>();
        }
    }

    public class ComputationalNode : Component
    {

        protected override void ProcessSolvePartialProblemsMessage(SolvePartialProblemsMessage message)
        {
            var partialProblemsHelper = new PartialProblemsHelper(message);
            
            foreach (var partialProblem in partialProblemsHelper.Problems)
            {
                var thread = new IOThread()
                {
                    TaskId = partialProblem.TaskId,
                    ProblemInstanceId = message.Id,
                    ProblemType = message.ProblemType,
                    State = EState.Busy,
                    RealThread = new Thread(() => SolvePartialProblem(partialProblemsHelper, partialProblem))
                };
                _runningThreads.Add(thread);
                thread.RealThread.Start();
            }
        }

        public ComputationalNode()
        {
            Type = EComponentType.ComputationalNode;
        }

        public ComputationalNode(List<String> problemTypes)
        {
            SolvableProblems = problemTypes;
            Type = EComponentType.ComputationalNode;
        }

        private void SolvePartialProblem(PartialProblemsHelper partialProblemsHelper, PartialProblem partialProblem)
        {
            var taskSolver = GetProperTaskSolver(partialProblemsHelper.ProblemType, partialProblem.Data);
            byte[] solution = taskSolver.Solve(partialProblem.Data, new TimeSpan(100000,0,0,0)); //TODO: Ogarnac te timeouty


            bool dziaa = false;
            IOThread thread = null;
            while (!dziaa)
            {
                try
                {
                    thread = _runningThreads.FirstOrDefault(x => x.TaskId == partialProblem.TaskId);
                    thread.State = EState.Idle;
                    dziaa = true;
                }
                catch (Exception)
                {
                }
            }
                partialProblemsHelper.Solutions.Add(new Solution(partialProblem.TaskId, false, ESolutionType.Partial,
                thread.HowLong, solution)); //TODO: Ogarnac te timeouty też

                if (partialProblemsHelper.Solutions.Count == partialProblemsHelper.Problems.Count)
                    //Wszystkie rozwiazania policzone
                {
                    SolutionsMessage solutionsMessage = new SolutionsMessage(partialProblemsHelper.ProblemType,
                        partialProblemsHelper.ProblemId, partialProblemsHelper.Message.CommonData,
                        partialProblemsHelper.Solutions);
                    SendMessage(solutionsMessage.Serialize());
                }
                lock (_runningThreadsLockObject)
                {
                    _runningThreads.Remove(thread);
                }
            
        }

        private TaskSolver GetProperTaskSolver(string problemType, byte[] data)
        {
            if (!SolvableProblems.Contains(problemType))
                throw new KeyNotFoundException();
            switch (problemType)
            {
                case ("MultiplyProblem"):
                    return new MultiplyTaskSolver(data);
                case ("DVRP"):
                    return new DllProject.DvrpTaskSolver(data);
            }

            return null;
        }
    }
}
