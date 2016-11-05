using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common.Models.Messages;
using Common.Enumerations;
using Components;
using Components.CommunicationServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TcpConnectionTests
    {
        // TEST
        private TcpServer _server;
        private string _ipAddress;
        private int _port;

        [TestInitialize]
        public void Initialize()
        {
            _ipAddress = "127.0.0.1";
            _port = 3000;
            _server = new CommunicationServer(_ipAddress,_port);
            _server.Start();
        }

        [TestMethod]
        public void ClientCanConnect()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress),_port);
            
            var client = new TcpClient();
            client.Connect(serverEndPoint);
            Thread.Sleep(2000);
            Assert.AreEqual(1,_server.ClientsCount);
            client.Close();
        }

        [TestMethod]
        public void ClientCanRegister()
        {
            Component component = new TaskManager();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);
            component.Register(endPoint);
            Thread.Sleep(2000);
            Assert.AreEqual(1, _server.ClientsCount);
        }

        [TestMethod]
        public void MultipleClientsCanConnect()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            var client = new TcpClient();
            var client2 = new TcpClient();
            client.Connect(serverEndPoint);
            client2.Connect(serverEndPoint);
            Thread.Sleep(2000);
            Assert.AreEqual(2, _server.ClientsCount);
            client.Close();
        }
        [TestMethod]
        public void ClientCanDisconnect()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            var client = new TcpClient();
            client.Connect(serverEndPoint);
            client.Close();
            Thread.Sleep(2000);
            Assert.AreEqual(0, _server.ClientsCount);
        }

        [TestMethod]
        public void MultipleClientCanDisconnect()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            var client = new TcpClient();
            var client2 = new TcpClient();
            client.Connect(serverEndPoint);
            client2.Connect(serverEndPoint);
            client.Close();
            client2.Close();
            Thread.Sleep(2000);
            Assert.AreEqual(0, _server.ClientsCount);
        }
        [TestMethod]
        public void ClientCanSendShortMessage()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            var client = new TcpClient();
            client.Connect(serverEndPoint);
            var message = Encoding.UTF8.GetBytes("message");

            client.Client.Send(message);
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void ClientCanSendLongMessage()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            var client = new TcpClient();
            client.Connect(serverEndPoint);
            var message = string.Empty;
            for (int i = 0; i < 8096; i++)
                message += "a";

            client.Client.Send(Encoding.UTF8.GetBytes(message));
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void ServerCanSendMessage()
        {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);

            var client = new TcpClient();
            client.Connect(serverEndPoint);
            var message = string.Empty;
            for (int i = 0; i < 8096; i++)
                message += "a";
            _server.SendMessage(Encoding.UTF8.GetBytes(message), client);
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void ClientCanGetId()
        {
            ComputationalClient component = new ComputationalClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            component.Register(endPoint);

            Thread.Sleep(5000);

            //po rejestracji komponent powinien dostac id od serwera (rowne 1 bo to pierwszy komponent)
            Assert.AreEqual(1u, component.Id);
            component.CloseConnection();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Stop();
        }
    }
}
