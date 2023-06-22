package org.dp.listeners;

import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.SocketIOServer;
import com.corundumstudio.socketio.listener.DataListener;
import com.digitalpersona.uareu.UareUGlobal;
import org.dp.models.DPModel;
import org.dp.models.DPReaderModel;
import org.dp.models.ReaderModel;

import java.util.List;

public class ReaderController implements DataListener<ReaderModel> {

    private final SocketIOServer server;
    private final DPModel dpModel;
   public ReaderController(SocketIOServer server, DPModel dpModel) {
       this.server = server;
       this.dpModel = dpModel;
    }

    @Override
    public void onData(SocketIOClient socketIOClient, ReaderModel readerModel, AckRequest ackRequest) throws Exception {
       try {
           this.dpModel.setSocketIOClient(socketIOClient);
           this.dpModel.setReaderCollection(UareUGlobal.GetReaderCollection());

           this.dpModel.getReaderCollection().GetReaders();

           final List<DPReaderModel> readers = this.dpModel.getReaderCollection().stream()
                   .map(x -> new DPReaderModel(x.GetDescription().id, x.GetDescription().name)).toList();

           this.server.getBroadcastOperations().sendEvent("readers", readers);
       }catch (Exception ex){
           this.server.getBroadcastOperations().sendEvent("error", ex.getMessage());
       }
    }
}
