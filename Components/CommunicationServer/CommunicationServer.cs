using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Common.Logging;
using Common.Models;
using Common.Models.Messages;
using Common.Abstractions;
using Common.Enumerations;
using System.Timers;

namespace Components.CommunicationServer
{
    /// <summary>
    /// Main CommunicationServer class - manages collections of devices 
    /// </summary>
    public class CommunicationServer : TcpServer
    {
        private ulong _maxComponentId = 0;
        private readonly List<ComponentsInfo> _taskManagers;
        private readonly List<ComponentsInfo> _computationalNodes;
        private readonly List<ComponentsInfo> _computationalClients;
        private readonly List<ProblemInstanceInfo> _problemInstances;
        private  ThreadLocal<MessageParser> _messageParser;

        private uint _timeout;
        private ulong _maxProblemId = 0;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ipString"></param>
        /// <param name="port"></param>
        public CommunicationServer(string ipString, int port) : base(ipString, port)
        {
            _taskManagers = new List<ComponentsInfo>();
            _computationalNodes = new List<ComponentsInfo>();
            _computationalClients = new List<ComponentsInfo>();
            _problemInstances = new List<ProblemInstanceInfo>();
            _messageParser = new ThreadLocal<MessageParser>(() => new MessageParser());
            _timeout = 10000;
        }

