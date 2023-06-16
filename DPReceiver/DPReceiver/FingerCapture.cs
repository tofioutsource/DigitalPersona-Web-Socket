using NLog;
using System;
using Alchemy.Classes;

namespace DPReceiver
{
    public class FingerCapture : FingerBase, IFinger
    { 
        private DPFP.Verification.Verification verification;
          
        public FingerCapture(UserContext context): base(context)
        { }


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
                SendMessage(e.Message); //logger.Error(e);
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
            capture?.Dispose();
        }
    }
}
