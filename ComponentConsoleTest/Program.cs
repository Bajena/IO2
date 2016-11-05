using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Common.Abstractions;
using Common.Models.Messages;
using Components;
using DllProject;

namespace ComponentConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLogger.AddAppender(new Log4NetAppender());
            var ipAddress = "127.0.0.1";
            var port = 8123;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            Console.WriteLine("Choose component: \n"
                    + "1            ComputationalClient\n"
                    + "2            TaskManager\n"
                    + "3            ComputationalNode\n");

            Console.WriteLine("Select: ");
            int componentType;
            int.TryParse(Console.ReadLine(), out componentType);

            Component component = new ComputationalClient();
            switch (componentType)
            {
                case 1:
                    component = new ComputationalClient();
                    break;
                case 2:
                    component = new TaskManager();
                    break;
                case 3:
                    component = new ComputationalNode();
                    break;
            }

            component.SolvableProblems = new List<string>() { "MultiplyProblem", "DVRP" };
            component.Register(endPoint);

            var filePath = @"DvrpData\okulD.vrp";
            var problem = new DvrpProblem(new DvrpProblemData(filePath));
            //var problem = new MultiplyProblem(10, 3, 1000000);
            while (true)
            {
                Common.Abstractions.Message msg = null;
                int result;
                Console.WriteLine("RegisterMessage was sent\n"
                    + "Choose another message: \n"
                    + "1            RegisterMessage\n"
                    + "2            RegisterResponseMessage\n"
                    + "3            StatusMessage\n"
                    + "4            SolveRequestMessage\n"
                    + "5            SolveRequestResponseMessage\n"
                    + "6            DivideProblemMessage\n"
                    + "7            SolutionRequestMessage\n"
                    + "8            PartialProblemsMessage\n"
                    + "9            Solutions message\n"
                    + "10           Exit"
                    );

                Console.WriteLine("Choose message to send: ");
                if (int.TryParse(Console.ReadLine(), out result) == false || result < 1 || result > 10)
                {
                    Console.WriteLine("\nWrong input\n\n");
                    continue;
                }

                switch (result)
                { 
                    case 1:
                        msg = new RegisterMessage(component.Type,0,component.SolvableProblems);
                        break;
                    case 2:
                        msg = new RegisterResponseMessage(123L, DateTime.Now);
                        break;
                    case 3:
                        msg = new StatusMessage(123L, null);
                        break;
                    case 4:
                        msg = new SolveRequestMessage(problem.ProblemType, problem.SolvingTimeout, problem.Data);
                        break;
                    case 5:
                        msg = new SolveRequestResponseMessage(123L);
                        break;
                    case 6:
                        msg = new DivideProblemMessage("Problem type", 123L, Encoding.UTF8.GetBytes("test1"), 321L); 
                        break;
                    case 7:
                        msg = new SolutionRequestMessage(123L);
                        break;
                    case 8:
                        msg = new SolvePartialProblemsMessage("problem type", 123L, Encoding.UTF8.GetBytes("test1"), 333L, null);
                        break;
                    case 9:
                        msg = new SolutionsMessage("problemy type", 123L, Encoding.UTF8.GetBytes("test1"), null);
                        break;
                    case 10:
                        Environment.Exit(0);
                        break;
                }

                component.SendMessage(msg.Serialize());
                
            }
            
            //component.SendMessage(Encoding.UTF8.GetBytes("dupa"));
            
           
        }
    }
}
