using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using SharpExtensionsUtil.Converter;
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
            host.Dispose();
            host = null;
            btnListen.Text = "слушать";
        }

        private void LogMessage(string str)
        {
            Invoke(new Action<string>(s => tbLog.AppendText(s + Environment.NewLine)), str);
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
            if (client == null)
            {
                client = new Client(tbHost.Text, LogMessage, OnMessageSafe);
                btnConnect.Text = "Стоп";
                return;
            }
            client.Dispose();
            client = null;
            btnConnect.Text = "Подключиться";
        }
    }

    class ServerHost : IDisposable
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

        public void Dispose()
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

    class Client : IDisposable
    {
        private WebSocketQuoteSource listener;

        private readonly Action<string> onConnectionEvent;

        public Client(string uri, Action<string> onConnectionEvent, Action<string> onMessage)
        {
            this.onConnectionEvent = onConnectionEvent;
            listener = new WebSocketQuoteSource(onMessage, OnConnectionEvent);
            listener.Connect(uri);
        }

        private void OnConnectionEvent(WebSocketQuoteSource.ConnectionEvent connectionEvent, string s, WebSocketQuoteSource arg3)
        {
            onConnectionEvent($"{connectionEvent}: [{s}]");
        }

        public void Dispose()
        {
            listener.Logout();
            listener = null;
        }
    }
}
