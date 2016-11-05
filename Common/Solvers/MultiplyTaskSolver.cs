using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCCTaskSolver;
using Common.Problems;
using Common.Solutions;
using Common.Enumerations;
using Common;
namespace Common.Solvers
{
    public class MultiplyTaskSolver : TaskSolver
    {
        MultiplyProblemData mData;

        public MultiplyTaskSolver(byte[] problemData)
            : base(problemData)
        {
            mData = MultiplyProblemData.Deserialize(problemData);
        }

        public override byte[][] DivideProblem(int threadCount)
        {
            State = TaskSolverState.Dividing;
            byte[][] dividedData = new byte[mData.Number][];
            for (int i = 0; i < mData.Number; ++i)
            {
                dividedData[i] = new MultiplyProblemData(1, mData.Multiplier).Serialize();
            }
            PartialProblems = dividedData;
            OnProblemDividingFinished(this);
            State = TaskSolverState.Idle;
            return dividedData;
        }

        public override event UnhandledExceptionEventHandler ErrorOccured;

        public override void MergeSolution(byte[][] solutions)
        {
            State = TaskSolverState.Merging;
            var datas = new List<MultiplySolutionData>();
            for (int i = 0; i < solutions.Count(); i++) //Deserialize
            {
                datas.Add(MultiplySolutionData.Deserialize(solutions[i]));
            }
            MergeSolution(datas); //Real merge
            OnSolutionsMergingFinished(this);
            State = TaskSolverState.Idle;
        }
        private void MergeSolution(IEnumerable<MultiplySolutionData> solutions)
        {
            int count = solutions.Sum(s => s.result);
            Solution = new MultiplySolutionData(count).Serialize();
        }
        public override string Name
        {
            get { return "MultiplyTaskSolver"; }
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
            MultiplyProblemData problemData = MultiplyProblemData.Deserialize(partialData);
            //nic nie trzeba tu robic
            State = TaskSolverState.Idle;
            OnProblemSolvingFinished(this);
            return new MultiplySolutionData(problemData.Multiplier).Serialize();
        }
    }
}
