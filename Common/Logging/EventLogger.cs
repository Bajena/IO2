using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Common.Logging
{

    public class EventLogger
    {
        private static readonly ILog Log =  LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog GetLog()
        {
            return Log;
        }

        public static void AddAppender(IAppender appender)
        {
            ((Logger)GetLog().Logger).AddAppender(appender);
        }
    }
}
