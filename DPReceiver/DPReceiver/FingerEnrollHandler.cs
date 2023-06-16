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
    [Obsolete]
    public class FingerEnrollHandler : DPFP.Capture.EventHandler
    {
        private FingerEnroll handler;

        public FingerEnrollHandler(FingerEnroll handler)
        {

            this.handler = handler;

        }

        public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
        {
            //SetFeaturesEnrollment(Sample);
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

       
      
    }
}
