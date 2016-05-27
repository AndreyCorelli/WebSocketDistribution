using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using SharpExtensionsUtil.Extension;
using SharpExtensionsUtil.Logging;
using SharpExtensionsUtil.ThreadSafe;
using SharpExtensionsUtil.Util;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;

namespace WebSocketDistribution.Model
{
    public class WebSocketDistributor : Scheduler//, IQuoteDistributor
    {
        private WebSocketServer server;

        private readonly DistributingQuotesStorage storage;

        private readonly int port;

        private readonly ConcurrentBag<WebSocketSession> clients = new ConcurrentBag<WebSocketSession>();  

        public WebSocketDistributor(int port, int distributeInterval,
            string symbolsSettingsPath, bool filterNotMatched)
        {
            this.port = port;
            storage = new DistributingQuotesStorage(symbolsSettingsPath, true, filterNotMatched);
            schedules = new[]
            {
                new Schedule(Distribute, distributeInterval)
            };
        }

        public static WebSocketDistributor MakeDistributor()
        {
            var symbolSetsPath = Path.Combine(ExecutablePath.ExecPath, "site_tcp_symbols.txt");
            return new WebSocketDistributor(
                AppConfig.GetIntParam("WebSocketFeeder.Port", 19004),
                AppConfig.GetIntParam("WebSocketFeeder.IntervalMils", 250),
                symbolSetsPath,
                false);
        }

        public void EnqueueQuotes(List<QuoteData> quotesToDistribute)
        {
            if (quotesToDistribute.Count == 0) return;
            storage.StoreQuotes(quotesToDistribute, true);
        }

        private void Distribute()
        {
            var quotes = storage.FlushQuotes();
            if (quotes.Count == 0) return;

            const int maxQuotesInPack = 50;

            while (quotes.Count > 0)
            {
                var pack = quotes.Take(maxQuotesInPack).ToList();
                quotes = quotes.Skip(maxQuotesInPack).ToList();
                var messageStr = string.Join(Environment.NewLine, pack.Select(m =>
                    m.ToStringWithTrend()));
                try
                {
                    foreach (var session in clients)
                    {
                        session.Send(messageStr);
                    }
//                    var sessions = server.GetAllSessions();
//                    if (sessions != null)
//                        foreach (var session in sessions)
//                        {
//                            session.Send(messageStr);
//                        }
                }
                catch (Exception ex)
                {
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

            server.Setup(cfg);
            server.NewMessageReceived += (session, value) =>
            {
                Logger.Info($"Web Socket - new message: {value}");
            };
            server.NewSessionConnected += session =>
            {
                clients.Add(session);
                Logger.Info($"Web Socket - client connected: {session.Host}");
                Debug.WriteLine("Server: new client connected");
                session.Send("Hello new client!");
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
