using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Logging;
using Components.CommunicationServer;
using log4net.Appender;
using log4net.Core;

namespace ServerConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {  
            EventLogger.AddAppender(new Log4NetAppender());
            var ipAddress = "127.0.0.1";
            var port = 8123;
            var server = new CommunicationServer(ipAddress, port);
            server.Start();
        }

    }
}
