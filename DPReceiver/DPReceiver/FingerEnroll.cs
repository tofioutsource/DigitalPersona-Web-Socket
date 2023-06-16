using Alchemy.Classes;
using Newtonsoft.Json;
using NLog;
using System;

namespace DPReceiver
{
    public class FingerEnroll : FingerBase, IFinger
    {
       
        public DPFP.Processing.Enrollment enrollment;
        
         
        public FingerEnroll(UserContext context): base(context) { }
          
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
         
    }
}
