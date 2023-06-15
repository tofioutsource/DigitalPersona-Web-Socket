using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPReceiver
{
    public class FingerEnroll : IFinger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public DPFP.Processing.Enrollment enrollment;
        private DPFP.Capture.Capture capture;
        public DPFP.Template template;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            Init();
            InitCapture();
        }

        public void Init()
        {
            try
            {
                capture = new DPFP.Capture.Capture();

                if (capture != null)
                {
                    capture.EventHandler = new FingerEnrollHandler(this);
                    enrollment = new DPFP.Processing.Enrollment();
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
                }
                catch (Exception e)
                {
                    logger.Error(e, "Could not stop capture device");
                }
            }
        }
    }
}
