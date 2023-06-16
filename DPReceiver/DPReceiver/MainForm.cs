using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;
using NLog;
using System;
using System.Configuration;
using System.Net;
using System.Windows.Forms;

namespace DPReceiver
{
    public partial class MainForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private WebSocketServer Server;
        private IFinger mForm;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
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
            this.label1.Text = $"ws://localhost:{port}";
        }

        public void OnConnect(UserContext context)
        {
            Console.WriteLine("Client Connection From : " + context.ClientAddress);
        }

        public void OnReceive(UserContext context)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { OnReceive(context); }));
                return;
            }

            Console.WriteLine("Received Data From :" + context.ClientAddress);

            try
            {
                var json = context.DataFrame.ToString();

                // <3 dynamics
                dynamic obj = JsonConvert.DeserializeObject(json);

                logger.Info(json);

                if (obj == null) return;
                if (obj is int || obj is long) return;

                switch ((CommandType)obj.Type)
                {
                    case CommandType.TapFinger:
                        if (mForm != null)
                        {
                            mForm.Dispose();
                            mForm = null;
                        }

                        mForm = new FingerCapture(context);
                        mForm.Load();
                        break;
                    case CommandType.Register:
                        if (mForm != null)
                        {
                            mForm.Dispose();
                            mForm = null;
                        }

                        mForm = new FingerEnroll(context);
                        mForm.Load();
                        break; 
                }
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                var r = new AppResponse { Type = ResponseType.Error, Data = new { e.Message } };

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

        public enum CommandType
        {
            TapFinger = 1,
            Register
        }
    }
}
