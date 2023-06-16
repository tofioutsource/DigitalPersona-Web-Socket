using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPReceiver
{
    public class FingerEnrollHandler : DPFP.Capture.EventHandler
    {
        private FingerEnroll handler;

        public FingerEnrollHandler(FingerEnroll handler)
        {

            this.handler = handler;

        }

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            SetFeaturesEnrollment(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback)
        {

        }

        //public void SetImagePawprint(DPFP.Sample sample)
        //{
        //    DPFP.Capture.SampleConversion converter = new DPFP.Capture.SampleConversion();
        //    Bitmap bitMap = null;

        //    converter.ConvertToPicture(sample, ref bitMap);
        //    imagePawprint.Image = bitMap;
        //}

        public DPFP.FeatureSet CreateFeatureSet(DPFP.Sample sample, DPFP.Processing.DataPurpose purpose)
        {
            DPFP.Processing.FeatureExtraction featureExtraction = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback captureFeedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet featureSet = new DPFP.FeatureSet();

            featureExtraction.CreateFeatureSet(sample, purpose, ref captureFeedback, ref featureSet);

            return (captureFeedback == DPFP.Capture.CaptureFeedback.Good) ? featureSet : null;
        }


        public void SetFeaturesEnrollment(DPFP.Sample sample)
        {
            DPFP.FeatureSet featureSet = CreateFeatureSet(sample, DPFP.Processing.DataPurpose.Enrollment);
            if (featureSet != null)
            {
                try
                {
                    this.handler.enrollment.AddFeatures(featureSet);
                }
                catch (Exception e)
                { 
                    this.handler.SendMessage("Error when extracting fingerprint characteristics", ResponseType.Error);
                }
                finally
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("Tap Finger {0} left", this.handler.enrollment.FeaturesNeeded);
                    this.handler.SendMessage(stringBuilder.ToString());

                    switch (this.handler.enrollment.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:
                            this.handler.template = this.handler.enrollment.Template;
                            this.handler.SendFingerData();
                            this.handler.StopCapture();
                            break;
                        case DPFP.Processing.Enrollment.Status.Failed:
                            this.handler.enrollment.Clear();
                            this.handler.StopCapture();
                            this.handler.InitCapture();

                            this.handler.SendMessage("Finger enrollment failed", ResponseType.Error);
                            break;
                    }
                }
            }
        }
    }
}
