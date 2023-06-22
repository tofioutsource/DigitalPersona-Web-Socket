package org.dp.models;

import com.corundumstudio.socketio.SocketIOClient;
import com.digitalpersona.uareu.ReaderCollection;

public class DPModel {
    private SocketIOClient socketIOClient;
    private ReaderCollection readerCollection;

    public SocketIOClient getSocketIOClient() {
        return socketIOClient;
    }

    public void setSocketIOClient(SocketIOClient socketIOClient) {
        this.socketIOClient = socketIOClient;
    }

    public ReaderCollection getReaderCollection() {
        return readerCollection;
    }

    public void setReaderCollection(ReaderCollection readerCollection) {
        this.readerCollection = readerCollection;
    }


}
