using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPReceiver
{
    [Obsolete]
    public class AppProcessor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private WebSocketServer Server;
        private Form mForm;

        public AppProcessor()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Port"], out int port))
            {
                logger.Warn("Configuration setting for Port could not be read, setting to a default value (8081)");
                port = 8081;
            }

            Server = new WebSocketServer(port, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };

            Server.Start();
        }

        public void OnConnect(UserContext context)
        {
            Console.WriteLine("Client Connection From : " + context.ClientAddress);
        }

        public void OnReceive(UserContext context)
        {
            Console.WriteLine("Received Data From :" + context.ClientAddress);

            try
            {
                var json = context.DataFrame.ToString();

                // <3 dynamics
                dynamic obj = JsonConvert.DeserializeObject(json);

                logger.Info(json);
                switch ((int)obj.Type)
                {
                    case (int)CommandType.TapFinger:
                        if (mForm != null)
                        {
                            mForm.Close();
                            mForm.Dispose();
                            mForm = null;
                        }

                        //mForm = new FingerCapture();
                        //mForm.Show();
                        break;
                        //case (int)CommandType.Message:
                        //    ChatMessage(obj.Message.Value, context);
                        //    break;
                        //case (int)CommandType.NameChange:
                        //    NameChange(obj.Name.Value, context);
                        //    break;
                }
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                var r = new Response { Type = ResponseType.Error, Data = new { e.Message } };

                context.Send(JsonConvert.SerializeObject(r));
            }
        }

        public void OnSend(UserContext context)
        {
            Console.WriteLine("Data Send To : " + context.ClientAddress);
        }

        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnDisconnect(UserContext context)
        {
            Console.WriteLine("Client Disconnected : " + context.ClientAddress);
            //var user = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();

            //string trash; // Concurrent dictionaries make things weird

            //OnlineUsers.TryRemove(user, out trash);

            //if (!String.IsNullOrEmpty(user.Name))
            //{
            //    var r = new Response { Type = ResponseType.Disconnect, Data = new { user.Name } };

            //    Broadcast(JsonConvert.SerializeObject(r));
            //}

            //BroadcastNameList();
        }

        public enum ResponseType
        {
            Connection = 0,
            Disconnect = 1,
            Message = 2,
            NameChange = 3,
            UserCount = 4,
            Error = 255
        }

        /// <summary>
        /// Defines the response object to send back to the client
        /// </summary>
        public class Response
        {
            public ResponseType Type { get; set; }
            public dynamic Data { get; set; }
        }

        public enum CommandType
        {
            TapFinger = 1,
            Register,
            Message,
            NameChange
        }
    }
}
