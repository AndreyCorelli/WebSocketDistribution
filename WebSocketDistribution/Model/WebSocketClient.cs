using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace WebSocketDistribution.Model
{
    public class WebSocketClient
    {
        private WebSocket websocketClient;

        private string url;
        private string protocol;
        private WebSocketVersion version;

        public void Setup(string url, string protocol, WebSocketVersion version)
        {
            this.url = url;
            this.protocol = protocol;
            this.version = WebSocketVersion.Rfc6455;

            websocketClient = new WebSocket(this.url, this.protocol, this.version);

            websocketClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocketClient_Error);
            websocketClient.Opened += new EventHandler(websocketClient_Opened);
            websocketClient.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocketClient_MessageReceived);
        }

        public void Start()
        {
            websocketClient.Open();
            websocketClient.Send("Hello world");
        }

        private void Stop()
        {
            websocketClient.Close();

        }

        private void websocketClient_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Client successfully connected.");

//            websocketClient.Send("Hello World!");

        }

        private void websocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.WriteLine("Message Received. Server answered: " + e.Message);
        }

        private void websocketClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception.GetType() + ": " + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);

            if (e.Exception.InnerException != null)
            {
                Debug.WriteLine(e.Exception.InnerException.GetType());
            }

            return;
        }
    }
}
