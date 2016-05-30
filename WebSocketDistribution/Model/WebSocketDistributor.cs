using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpExtensionsUtil.Extension;
using SharpExtensionsUtil.Logging;
using SharpExtensionsUtil.ThreadSafe;
using SharpExtensionsUtil.Util;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;

namespace WebSocketDistribution.Model
{
    public class WebSocketDistributor : Scheduler
    {
        private WebSocketServer server;

        private readonly MessageStorage storage;

        private readonly int port;

        private readonly ConcurrentBag<WebSocketSession> clients = new ConcurrentBag<WebSocketSession>();

        public Action<string> logMessage;

        public Action<string> onMsgReceived;

        public Action<string> onClientConnected;

        public Action<string> onClientDisconnected;

        public WebSocketDistributor(int port, int distributeInterval)
        {
            this.port = port;
            storage = new MessageStorage();
            schedules = new[]
            {
                new Schedule(Distribute, distributeInterval)
            };
        }

        public static WebSocketDistributor MakeDistributor()
        {
            return new WebSocketDistributor(
                AppConfig.GetIntParam("WebSocketFeeder.Port", 19004),
                AppConfig.GetIntParam("WebSocketFeeder.IntervalMils", 250));
        }

        public void EnqueueQuotes(List<string> quotesToDistribute)
        {
            if (quotesToDistribute.Count == 0) return;
            storage.StoreMessages(quotesToDistribute);
        }

        private void Distribute()
        {
            var quotes = storage.FlushMessages();
            if (quotes.Count == 0) return;

            const int maxQuotesInPack = 50;

            while (quotes.Count > 0)
            {
                var pack = quotes.Take(maxQuotesInPack).ToList();
                quotes = quotes.Skip(maxQuotesInPack).ToList();
                var messageStr = string.Join(Environment.NewLine, pack);
                try
                {
                    foreach (var session in clients)
                    {
                        session.Send(messageStr);
                        // убить сессию, если она не отвечает?
                    }
                }
                catch (Exception ex)
                {
                    if (logMessage != null)
                        logMessage($"Error in WebSocketDistributor.Send(): {ex.ToShortString()}");
                    Logger.Error($"Error in WebSocketDistributor.Send(): {ex.ToShortString()}");
                }
            }
        }

        public override void Start()
        {
            server = new WebSocketServer();
            var cfg = new ServerConfig
            {
                Port = port,
                Ip = "Any",
                MaxConnectionNumber = 100,
                Mode = SocketMode.Tcp,
                Name = "SuperWebSocket Server"
            };
            Logger.Info($"WebSocketServer listens to {port}");
            if (logMessage != null)
                logMessage($"WebSocketServer listens to {port}");

            server.Setup(cfg);
            server.NewMessageReceived += (session, value) =>
            {
                if (onMsgReceived != null)
                    onMsgReceived($"Web Socket - new message: {value}");
            };
            server.NewSessionConnected += session =>
            {
                clients.Add(session);
                if (onClientConnected != null)
                    onClientConnected($"Web Socket - client connected: {session.Host}");
                Debug.WriteLine("Server: new client connected");
                //session.Send("Hello new client!");
            };

            server.SessionClosed += (session, value) =>
            {
                if (onClientDisconnected != null)
                    onClientDisconnected($"Session closed: {session.SessionID} {value}");
                clients.TryTake(out session);
            };

            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in WebSocket: {ex}");
                throw;
            }

            base.Start();
        }

        public override void Stop()
        {
            server.Stop();
            base.Stop();
        }
    }
}
