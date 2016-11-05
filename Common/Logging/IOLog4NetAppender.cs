using System;
using log4net.Appender;
using log4net.Core;

namespace Common.Logging
{
    public class Log4NetAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                case "INFO":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "WARN":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "ERROR":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "FATAL":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }
            Console.WriteLine("(Wątek:{0}) [{1}] - {2}", loggingEvent.ThreadName, loggingEvent.TimeStamp, loggingEvent.RenderedMessage);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

    }
}

