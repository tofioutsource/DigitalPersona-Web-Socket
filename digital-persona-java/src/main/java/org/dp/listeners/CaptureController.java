package org.dp.listeners;

import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.SocketIOServer;
import com.corundumstudio.socketio.listener.DataListener;
import com.digitalpersona.javapos.services.biometrics.CaptureThread;
import com.digitalpersona.uareu.Fid;
import com.digitalpersona.uareu.Reader;
import org.dp.DPHelper;
import org.dp.models.CaptureModel;
import org.dp.models.DPModel;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class CaptureController implements DataListener<CaptureModel>  {

    private final SocketIOServer server;
    private final DPModel dpModel;

    private CaptureThread mCapture;
    private Reader mReader;

    public CaptureController(SocketIOServer server, DPModel dpModel) {
        this.server = server;
        this.dpModel = dpModel;
    }

    private void startCaptureThread() {
        mCapture = new CaptureThread(mReader, false, Fid.Format.ANSI_381_2004, Reader.ImageProcessing.IMG_PROC_DEFAULT);
        mCapture.start(new DPCaptureHandler(this.server));
    }

    private void stopCaptureThread() {
        if (null != mCapture) mCapture.cancel();
    }

    private void waitForCaptureThread() {
        if (null != mCapture) mCapture.join(1000);
    }

    @Override
    public void onData(SocketIOClient socketIOClient, CaptureModel captureModel, AckRequest ackRequest) throws Exception {
        try {
            this.mReader = this.dpModel.getReaderCollection().stream().findFirst()
                    .orElseThrow(() -> new Throwable("Reader is not present, please connect device"));

            mReader.Open(Reader.Priority.COOPERATIVE);
            startCaptureThread();

            this.server.getBroadcastOperations().sendEvent("message", "Device Ready, tap finger");
        } catch (Throwable e) {
            this.server.getBroadcastOperations().sendEvent("error", e.getMessage());
        }
    }

    private class DPCaptureHandler implements ActionListener {

        private final SocketIOServer server;

        public DPCaptureHandler(SocketIOServer server){
            this.server = server;
        }

        @Override
        public void actionPerformed(ActionEvent e) {
            if (e.getActionCommand().equals(CaptureThread.ACT_CAPTURE)) {
                //event from capture thread
                CaptureThread.CaptureEvent evt = (CaptureThread.CaptureEvent) e;
                boolean bCanceled = false;

                if (null != evt.capture_result) {
                    if (null != evt.capture_result.image && Reader.CaptureQuality.GOOD == evt.capture_result.quality) {
                        //display image
                        Fid image = evt.capture_result.image;
                        String img = DPHelper.imageToBase64(image);
                        this.server.getBroadcastOperations().sendEvent("fingerData", img);
                    } else if (Reader.CaptureQuality.CANCELED == evt.capture_result.quality) {
                        //capture or streaming was canceled, just quit
                        bCanceled = true;
                    } else {
                        //bad quality
                        this.server.getBroadcastOperations().sendEvent("error", "Bad Quality: %s".formatted(evt.capture_result.quality));
                    }
                } else if (null != evt.exception) {
                    //exception during capture
                    this.server.getBroadcastOperations().sendEvent("error", "Capture: %s".formatted(evt.exception));
                    bCanceled = true;
                } else if (null != evt.reader_status) {
                    this.server.getBroadcastOperations().sendEvent("error", "Bad Status: %s".formatted(evt.reader_status));
                    bCanceled = true;
                }

                if (!bCanceled) {
                    //restart capture thread
                    waitForCaptureThread();
                    startCaptureThread();
                }
            }
        }
    }
}
