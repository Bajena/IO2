using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Common.Models;
using Common.Models.Messages;
using Common.Solutions;
using Common.Solvers;
using log4net.Repository.Hierarchy;
using UCCTaskSolver;
using Common.Enumerations;
using System.Threading;
using Thread = System.Threading.Thread;

namespace Components
{
    public class ProblemSolvingHelper
    {
        public ulong ProblemId { get; set; }
        public string ProblemType { get; set; }
        public List<Solution> PartialSolutions { get; set; }
        public List<PartialProblem> PartialProblems { get; set; }
        public TaskSolver TaskSolver { get; set; }
        public byte[] CommonData { get; set; }

        public ulong ComputationsTime
        {
            get { return (ulong) (DateTime.Now - _computationStartTime).Milliseconds; }
        }

        private DateTime _computationStartTime;

        public ProblemSolvingHelper()
        {
            PartialSolutions = new List<Solution>();
            PartialProblems = new List<PartialProblem>();
            _computationStartTime = DateTime.Now;
        }
    }

    public class TaskManager : Component
    {
        private ulong _maxTaskId = 0;
        private Dictionary<ulong, ProblemSolvingHelper> _problems;
        private const int PartialProblemsPackageSize = 1;

        public TaskManager()
        {
            _problems = new Dictionary<ulong, ProblemSolvingHelper>();
            Type = EComponentType.TaskManager;
        }

        protected override void ProcessDivideProblemMessage(DivideProblemMessage message)
        {

            if (SolvableProblems.FirstOrDefault(x => x == message.ProblemType) == null)
            {
                EventLogger.GetLog().ErrorFormat("Nie obsługiwany problem: {0}", message.ProblemType);
                return;
            }

            TaskSolver taskSolver = null;

            switch (message.ProblemType)
            {
                case ("MultiplyProblem"):
                    taskSolver = new MultiplyTaskSolver(message.Data);
                    break;
                case ("DVRP"):
                    taskSolver = new DllProject.DvrpTaskSolver(message.Data);
                    break;
            }

            var problemSolvingHelper = new ProblemSolvingHelper()
            {
                ProblemId = message.Id,
                ProblemType = message.ProblemType,
                TaskSolver = taskSolver,
            };
            _problems.Add(message.Id,problemSolvingHelper);
            taskSolver.ProblemDividingFinished+=taskSolver_ProblemDividingFinished;
            taskSolver.SolutionsMergingFinished += taskSolver_SolutionsMergingFinished;

            var thread = new IOThread
            {
                ProblemType = message.ProblemType,
                State = EState.Busy,
                RealThread = new Thread(() => DivideProblem(message, taskSolver)),
                ProblemInstanceId = message.Id
            };
            _runningThreads.Add(thread);
            thread.RealThread.Start();
        }
        
        protected override void ProcessSolutionsMessage(SolutionsMessage message)
        {
            var problemHelper = _problems[message.Id];
            
            problemHelper.PartialSolutions.AddRange(message.Solutions);
            if (problemHelper.CommonData == null)
            {
                problemHelper.CommonData = message.CommonData;
            }

            if (problemHelper.PartialSolutions.Count == problemHelper.PartialProblems.Count)
            {
                var problemThread = GetThreadByProblemInstanceId(message.Id);
                problemThread.RealThread = new Thread(() => MergeProblem(problemHelper));
                problemThread.State = EState.Busy;
                problemThread.RealThread.Start();
            }
        }

        private void MergeProblem(ProblemSolvingHelper problemHelper)
        {
            var taskSolver = problemHelper.TaskSolver;
            var solutions = problemHelper.PartialSolutions.Select(s => s.Data).ToArray();
            taskSolver.MergeSolution(solutions);
            var finalSolution = new Solution(false, ESolutionType.Final, problemHelper.ComputationsTime, taskSolver.Solution);
            var message = new SolutionsMessage(problemHelper.ProblemType, problemHelper.ProblemId, problemHelper.CommonData, new List<Solution>(){finalSolution});
            SendMessage(message.Serialize());
        }

        private void DivideProblem(DivideProblemMessage msg,TaskSolver taskSolver)
        {
            var partialProblemsData = taskSolver.DivideProblem((int) msg.ComputationalNodes);
            var messagesList = new List<SolvePartialProblemsMessage>();
            for (int i = partialProblemsData.Length - 1; i >= 0; i -= PartialProblemsPackageSize)
            {
                var packageSize = i +1 < PartialProblemsPackageSize ? i+1 : PartialProblemsPackageSize;
                var partialProblemsPackage = new byte[packageSize][];
                for (int j = 0; j < packageSize; j++)
                {
                    partialProblemsPackage[j] = partialProblemsData[i - j];
                }
                var partialProblemsObjects = new List<PartialProblem>();
                foreach (var partialData in partialProblemsPackage)
                {
                    partialProblemsObjects.Add(new PartialProblem()
                    {
                        Data = partialData,
                        TaskId = ++_maxTaskId
                    });
                }
                _problems[msg.Id].PartialProblems.AddRange(partialProblemsObjects);
                //TODO: NA PEWNO NIE MILIJON TAJMAŁTUW
                var partialProblemsMsg = new SolvePartialProblemsMessage(msg.ProblemType, msg.Id, new byte[0], 100000000, partialProblemsObjects);
                messagesList.Add(partialProblemsMsg);
            }

            foreach (var partialProblemsMessage in messagesList)
            {
                var serialized = partialProblemsMessage.Serialize();
                SendMessage(serialized);
            }

            var thread = GetThreadByProblemInstanceId(msg.Id);
            thread.State = EState.Idle;
        }

        IOThread GetThreadByProblemInstanceId(ulong problemInstanceId)
        {
            return _runningThreads.FirstOrDefault(t => t.ProblemInstanceId == problemInstanceId);
        }

        void taskSolver_SolutionsMergingFinished(EventArgs eventArgs, TaskSolver sender)
        {
            EventLogger.GetLog().InfoFormat("TaskSolver {0} skonczyl mergowanie problemu", sender.Name);
        }

        void taskSolver_ProblemDividingFinished(EventArgs eventArgs, TaskSolver sender)
        {
            EventLogger.GetLog().InfoFormat("TaskSolver {0} skonczyl dzielenie problemu",sender.Name);
        }

    }
}
