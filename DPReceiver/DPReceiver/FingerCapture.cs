using DPFP.Capture;
using DPFP;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPReceiver
{
    public class FingerCapture : IFinger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DPFP.Verification.Verification verification;
        private DPFP.Capture.Capture capture;
        private DPFP.Template template;
         

        public void Init()
        {
            try
            {
                capture = new DPFP.Capture.Capture();

                if (capture != null)
                {
                    capture.EventHandler = new FingerCapturehandler(this);
                    template = new DPFP.Template();
                    verification = new DPFP.Verification.Verification();
                }
                else
                {
                    throw new Exception("Could not start capturing interface");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        public void InitCapture()
        {
            if (capture != null)
            {
                try
                {

                    capture.StartCapture();
                    logger.Info("Capturing");
                }
                catch (Exception e)
                {
                    logger.Error(e, "Could not start capture device");
                }
            }
        }

        public void StopCapture()
        {
            if (capture != null)
            {
                try
                {
                    capture.StopCapture();
                    logger.Info("Capturing Stopped");
                }
                catch (Exception e)
                {
                    logger.Error(e, "Could not stop capture device");
                }
            }
        }

    
        public void Load()
        {
            Init();
            InitCapture();
        }

        public void Dispose()
        {
            StopCapture();
        }
    }
}
