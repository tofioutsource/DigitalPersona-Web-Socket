using DPFP.Capture;
using DPFP;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy.Classes;

namespace DPReceiver
{
    public class FingerCapturehandler : DPFP.Capture.EventHandler
    { 
        private MainForm handler;

        public delegate void FingerCompleteHandler(Sample sample);
        public event FingerCompleteHandler FingerComplete;

        public FingerCapturehandler(MainForm handler)
        {

            this.handler = handler;

        }

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            // SetImagePawprint(Sample);
            DPFP.FeatureSet featureSet = CreateFeatureSet(Sample, DPFP.Processing.DataPurpose.Verification);

            if (featureSet != null)
            {
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();

                //List<User> ListUser = mySqlAdapter.GetAllUsers();

                //ListUser.ForEach(delegate (User user) {

                //using var ms = new MemoryStream(featureSet.Bytes);
                this.handler.template.DeSerialize(featureSet.Bytes);

                //    verification.Verify(featureSet, template, ref result);
                //    if (result.Verified)
                //    {
                //        MessageBox.Show("Hola: " + user.Name);
                //    }
                //});
                this.handler.SendMessage("Finger has been logged");
                FingerComplete?.Invoke(Sample);
                //this.handler.SendFingerData();
            }
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            this.handler.InitCapture();
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            this.handler.StopCapture();
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback)
        {

        }

        public DPFP.FeatureSet CreateFeatureSet(DPFP.Sample sample, DPFP.Processing.DataPurpose purpose)
        {
            DPFP.Processing.FeatureExtraction featureExtraction = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback captureFeedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet featureSet = new DPFP.FeatureSet();

            featureExtraction.CreateFeatureSet(sample, purpose, ref captureFeedback, ref featureSet);

            return (captureFeedback == DPFP.Capture.CaptureFeedback.Good) ? featureSet : null;
        }

    }
}
