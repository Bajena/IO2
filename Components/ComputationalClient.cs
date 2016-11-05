using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Common.Enumerations;
using Common.Logging;
using Common.Models;
using Common.Models.Messages;
using Common.Solutions;

namespace Components
{
    public class ComputationalClient : Component
    {
        private ulong _taskId;
        private bool _waitingForSolution;
        private Timer _solutionRequestApoloniuszTimer;

        private const int SolutionCheckTime = 1000;

        public ComputationalClient()
            : base()
        {
            Type = EComponentType.ComputationalClient;
        }

        protected override void ProcessSolutionsMessage(SolutionsMessage message)
        {
            foreach (var solution in message.Solutions)
            {
                if (solution.SolutionType == ESolutionType.Final)
                {
                    ProcessFinalSolution(message,solution);
                }
                else if (solution.SolutionType == ESolutionType.Partial)
                {
                    throw new NotImplementedException();
                }
                else if (solution.SolutionType == ESolutionType.Ongoing)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void ProcessFinalSolution(SolutionsMessage message, Solution solution)
        { 
            EventLogger.GetLog().InfoFormat("Odebrano rozwiazanie dla problemu typu: {0} o id: {1}",message.ProblemType,message.Id);
            switch (message.ProblemType)
            {
                case "DVRP":
                    var dvrpSolution =DllProject.DvrpSolutionData.Deserialize(solution.Data);
                    EventLogger.GetLog().InfoFormat(dvrpSolution.ToString());
                    break;
                case "MultiplyProblem":
                    var multiplySolution = MultiplySolutionData.Deserialize(solution.Data);
                    EventLogger.GetLog().InfoFormat("Wynik: {0}",multiplySolution.result);

                    break;
            }
        }

        protected override void ProcessSolveRequestResponseMessage(SolveRequestResponseMessage message)
        {
            _taskId = message.Id;
            _waitingForSolution = true;
            _solutionRequestApoloniuszTimer = new Timer(SolutionCheckTime);
            _solutionRequestApoloniuszTimer.Elapsed += CheckForSolution;
            //_solutionRequestApoloniuszTimer.Start();

            EventLogger.GetLog().InfoFormat("Zaczęto rozwiązywanie problemu. Przydzielone id:{0}",_taskId);
        }

        private void CheckForSolution(object sender, ElapsedEventArgs e)
        {
            var solutionRequestMsg = new SolutionRequestMessage(_taskId);
            SendMessage(solutionRequestMsg.Serialize());
        }
    }
}
