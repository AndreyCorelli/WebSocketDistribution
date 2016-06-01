using System;
using System.Diagnostics;
using System.Threading;
using WebSocket4Net;

namespace WebSocketDistribution.Model
{
    public class WebSocketClient
    {
        private WebSocket websocketClient;

        private string url;

        private string protocol;

        private WebSocketVersion version;

        private Action<string> onMessage;

        private Action<ConnectionEvent, string> onEvent;

        private volatile bool isStopping;

        public void Setup(string url, string protocol, WebSocketVersion version,
            Action<string> onMessage, Action<ConnectionEvent, string> onEvent)
        {
            this.url = url;
            this.protocol = protocol;
            this.version = version;
            this.onMessage += onMessage;
            this.onEvent += onEvent;

            websocketClient = new WebSocket(this.url, this.protocol, this.version);

            websocketClient.Error += websocketClient_Error;
            websocketClient.Opened += websocketClient_Opened;
            websocketClient.MessageReceived += websocketClient_MessageReceived;
            websocketClient.Closed += websocketClient_Closed;
        }

        public void Start()
        {
            websocketClient.Open();
        }

        public void Stop()
        {
            isStopping = true;
            websocketClient.Close();
        }

        private void Restart()
        {
            Debug.WriteLine("Client restart.");
            websocketClient.Error -= websocketClient_Error;
            websocketClient.Opened -= websocketClient_Opened;
            websocketClient.MessageReceived -= websocketClient_MessageReceived;
            websocketClient.Closed -= websocketClient_Closed;
            websocketClient.Close();
            Thread.Sleep(1000);
            websocketClient = new WebSocket(url, protocol, version);

            websocketClient.Error += websocketClient_Error;
            websocketClient.Opened += websocketClient_Opened;
            websocketClient.MessageReceived += (sender, args) => onMessage(args.Message);
            websocketClient.Closed += websocketClient_Closed;
            Start();
        }

        private void websocketClient_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Client successfully connected.");
            onEvent(ConnectionEvent.Connected, "connected");
        }

        private void websocketClient_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Client closed");
            onEvent(ConnectionEvent.Disconnected, "closed");
            if (!isStopping)
                Restart();
        }

        private void websocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            onMessage(e.Message);
        }

        private void websocketClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            onEvent(ConnectionEvent.Faulted, e.Exception.Message);
            Debug.WriteLine(e.Exception.GetType() + ": " + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);

            if (e.Exception.InnerException != null)
            {
                Debug.WriteLine(e.Exception.InnerException.GetType());
            }
            if (!isStopping)
                Restart();
        }
    }
}
