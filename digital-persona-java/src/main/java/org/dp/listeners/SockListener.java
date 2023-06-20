package org.dp.listeners;

import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.listener.DataListener;
import org.dp.models.SockModel;

public class SockListener implements DataListener<SockModel> {
    @Override
    public void onData(SocketIOClient socketIOClient, SockModel sockModel, AckRequest ackRequest) throws Exception {

    }
}
