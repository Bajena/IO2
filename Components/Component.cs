using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Helpers;
using Common.Logging;
using Common.Models;
using Common.Models.Messages;
using Common.Enumerations;
using Common.Abstractions;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Components
{
    public abstract class Component
    {
        public ulong Id { get; protected set; }
        public int MaxParallelThreads { get; protected set; }
        public List<string> SolvableProblems { get; set; }
        public EComponentType Type { get; set; }

        protected List<IOThread> _runningThreads;
        protected object _runningThreadsLockObject = new object();

        private readonly MessageParser _messageParser;
        private Timer _statusTimer;
        private TcpClient _tcpClient;
        private const uint Timeout = 5000; // TODO - THIS IS NOT GOOD, NONONO NOooooo
        private const int MessageSize = 40960;

        /// <summary>
        /// Constructor
        /// </summary>
        public Component()
        {
            _tcpClient = new TcpClient();
            _runningThreads = new List<IOThread>();
            _messageParser = new MessageParser();
            MaxParallelThreads = 1;
            
        }

        /// <summary>
        /// registers the component to server
        /// </summary>
        /// <param name="endpoint"></param>
        public void Register(IPEndPoint endpoint)
        {
            _tcpClient.Connect(endpoint);

            var listen = new Thread(ListenForMessages);
            listen.Start();

            var msg = new RegisterMessage(Type, MaxParallelThreads, SolvableProblems);
            SendMessage(msg.Serialize());
        }

        /// <summary>
        /// Started in new thread. Waits for new incoming server messages.
        /// </summary>
        private void ListenForMessages()
        {
            var serverStream = _tcpClient.GetStream();

            while (true)
            {
                var messageStream = new MemoryStream();
                var inBuffer = new byte[MessageSize];

                try
                {
                    if (serverStream.CanRead)
                    {
                        do
                        {
                            var bytes = serverStream.Read(inBuffer, 0, 4096);                            
                            messageStream.Write(inBuffer, 0, bytes);
                        } while (serverStream.DataAvailable);
                    }
                }
                catch (Exception e)
                {
                    EventLogger.GetLog().ErrorFormat("Błąd podczas próby odebrania wiadomości: {0}", e.Message);
                    //a socket error has occured
                    break;
                }

                if (messageStream.Length == 0)
                {
                    //the client has disconnected from the server
                    break;
                }
                messageStream.Position = 0;
                byte[] completeMessage = messageStream.ToArray();
                //EventLogger.GetLog().InfoFormat("Odebrano wiadomość:\n{0}", Encoding.UTF8.GetString(completeMessage));
                ProcessMessage(completeMessage);
            }
            _tcpClient.Close();
        }

        private void StartStatusTimer()
        {
            _statusTimer = new Timer(Timeout);
            _statusTimer.Elapsed += RefeshStatus;
            _statusTimer.Start();
        }

        private void StopStatusTimer()
        {
            _statusTimer.Stop();
        }

        private void RefeshStatus(object source, ElapsedEventArgs e)
        {
            var msg = new StatusMessage(Id, _runningThreads);
            SendMessage(msg.Serialize());
        }

        protected void ProcessMessage(byte[] msg)
        {
            var encoder = new UTF8Encoding();
            string messagesString = encoder.GetString(msg);
            _messageParser.ParseMessage(messagesString);
            var messagesToProcess = _messageParser.GetMessagesToProcess();

            foreach (var xmlMessage in messagesToProcess)
            {
                Type type = Message.GetMessageType(xmlMessage);

                switch (type.Name)
                {
                    case ("RegisterResponseMessage"):
                        {
                            var regMsg = Message.Deserialize<RegisterResponseMessage>(xmlMessage);

                            ProcessRegisterResponseMessage(regMsg);
                            break;
                        }
                    case ("SolveRequestMessage"):
                        {
                            throw new NotImplementedException();
                            break;
                        }
                    case ("DivideProblemMessage"):
                        {
                            DivideProblemMessage divMsg = Message.Deserialize<DivideProblemMessage>(xmlMessage);
                            ProcessDivideProblemMessage(divMsg);
                            break;
                        }
                    case ("SolveRequestResponseMessage"):
                        {
                            var solveRequestResponseMsg = Message.Deserialize<SolveRequestResponseMessage>(xmlMessage);
                            ProcessSolveRequestResponseMessage(solveRequestResponseMsg);
                            break;
                        }
                    case ("SolutionRequestMessage"):
                        {
                            throw new NotImplementedException();
                            break;
                        }
                    case ("SolutionsMessage"):
                        {
                            var solutionsMsg = Message.Deserialize<SolutionsMessage>(xmlMessage);
                            ProcessSolutionsMessage(solutionsMsg);
                            break;
                        }
                    case ("SolvePartialProblemsMessage"):
                        {
                            SolvePartialProblemsMessage _msg = Message.Deserialize<SolvePartialProblemsMessage>(xmlMessage);
                            ProcessSolvePartialProblemsMessage(_msg);
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// sends a message to server, yes
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(byte[] msg)
        {
            //_tcpClient.GetStream().Write(msg, 0, msg.Length);
            try
            {
                _tcpClient.Client.Send(msg);
                //Thread.Sleep(500);
                //EventLogger.GetLog().InfoFormat("Wiadomość wysłana:\n{0}", Encoding.UTF8.GetString(msg));
            }
            catch (Exception e)
            {
                EventLogger.GetLog().ErrorFormat("Błąd podczas próby wysłania wiadomości: {0}", e.Message);
            }

        }

        /// <summary>
        /// closes the component connection
        /// </summary>
        public void CloseConnection()
        {
            StopStatusTimer();
            _tcpClient.Close();
        }

        private void ProcessRegisterResponseMessage(RegisterResponseMessage message)
        {
            Id = message.Id;
            //StartStatusTimer();
        }
        protected virtual void ProcessDivideProblemMessage(DivideProblemMessage message){ }
        protected virtual void ProcessSolveRequestMessage(SolveRequestMessage message) { }
        protected virtual void ProcessSolveRequestResponseMessage(SolveRequestResponseMessage message) { }
        protected virtual void ProcessSolutionRequestMessage(SolutionRequestMessage message) { }
        protected virtual void ProcessSolutionsMessage(SolutionsMessage message) { }
        protected virtual void ProcessSolvePartialProblemsMessage(SolvePartialProblemsMessage message) { }

    }
}
