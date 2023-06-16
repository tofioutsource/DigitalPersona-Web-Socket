using NLog;
using System;
using Alchemy.Classes;

namespace DPReceiver
{
    public class FingerCapture : IFinger
    {
        private DPFP.Verification.Verification verification;
        private readonly MainForm mainForm;

        public FingerCapture(MainForm mainForm)
        {
            this.mainForm = mainForm;
            this.mainForm.capturehandler.FingerComplete += Capturehandler_FingerComplete;    
        }

        private void Capturehandler_FingerComplete(DPFP.Sample sample)
        {
            this.mainForm.SendFingerData();
        }

        public void Init()
        {
            try
            {

                verification = new DPFP.Verification.Verification();

            }
            catch (Exception e)
            {
                this.mainForm.SendMessage(e.Message); //logger.Error(e);
            }
        }


        public void Load()
        {
            Init();
            this.mainForm.InitCapture();
        }

        public void Dispose()
        {
            verification = null;
            this.mainForm.StopCapture();
        }
    }
}
