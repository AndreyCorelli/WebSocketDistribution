﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private readonly ConcurrentHashSet<WebSocketSession> clients = new ConcurrentHashSet<WebSocketSession>();

        public Action<string> logMessage;

        public Action<string> onMsgReceived;

        public Action<string> onClientConnected;

        public Action<string> onClientDisconnected;

        public int SessionsActive { get; private set; }

        public WebSocketDistributor(int port, int distributeInterval)
        {
            this.port = port;
            storage = new MessageStorage();
            schedules = new[]
            {
                new Schedule(Distribute, distributeInterval)
            };
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
                    var clientsList = clients.GetItems();
                    SessionsActive = clientsList.Count;
                    foreach (var session in clientsList)
                    {
                        try
                        {
                            session.Send(messageStr);
                        }
                        catch
                        {
                            clients.TryRemove(session);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logMessage?.Invoke($"Error in WebSocketDistributor.Send(): {ex.ToShortString()}");
                    //Logger.Error($"Error in WebSocketDistributor.Send(): {ex.ToShortString()}");
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
            logMessage?.Invoke($"WebSocketServer listens to {port}");

            server.Setup(cfg);
            server.NewMessageReceived += (session, value) =>
            {
                try
                {
                    onMsgReceived?.Invoke($"Web Socket - new message: {value}");
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            };
            server.NewSessionConnected += session =>
            {
                try
                {
                    clients.AddOrReplace(session);
                    onClientConnected?.Invoke($"Web Socket - client connected: {session.Host}");
                    Debug.WriteLine("Server: new client connected");
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            };

            server.SessionClosed += (session, value) =>
            {
                try
                {
                    onClientDisconnected?.Invoke($"Session closed: {session.SessionID} {value}");
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
                finally
                {
                    clients.TryRemove(session);
                }
            };

            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Debug.Write($"Error in WebSocket: {ex}");
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
