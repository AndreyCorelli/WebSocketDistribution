using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using SharpExtensionsUtil.Converter;
using WebSocket4Net;
using WebSocketDistribution.Model;

namespace WebSocketGUI
{
    public partial class MainForm : Form
    {
        private ServerHost host;

        private Client client;

        private volatile int messagesCount;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            if (host == null)
            {
                host = new ServerHost(tbPort.Text.ToIntSafe() ?? 19006, LogMessage, OnMessageSafe);
                btnListen.Text = "Стоп";
                return;
            }
            host.Stop();
            host = null;
            btnListen.Text = "слушать";
        }

        private void LogMessage(string str)
        {
            try
            {
                Invoke(new Action<string>(s => tbLog.AppendText(s + Environment.NewLine)), str);
            }
            catch
            {
            }
        }

        private void OnMessageSafe(string msg)
        {
            Invoke(new Action<string>(OnMessageUnsafe), msg);
        }

        private void OnMessageUnsafe(string msg)
        {
            messagesCount++;
            lblLastMsgTime.Text = DateTime.Now.ToString($"HH:mm:ss");
            lblMessagesCount.Text = messagesCount.ToString();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (host != null)
            {
                LogMessage($"Подключено {host.SessionsCount()} клиентов");
                return;
            }

            if (client == null)
            {
                client = new Client(tbHost.Text, LogMessage, OnMessageSafe);
                btnConnect.Text = "Стоп";
                return;
            }
            client.Stop();
            client = null;
            btnConnect.Text = "подключиться";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            host?.Stop();
            client?.Stop();
        }
    }

    class ServerHost
    {
        private readonly WebSocketDistributor distributor;

        private readonly Thread distrThread;

        private volatile bool isStopping;

        private readonly Random rand = new Random();

        private long seed = 1;

        public ServerHost(int port, Action<string> onMessage, Action<string> onMsgReceived)
        {
            distributor = new WebSocketDistributor(port, 100);
            distributor.logMessage += onMessage;
            distributor.onMsgReceived += onMsgReceived;
            distributor.onClientConnected += onMessage;
            distributor.onClientDisconnected += onMessage;
            distributor.Start();
            distrThread = new Thread(ThreadFunc);
            distrThread.Start();
        }

        public int SessionsCount()
        {
            return distributor?.SessionsActive ?? 0;
        }

        public void Stop()
        {
            isStopping = true;
            distrThread.Join();
            distributor.Stop();
        }

        private void ThreadFunc()
        {
            while (!isStopping)
            {
                Thread.Sleep(200);

                var count = rand.Next(0, 4);
                if (count == 0) continue;

                var messages = new List<string>(count);
                for (var i = 0; i < count; i++)
                    messages.Add($"EURSGD 1.2234 1.2239 1.2201 1.2238 {seed++}");

                distributor.EnqueueQuotes(messages);
            }
        }
    }

    class Client
    {
        private WebSocketClient listener;

        private readonly Action<string> onConnectionEvent;

        public Client(string uri, Action<string> onConnectionEvent, Action<string> onMessage)
        {
            this.onConnectionEvent = onConnectionEvent;
            listener = new WebSocketClient();
            listener.Setup(uri, "basic", WebSocketVersion.Rfc6455, onMessage, OnConnectionEvent);
            listener.Start();
        }

        private void OnConnectionEvent(ConnectionEvent connectionEvent, string s)
        {
            onConnectionEvent($"{connectionEvent}: [{s}]");
        }

        public void Stop()
        {
            listener.Stop();
            listener = null;
        }
    }
}
