package org.dp;

import com.corundumstudio.socketio.Configuration;
import com.corundumstudio.socketio.SocketIOServer;
import org.dp.listeners.CaptureController;
import org.dp.listeners.ReaderController;
import org.dp.models.CaptureModel;
import org.dp.models.DPModel;
import org.dp.models.ReaderModel;

import java.util.Objects;
import java.util.Scanner;

public class DigitalPersona {

    private static final DPModel dpModel = new DPModel();

    public static void main(String[] args) {

        final Configuration configuration = new Configuration();
        configuration.setHostname("localhost");
        configuration.setPort(9092);

        final SocketIOServer server = new SocketIOServer(configuration);
        server.addEventListener("reader", ReaderModel.class, new ReaderController(server, dpModel));
        server.addEventListener("capture", CaptureModel.class, new CaptureController(server, dpModel));
        server.start();

        //UareUGlobal.DestroyReaderCollection();

        Scanner scanner = new Scanner(System.in);
        System.out.println("Type quit to exit program");

        while (!Objects.equals(scanner.nextLine(), "quit")) {
            System.out.println("Type quit to exit program");
        }

        server.stop();
    }

}
