using Alchemy.Classes;
using DPFP.Verification;
using Newtonsoft.Json;
using NLog;
using System;
using System.Text;

namespace DPReceiver
{
    public class FingerEnroll : IFinger
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        public DPFP.Processing.Enrollment enrollment;
        private readonly MainForm mainForm;

        public FingerEnroll(MainForm mainForm)
        {
            this.mainForm = mainForm;
            this.mainForm.capturehandler.FingerComplete += SetFeaturesEnrollment;
        }
         

        public void Dispose()
        {
            enrollment = null;
            this.mainForm.StopCapture(); 
        }

        public void Load()
        {
            Init();
            this.mainForm.InitCapture();
        }

        public void Init()
        {
            try
            {

                enrollment = new DPFP.Processing.Enrollment();

            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        public void SetFeaturesEnrollment(DPFP.Sample sample)
        {
            DPFP.FeatureSet featureSet = CreateFeatureSet(sample, DPFP.Processing.DataPurpose.Enrollment);
            if (featureSet != null)
            {
                try
                {
                    this.enrollment.AddFeatures(featureSet);
                }
                catch (Exception e)
                {
                    this.mainForm.SendMessage("Error when extracting fingerprint characteristics", ResponseType.Error);
                }
                finally
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("Tap Finger {0} left", this.enrollment.FeaturesNeeded);
                    this.mainForm.SendMessage(stringBuilder.ToString());

                    switch (this.enrollment.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:
                            this.mainForm.template = this.enrollment.Template;
                            this.mainForm.SendFingerData();
                            this.mainForm.StopCapture();
                            break;
                        case DPFP.Processing.Enrollment.Status.Failed:
                            this.enrollment.Clear();
                            this.mainForm.StopCapture();
                            this.mainForm.InitCapture();

                            this.mainForm.SendMessage("Finger enrollment failed", ResponseType.Error);
                            break;
                    }
                }
            }
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
