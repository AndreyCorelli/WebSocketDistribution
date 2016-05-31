namespace WebSocketDistribution.Model
{
    public enum ConnectionEvent
    {
        Connected = 0,
        Disconnected,
        Faulted
    }

    //public class WebSocketQuoteSource
    //{
    //    private ClientWebSocket webSocket;

    //    private static readonly UTF8Encoding encoding = new UTF8Encoding();

    //    private readonly Action<string> onMessage;

    //    private readonly Action<ConnectionEvent, string, WebSocketQuoteSource> onConnectionEvent;

    //    private readonly ArraySegment<byte> bufferReceived;

    //    private readonly byte[] dataReceived = new byte[4096];

    //    private StringBuilder messageCollected = new StringBuilder();

    //    private volatile bool loggedOut;

    //    private string uriStr;

    //    public string LoginMessage { get; set; }

    //    public string LogoutMessage { get; set; }

    //    public WebSocketQuoteSource(
    //        Action<string> onMessage,
    //        Action<ConnectionEvent, string, WebSocketQuoteSource> onConnectionEvent)
    //    {
    //        this.onMessage = onMessage;
    //        this.onConnectionEvent = onConnectionEvent;
    //        bufferReceived = new ArraySegment<byte>(dataReceived);
    //    }

    //    public void Connect(string uriStr)
    //    {
    //        this.uriStr = uriStr;
    //        var uri = new Uri(uriStr);
    //        webSocket = new ClientWebSocket();
    //        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

    //        var connectTask = webSocket.ConnectAsync(uri, CancellationToken.None);
    //        connectTask.ContinueWith(task =>
    //        {
    //            if (webSocket.State != WebSocketState.Open)
    //            {
    //                onConnectionEvent(ConnectionEvent.Faulted, $"Not connected:{webSocket.State} ({webSocket.CloseStatus})", this);
    //                return;
    //            }

    //            ReceiveData();

    //            if (!string.IsNullOrEmpty(LoginMessage))
    //            {
    //                if (SendCmd(LoginMessage).Wait(1000))
    //                    onConnectionEvent(ConnectionEvent.Connected, "", this);
    //                return;
    //            }
    //            onConnectionEvent(ConnectionEvent.Connected, "", this);
    //        });
    //    }

    //    public void Reconnect()
    //    {
    //        if (loggedOut) return;
    //        Logout();
    //        Connect(uriStr);
    //    }

    //    public Task Logout()
    //    {
    //        loggedOut = true;
    //        if (!string.IsNullOrEmpty(LogoutMessage))
    //            SendCmd(LogoutMessage).Wait(1000);

    //        try
    //        {
    //            return webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None);
    //        }
    //        catch (ObjectDisposedException)
    //        {
    //            return Task.FromResult(false);
    //        }
    //        catch (InvalidOperationException)
    //        {
    //            return Task.FromResult(false);
    //        }
    //    }

    //    public Task SendCmd(string msg)
    //    {
    //        var data = encoding.GetBytes(msg);
    //        if (webSocket.State == WebSocketState.Open) return Task.FromResult(0);
    //        return webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
    //    }

    //    private void ReceiveData()
    //    {
    //        webSocket.ReceiveAsync(bufferReceived, CancellationToken.None).ContinueWith(t =>
    //        {
    //            if (t.Status == TaskStatus.Faulted)
    //            {
    //                onConnectionEvent(ConnectionEvent.Faulted, "", this);
    //                return;
    //            }

    //            WebSocketReceiveResult result = t.Result;
    //            var message = Encoding.ASCII.GetString(bufferReceived.Array,
    //                 bufferReceived.Offset, result.Count);

    //            messageCollected.Append(message);
    //            if (result.EndOfMessage)
    //            {
    //                onMessage(messageCollected.ToString());
    //                messageCollected = new StringBuilder();
    //            }

    //            if (webSocket.State == WebSocketState.Open)
    //                ReceiveData();
    //        });
    //    }
    //}
}
