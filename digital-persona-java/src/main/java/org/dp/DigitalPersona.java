package org.dp;

import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.Configuration;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.SocketIOServer;
import com.corundumstudio.socketio.listener.DataListener;
import org.dp.listeners.SockListener;
import org.dp.models.SockModel;

import java.io.Console;
import java.util.Objects;
import java.util.Scanner;

public class DigitalPersona {

    public static void main(String[] args) {

        final Configuration configuration = new Configuration();
        configuration.setHostname("localhost");
        configuration.setPort(8081);

        final SocketIOServer server = new SocketIOServer(configuration);
        server.addEventListener("dp", SockModel.class, new SockListener());
        server.start();

        Scanner scanner = new Scanner(System.in);
        System.out.println("Type quit to exit program");

        while (!Objects.equals(scanner.nextLine(), "quit")) {
            System.out.println("Type quit to exit program");
        }

        server.stop();
    }

}
