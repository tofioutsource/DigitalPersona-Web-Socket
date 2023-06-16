using Alchemy.Classes;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPReceiver
{
    public abstract class FingerBase
    { 
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        public DPFP.Template template;
        protected UserContext context;
        protected DPFP.Capture.Capture capture;

        protected FingerBase(UserContext context)
        {
            this.context = context;
        }

        public void InitCapture()
        {
            if (capture != null)
            {
                try
                {

                    capture.StartCapture();
                    SendMessage("Capturing");
                }
                catch (Exception e)
                {
                    SendMessage(e.Message, ResponseType.Error);
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
                    SendMessage("Capturing Stopped");
                }
                catch (Exception e)
                {
                    SendMessage(e.Message, ResponseType.Error);
                }
            }
        }

        public void SendMessage(string message, ResponseType responseType = ResponseType.Message)
        {
            var r = new AppResponse { Type = responseType, Data = new { Message = message } };
            this.context.Send(JsonConvert.SerializeObject(r));

            if (responseType == ResponseType.Message)
                logger.Info(message);
            else if (responseType == ResponseType.Error)
                logger.Error(message);
        }

        public void SendFingerData()
        {
            var r = new AppResponse { Type = ResponseType.FingerData, Data = this.template.Bytes };
            this.context.Send(JsonConvert.SerializeObject(r));
        }
    }
}