        /// <summary>
        /// Procesess a byte array message received from a Component (byte to xml conversion, message logic etc.)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        public override void ProcessMessage(byte[] msg, TcpClient client)
        {
            Type messageType;
            ASCIIEncoding encoder = new ASCIIEncoding();
            
            string messagesString = encoder.GetString(msg);
            _messageParser.Value.ParseMessage(messagesString);
            var messagesToProcess = _messageParser.Value.GetMessagesToProcess();

            foreach (var messageString in messagesToProcess)
            {
                try
                {
                    messageType = Message.GetMessageType(messageString);
                }
                catch (Exception exception)
                {
                    EventLogger.GetLog().ErrorFormat("{0} - Problem z deserializacją wiadomości - {1}\n{2}", client.Client.RemoteEndPoint, exception.Message, messageString);
                    return;
                }

                try
                {
                    switch (messageType.Name)
                    {
                        case ("RegisterMessage"):
                            var regMsg = Message.Deserialize<RegisterMessage>(messageString);
                            RegisterComponent(client, regMsg.ComponentType, regMsg.ParallelThreads, regMsg.SolvableProblems);
                            break;
                        case ("StatusMessage"):
                            var statusMsg = Message.Deserialize<StatusMessage>(messageString);
                            GetComponentsInfoById(statusMsg.Id).ResetTimeoutTimer();
                            break;
                        case ("SolutionRequestMessage"):
                            {
                                break;
                            }
                        case ("SolutionsMessage"):
                            {
                                var solutionMsg = Message.Deserialize<SolutionsMessage>(messageString);
                                ProcessSolutionsMessage(solutionMsg);
                                break;
                            }
                        case ("SolvePartialProblemsMessage"):
                            {
                                var solvePartialProblemsMsg = Message.Deserialize<SolvePartialProblemsMessage>(messageString);
                                ProcessSolvePartialProblemsMessage(solvePartialProblemsMsg);
                                break;
                            }
                        case ("SolveRequestResponseMessage"):
                            {
                                break;
                            }
                        case ("SolveRequestMessage"):
                            {
                                var solveRequestMsg = Message.Deserialize<SolveRequestMessage>(messageString);
                                ProcessSolveRequestMessage(solveRequestMsg, client);
                                break;
                            }
                        case ("DivideProblemMessage"):
                            {
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    EventLogger.GetLog().ErrorFormat("{0} - Błąd przy przetwarzaniu wiadomości - {1}", client.Client.RemoteEndPoint, e.Message);
                }
            }
        }

        /// <summary>
        /// generates id for the component, adds it to an adequate list, and sends returning message
        /// </summary>
        /// <param name="client"></param>
        /// <param name="type"></param>
        /// <param name="parallelThreads"></param>
        /// <param name="solvableProblems"></param>
        public void RegisterComponent(TcpClient client,EComponentType type,int parallelThreads=1,List<string> solvableProblems=null)
        {        
            var componentsInfo = new ComponentsInfo(type, ++_maxComponentId, client, _timeout, parallelThreads,solvableProblems);
            componentsInfo.ComponentTimedOut += componentsInfo_ComponentTimedOut;
            switch (type.ToString())
            {
                case ("TaskManager"):
                    _taskManagers.Add(componentsInfo);
                    break;
                case ("ComputationalNode"):
                    _computationalNodes.Add(componentsInfo);
                    break;
                case ("ComputationalClient"):
                    _computationalClients.Add(componentsInfo);
                    break;
            }
            var responseMsg = new RegisterResponseMessage(_maxComponentId, DateTime.Now);
            SendMessage(responseMsg.Serialize(), client);
            EventLogger.GetLog().InfoFormat("Zarejestrowano {0} z Id:{1}", componentsInfo.Type,componentsInfo.Id);
        }

        void componentsInfo_ComponentTimedOut(object sender, EventArgs e)
        {
            var cInfo = sender as ComponentsInfo;
            EventLogger.GetLog().WarnFormat("{0}({1}) przekroczył timeout", cInfo.Type, cInfo.Id);
            TerminateComponent(cInfo);
        }

        protected override void RemoveClient(TcpClient client)
        {
            var cInfo = GetComponentsInfoByTcpClient(client);
            if (cInfo != null)
            {
                cInfo.StopTimeoutTimer();
                switch (cInfo.Type.ToString())
                {
                    case ("TaskManager"):
                        _taskManagers.Remove(cInfo);
                        break;
                    case ("ComputationalNode"):
                        _computationalNodes.Remove(cInfo);
                        break;
                    case ("ComputationalClient"):
                        _computationalClients.Remove(cInfo);
                        break;
                }
            }
            base.RemoveClient(client);
        }

        public void TerminateComponent(ComponentsInfo cInfo)
        {
           EventLogger.GetLog().WarnFormat("{0}({1}) Odłączony", cInfo.Type, cInfo.Id);
            RemoveClient(cInfo.Client);
        }


        private void ProcessSolveRequestMessage(SolveRequestMessage msg, TcpClient client)
        {
            var data = msg.Data;
            var problemType = msg.ProblemType;
            var timeout = msg.SolvingTimeout;
            var taskManagerInfo = GetTaskManagerByProblemType(problemType);

            if (taskManagerInfo == null)
            {
                EventLogger.GetLog().ErrorFormat("Brak task managera dla problemu {0}", msg.ProblemType);
                return;
            }
       
            var problemInstance = new ProblemInstanceInfo(++_maxProblemId, msg.ProblemType)
            {
                Data = data,
                TaskManager = taskManagerInfo,
                ComputationalClient = GetComponentsInfoByTcpClient(client),
                SolvingTimeout = timeout,
                ProblemType = msg.ProblemType
            };

            _problemInstances.Add(problemInstance);

            //Wyslij odpowiedz
            var responseMsg = new SolveRequestResponseMessage(problemInstance.Id);
            SendMessage(responseMsg.Serialize(), client);
            EventLogger.GetLog().InfoFormat("Otrzymano żądanie rozwiązania problemu {0} o id {1}", problemInstance.ProblemType, problemInstance.Id);

            //Wyslij prosbe o podzielenie
            var availableComputationalNodes = _computationalNodes.Count(c => c.SolvableProblems.Contains(problemInstance.ProblemType));
            var divideProblemMsg = new DivideProblemMessage(problemInstance.ProblemType,problemInstance.Id,problemInstance.Data,(ulong)availableComputationalNodes);
            SendMessage(divideProblemMsg.Serialize(), taskManagerInfo.Client);
            EventLogger.GetLog().InfoFormat("Wysłano żądanie podzielenia problemu {0} o id {1} do TM {2}", problemInstance.ProblemType, problemInstance.Id,taskManagerInfo.Id);
        }

        private void ProcessSolvePartialProblemsMessage(SolvePartialProblemsMessage partialProblemsMsg)
        {
            var availableNodes = GetComputationalNodesByProblemType(partialProblemsMsg.ProblemType);
            var availableNodesCount = availableNodes.Count;
            if (availableNodesCount == 0)
            {
                EventLogger.GetLog().ErrorFormat("Brak node'ów dla problemu {0} - id:{1}", partialProblemsMsg.ProblemType,partialProblemsMsg.Id);
                return;
            }

            var subProblemDivision = new List<List<PartialProblem>>(availableNodesCount); //Lista podproblemow dla kazdego node'a 
            for (int i = 0;i<availableNodesCount;i++)
            {
                subProblemDivision.Add(new List<PartialProblem>());
            }
            var partialProblems = partialProblemsMsg.PartialProblems;

            for (int i = 0; i < partialProblems.Count; i++) //przydziel podproblemy
            {
                subProblemDivision[i%availableNodesCount].Add(partialProblems[i]);
            }

           for (int i = 0;i<subProblemDivision.Count;i++) //Roześlij
           {
               var node = availableNodes[i];
                var message = new SolvePartialProblemsMessage()
                {
                    CommonData = partialProblemsMsg.CommonData,
                    Id = partialProblemsMsg.Id,
                    PartialProblems = subProblemDivision[i],
                    ProblemType = partialProblemsMsg.ProblemType,
                    SolvingTimeout = partialProblemsMsg.SolvingTimeout
                };
                SendMessage(message.Serialize(), node.Client);
                //EventLogger.GetLog().InfoFormat("Wysłano {0} partial problemy typu {1} do Computational Noda o id {2}", subProblemDivision[i].Count, partialProblemsMsg.ProblemType, node.Id);
            }

        }

        private void ProcessSolutionsMessage(SolutionsMessage solutionsMessage)
        {
            switch (solutionsMessage.Solutions[0].SolutionType)
            {
                case (ESolutionType.Partial):
                    ProcessPartialSolutionsMessage(solutionsMessage);
                    break;
                case (ESolutionType.Final):
                    ProcessFinalSolutionsMessage(solutionsMessage);
                    break;
            }
        }

        private void ProcessPartialSolutionsMessage(SolutionsMessage solutionsMessage)
        {
            ComponentsInfo taskManager = GetTaskManagerByProblemId(solutionsMessage.Id);
            if (taskManager == null)
            {
                EventLogger.GetLog().ErrorFormat("Brak task managera dla problemu {0}", solutionsMessage.Id);
                return;
            }
            SendMessage(solutionsMessage.Serialize(), taskManager.Client);
        }

        private void ProcessFinalSolutionsMessage(SolutionsMessage solutionsMessage)
        {
            ComponentsInfo computationalClient = GetComputationalClientByProblemId(solutionsMessage.Id);
            var solution = solutionsMessage.Solutions[0];

            var finalSolutionMessage = new SolutionsMessage(solutionsMessage.ProblemType, solutionsMessage.Id,
                solutionsMessage.CommonData, new List<Solution>() {solution});
            SendMessage(finalSolutionMessage.Serialize(),computationalClient.Client);
            //throw new NotImplementedException();
            
        }

        private ComponentsInfo GetComponentsInfoById(ulong id)
        {
            if (_taskManagers.FirstOrDefault(x => x.Id == id) != null)
                return _taskManagers.First(x => x.Id == id);
            if (_computationalClients.FirstOrDefault(x => x.Id == id) != null)
                return _computationalClients.First(x => x.Id == id);
            if (_computationalNodes.FirstOrDefault(x => x.Id == id) != null)
                return _computationalNodes.First(x => x.Id == id);

            return null;
        }

        private ComponentsInfo GetComponentsInfoByTcpClient(TcpClient tcpClient)
        {
            if (_taskManagers.FirstOrDefault(x => x.Client == tcpClient) != null)
                return _taskManagers.First(x => x.Client == tcpClient);
            if (_computationalClients.FirstOrDefault(x => x.Client == tcpClient) != null)
                return _computationalClients.First(x => x.Client == tcpClient);
            if (_computationalNodes.FirstOrDefault(x => x.Client == tcpClient) != null)
                return _computationalNodes.First(x => x.Client == tcpClient);

            return null;
        }

        private ProblemInstanceInfo GetProblemInstanceById(ulong problemId)
        {
           return _problemInstances.FirstOrDefault(x => x.Id == problemId);
        }

        private ComponentsInfo GetComputationalClientByProblemId(ulong problemId)
        {
            var task = GetProblemInstanceById(problemId);
            if (task == null) return null;
            return task.ComputationalClient;
        }

        private ComponentsInfo GetTaskManagerByProblemId(ulong problemId)
        {
            var task = GetProblemInstanceById(problemId);
            if (task == null) return null;
            return task.TaskManager;
        }

        private ComponentsInfo GetTaskManagerByProblemType(string problemType)
        {
            return _taskManagers.FirstOrDefault(x => x.SolvableProblems.Contains(problemType));
        }

        private IList<ComponentsInfo> GetComputationalNodesByProblemType(string problemType)
        {
            return _computationalNodes.Where(x => x.SolvableProblems.Contains(problemType)).ToList();
        }

        //private IList<ComponentsInfo> GetComputationalNodesByForTask(ulong taskId)
        //{
        //    var problemInstance = _problemInstances.FirstOrDefault(x => x.Id )
        //    return _computationalNodes.Where(
        //        )
        //    _computationalNodes.Count(c => c.SolvableProblems.Contains(problemInstance.ProblemType))
        //}
    }
}
