using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;
using Common.Enumerations;

namespace Components.CommunicationServer
{
    //enables storage of information about components in the communication server
    public class ComponentsInfo
    {
        public delegate void ComponentTimedOutEventHandler(object sender, EventArgs e);

        public event ComponentTimedOutEventHandler ComponentTimedOut;

        public ulong Id { get; private set; }
        public TcpClient Client { get; set; }
        public int ParallelThreads { get; set; }
        public List<string> SolvableProblems { get; set; }
        public EComponentType Type { get; set; }
        private readonly Timer _apoloniuszTimer;

        public ComponentsInfo(EComponentType type, ulong id, TcpClient client, uint timeout, int pThreads=1, List<string> solvableProblems=null)
        {
            Id = id;
            Client = client;
            ParallelThreads = pThreads;
            SolvableProblems = solvableProblems;
            Type = type;
            _apoloniuszTimer = new Timer(timeout);
            _apoloniuszTimer.Elapsed += OnTimeout;
            _apoloniuszTimer.Start();
        }

        public void ResetTimeoutTimer()
        {
            _apoloniuszTimer.Stop();
            _apoloniuszTimer.Start();
        }

        public void StopTimeoutTimer()
        {
            _apoloniuszTimer.Stop();
        }
        private void OnTimeout(object source, ElapsedEventArgs e)
        {
            //ComponentTimedOut.Invoke(this,new EventArgs());
            _apoloniuszTimer.Stop();
        }

    }
}
