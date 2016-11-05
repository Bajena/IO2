using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common;
using Common.Enumerations;
using Common.Logging;

namespace Components.CommunicationServer
{
    /// <summary>
    /// Class for TCP/IP logic - receiving/sending messages
    /// </summary>
    public abstract class TcpServer
    {
        private TcpListener _tcpListener;
        private Thread _listenerThread;
        private Dictionary<TcpClient, Thread> _clients;

        public IPEndPoint EndPoint { get; set; }

        public static readonly int MessageSize = 40960;

        public int ClientsCount { get { return _clients.Count; } }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ipString">String with IPAddress</param>
        /// <param name="port">Listening Port</param>
        public TcpServer(string ipString, int port)
        {
            IPAddress ip = null;
            if (!IPAddress.TryParse(ipString, out ip))
            {
                throw new Exception("Wrong IP address");
            }

            EndPoint = new IPEndPoint(ip,port);
            _clients = new Dictionary<TcpClient, Thread>();
            _tcpListener = new TcpListener(EndPoint);
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="ipEndPoint"></param>
        public TcpServer(IPEndPoint ipEndPoint)
        {
            EndPoint = ipEndPoint;
            _clients = new Dictionary<TcpClient, Thread>();
            _tcpListener = new TcpListener(EndPoint);
        }

        /// <summary>
        /// Starts Server's listening thread
        /// </summary>
        public void Start()
        {
            _listenerThread = new Thread(ListenForClients);
            _listenerThread.Start();
        }

        /// <summary>
        /// Closes all existing connections
        /// </summary>
        public void Stop()
        {
            _tcpListener.Stop();
            if (_listenerThread != null && _listenerThread.ThreadState == ThreadState.Running)
                _listenerThread.Abort();

            var clients = new TcpClient[_clients.Count];
            _clients.Keys.CopyTo(clients,0);
            foreach (var client in clients)
            {
                RemoveClient(client);
            }
        }

        private void ListenForClients()
        {
            
                _tcpListener.Start();
                while (true)
                {
                    //blocks until a client has connected to the server
                    try
                    {
                        EventLogger.GetLog().Info("Czekanie na nowego klienta");
                        var client = _tcpListener.AcceptTcpClient();
                        AddClient(client);
                    }
                    catch (Exception)
                    {
                        EventLogger.GetLog().ErrorFormat("Błąd podczas podłączania klienta");
                        break;
                    }
                }
        }

        private void AddClient(TcpClient client)
        {
            //create a thread to handle communication 
            //with connected client
            client.NoDelay = true;
            client.Client.NoDelay = true;
            var clientThread = new Thread(ListenForClientMessages);
            _clients.Add(client, clientThread);
            clientThread.Start(client);

            EventLogger.GetLog().InfoFormat("Klient podłączony: {0}", client.Client.RemoteEndPoint);
        }

        protected virtual void RemoveClient(TcpClient client)
        {
            var clientThread = _clients[client];
            _clients.Remove(client);
            if (clientThread != null && clientThread.ThreadState == ThreadState.Running)
                clientThread.Abort();

            EventLogger.GetLog().InfoFormat("Klient rozłączony: {0}", client.Client.RemoteEndPoint);
        }

        private void ListenForClientMessages(object client)
        {
            var tcpClient = (TcpClient)client;
            var clientStream = tcpClient.GetStream();


            while (true)
            {
                
                var messageStream = new MemoryStream();
                var inBuffer = new byte[MessageSize];
                try
                {
                    if (clientStream.CanRead)
                    {
                        do
                        {
                            var bytes = clientStream.Read(inBuffer, 0, 4096);
                            messageStream.Write(inBuffer, 0, bytes);
                        } while (clientStream.DataAvailable);
                    }

                }
                catch (Exception ex)
                {
                    //TcpConnectionErrorEvent.Invoke(this,new TcpConnectionErrorEventArgs(tcpClient,ETcpAction.Receive, "Błąd odbierania wiadomości"));
                    EventLogger.GetLog().ErrorFormat("Błąd podczas odbierania wiadomosci od: {0}",tcpClient.Client.RemoteEndPoint);
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
                //message has successfully been received
                //EventLogger.GetLog().InfoFormat("Odebrano wiadomość od {0}:\n{1}", tcpClient.Client.RemoteEndPoint,Encoding.UTF8.GetString(completeMessage));
                ProcessMessage(completeMessage, tcpClient);
            }
            RemoveClient(tcpClient);
        }

        /// <summary>
        /// Sends a message to a client
        /// </summary>
        /// <param name="msg">byte array with message</param>
        /// <param name="client">Connected tcp client</param>
        public void SendMessage(byte[] msg, TcpClient client)
        {
            try
            {
                var clientStream = client.GetStream();

                if (clientStream.CanWrite)
                {
                    clientStream.Write(msg, 0, msg.Length);

                    clientStream.FlushAsync();
                }
                //EventLogger.GetLog().InfoFormat("Wysłano wiadomość do {0}:\n{1} ", client.Client.RemoteEndPoint,Encoding.UTF8.GetString(msg) );
            }
            catch (Exception)
            {
                EventLogger.GetLog().ErrorFormat("Błąd podczas wysyłania wiadomosci do: {0}", client.Client.RemoteEndPoint);
            }
        }

        /// <summary>
        /// Handle received message
        /// </summary>
        /// <param name="msg">Byte array with received message</param>
        /// <param name="sender">Reference to sender object</param>
        public abstract void ProcessMessage(byte[] msg, TcpClient sender);

    }
}
