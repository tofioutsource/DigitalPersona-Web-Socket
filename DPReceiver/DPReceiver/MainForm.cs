using Alchemy;
using Alchemy.Classes;
using DPFP.Verification;
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
        public FingerCapturehandler capturehandler;

        public DPFP.Template template;
        protected UserContext context;
        protected DPFP.Capture.Capture capture;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            try
            {
                capture = new DPFP.Capture.Capture();

                if (capture != null)
                {
                    capturehandler = new FingerCapturehandler(this);
                    capture.EventHandler = capturehandler; 
                    template = new DPFP.Template(); 
                }
                else
                {
                    throw new Exception("Could not start capturing interface");
                }

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
            catch (Exception ex)
            {
                SendMessage(ex.Message); //logger.Error(e);
            }
        }

        private void Handler_OnMessage(string message)
        {
            logger.Info(message);
        }

        public void OnConnect(UserContext context)
        {
            this.context = context;
            Console.WriteLine("Client Connection From : " + context.ClientAddress);
        }

        public void OnReceive(UserContext context)
        {
            //if (this.InvokeRequired)
            //{
            //    this.Invoke(new MethodInvoker(delegate { OnReceive(context); }));
            //    return;
            //}

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

                        mForm = new FingerCapture(this);
                        mForm.Load();
                        break;
                    case CommandType.Register:
                        if (mForm != null)
                        {
                            mForm.Dispose();
                            mForm = null;
                        }

                        mForm = new FingerEnroll(this);
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
            this.mForm?.Dispose();
        }

        public enum CommandType
        {
            TapFinger = 1,
            Register
        }

        public void InitCapture()
        {
            if (capture != null)
            {
                try
                {

                    capture.StartCapture();
                    SendMessage("Capturing");
                }
                catch (Exception e)
                {
                    SendMessage(e.Message, ResponseType.Error);
                }
            }
        }

        public void StopCapture()
        {
            if (capture != null)
            {
                try
                {
                    capture.StopCapture();
                    SendMessage("Capturing Stopped");
                }
                catch (Exception e)
                {
                    SendMessage(e.Message, ResponseType.Error);
                }
            }
        }

        public void SendMessage(string message, ResponseType responseType = ResponseType.Message)
        {
            var r = new AppResponse { Type = responseType, Data = new { Message = message } };
            this.context?.Send(JsonConvert.SerializeObject(r));

            if (responseType == ResponseType.Message)
                logger.Info(message);
            else if (responseType == ResponseType.Error)
                logger.Error(message);
        }

        public void SendFingerData()
        {
            var r = new AppResponse { Type = ResponseType.FingerData, Data = this.template.Bytes };
            this.context?.Send(JsonConvert.SerializeObject(r));
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            Console.WriteLine("1111");
        }
    }
}
