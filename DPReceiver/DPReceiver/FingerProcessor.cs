using NLog;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace DPReceiver
{
    partial class FingerProcessor : ServiceBase
    {
        private static FingerProcessor instance = null;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _isServiceRunning = true;
         
        private IDisposable _server = null;

        public FingerProcessor()
        {
            InitializeComponent();
        }

        public static FingerProcessor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FingerProcessor();
                }
                return instance;
            }
        }

        protected override void OnStart(string[] args)
        {
            Task.Factory.StartNew(RunProcessing, TaskCreationOptions.LongRunning);

            //var apiUrl = $"http://localhost:{ConfigurationManager.AppSettings["ApiPort"]}/";
            //_server = WebApp.Start<ApiStartup>(url: apiUrl);

            //logger.Info($"Status API listening at {apiUrl}");
        }

        protected override void OnStop()
        {
            try
            {
                _isServiceRunning = false; 
                _server.Dispose(); 
                base.OnStop();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unexpected exception on stopping the service!");
            }
        }

        private void RunProcessing()
        {
            if (!Int32.TryParse(ConfigurationManager.AppSettings["ProcessDelayMs"], out int processingDelayMs))
            {
                logger.Warn("Configuration setting for ProcessDelayMs could not be read, setting to a default value (100ms)");
                processingDelayMs = 100;
            }

            while (_isServiceRunning)
            {
                bool isException = false;
                try
                {
                    
                }
                catch (Exception ex)
                {
                    isException = true;
                    logger.Error(ex, "An exception occured!");
                }
               
                var delayPeriod = isException ? 5 * processingDelayMs : processingDelayMs;
                Thread.Sleep(delayPeriod);
            }
        }
    }
}
