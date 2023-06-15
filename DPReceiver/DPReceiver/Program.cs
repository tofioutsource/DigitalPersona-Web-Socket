using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DPReceiver
{
    internal class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main()
        { 

            var servicesToRun = new ServiceBase[]
            {
                FingerProcessor.Instance
            };
            if (Environment.UserInteractive)
            {
                RunInteractive(servicesToRun);
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }

        private static void RunInteractive(ServiceBase[] servicesToRun)
        {
            logger.Info("Running in interactive mode.");
            logger.Info(string.Empty);
            var onStartMethod = typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            logger.Info($"Starting {servicesToRun[0].ServiceName}...");
            onStartMethod.Invoke(servicesToRun[0], new object[] { new string[] { } });
            logger.Info("Started.");
            logger.Info(string.Empty);
            logger.Info("Press any key to stop the service...");
            Console.ReadKey(true);
            logger.Info(string.Empty);
            var onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            logger.Info($"Stopping {servicesToRun[0].ServiceName}...");
            onStopMethod.Invoke(servicesToRun[0], null);
            logger.Info("Stopped.");
            logger.Info(string.Empty);
            logger.Info("Press any key...");
            Console.ReadKey(true);
        }
    }
}
