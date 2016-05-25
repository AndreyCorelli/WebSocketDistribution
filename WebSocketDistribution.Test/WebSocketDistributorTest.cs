using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SharpExtensionsUtil.Logging;
using SharpExtensionsUtil.Util;
using WebSocket4Net;
using WebSocketDistribution.Model;

namespace WebSocketDistribution.Test
{
    [TestFixture]
    public class WebSocketDistributorTest
    {
        private readonly List<QuoteData> quotes = new List<QuoteData>();

        private readonly string quoteSetsPath = ExecutablePath.Combine("Settings\\symbol_settings.txt");

        [TestFixtureSetUp]
        public void Setup()
        {
            ExecutablePath.InitializeFake(string.Empty);
            var names = new[] { "EURUSD", "GBPUSD" };
            var prices = new[] { 1.1000, 1.4000 };
            for (var i = 0; i < 500; i++)
            {
                for (var j = 0; j < names.Length; j++)
                    quotes.Add(new QuoteData(names[j], prices[j] + i * 0.0001, prices[j] + i * 0.0001));
            }
        }

        [Test]
        [Ignore("not exactly unit test")]
        public void TestDistribute()
        {
            var distr = new WebSocketDistributor(19006, 100,
                quoteSetsPath, false);

            distr.EnqueueQuotes(quotes);

            distr.Start();
            Thread.Sleep(1000 * 1);
            distr.Stop();
            Logger.Info("1");

            distr.Start();
            distr.EnqueueQuotes(quotes);
            Thread.Sleep(1000 * 1);
            distr.Stop();
        }

        [Test]
        [Ignore("not exactly unit test")]
        public void TestDistributeLong()
        {
            var distr = new WebSocketDistributor(19006, 100,
                quoteSetsPath, false);
            distr.Start();

            for (var i = 0; i < 30; i++)
            {
                distr.EnqueueQuotes(quotes);
                Thread.Sleep(1000);
            }
            distr.Stop();
        }

        [Test]
        public void ClientWsTest()
        {
            const string uri = "ws://127.0.0.1:19006";
            var distr = new WebSocketDistributor(19006, 100, quoteSetsPath, false);
            distr.Start();
            Thread.Sleep(1300);
            var client = new WebSocketClient();
            client.Setup("ws://127.0.0.1:19006", "basic", WebSocketVersion.Rfc6455);
            client.Start();
            Thread.Sleep(1300);
        }

        [Test]
        //[Ignore("not exactly unit test")]
        public void TestDistributeConsume()
        {
            const string uri = "ws://127.0.0.1:19006";
            var distr = new WebSocketDistributor(19006, 100, quoteSetsPath, false);
            distr.Start();
            Thread.Sleep(1300);

            var quotesRecv = new List<string>();
            var events = new List<WebSocketQuoteSource.ConnectionEvent>();
            var listener = new WebSocketQuoteSource(s => quotesRecv.Add(s),
                (@event, s, sender) =>
                {
                    events.Add(@event);
                    if (@event == WebSocketQuoteSource.ConnectionEvent.Disconnected ||
                        @event == WebSocketQuoteSource.ConnectionEvent.Faulted)
                        sender.Reconnect();
                });
            listener.Connect(uri);
            Thread.Sleep(3800);

            for (var i = 0; i < 15; i++)
            {
                distr.EnqueueQuotes(new List<QuoteData>(5) { quotes[i] });
                Thread.Sleep(150);
            }
            Thread.Sleep(500);
            distr.Stop();

            listener.Logout();
            //Assert.AreEqual(quotes.Count, quotesRecv.Count);
        }

        [Test]
        [Ignore("not exactly unit test")]
        public void TestDistributeConsumeReconnect()
        {
            const string uri = "ws://127.0.0.1:19006";
            var distr = new WebSocketDistributor(19006, 100, quoteSetsPath, false);
            distr.Start();
            Thread.Sleep(1300);

            var quotesRecv = new List<string>();
            var events = new List<WebSocketQuoteSource.ConnectionEvent>();
            var listener = new WebSocketQuoteSource(s => quotesRecv.Add(s),
                (@event, s, sender) =>
                {
                    events.Add(@event);
                    if (@event == WebSocketQuoteSource.ConnectionEvent.Disconnected ||
                        @event == WebSocketQuoteSource.ConnectionEvent.Faulted)
                        sender.Reconnect();
                });
            listener.Connect(uri);
            Thread.Sleep(3800);

            for (var i = 0; i < 15; i++)
            {
                distr.EnqueueQuotes(new List<QuoteData>(1) { quotes[i] });
                Thread.Sleep(150);
            }
            Thread.Sleep(500);
            distr.Stop();

            listener.Logout();
            Thread.Sleep(4500);
            listener = new WebSocketQuoteSource(s => quotesRecv.Add(s),
                (@event, s, sender) =>
                {
                    events.Add(@event);
                    if (@event == WebSocketQuoteSource.ConnectionEvent.Disconnected ||
                        @event == WebSocketQuoteSource.ConnectionEvent.Faulted)
                        sender.Reconnect();
                });


            Thread.Sleep(4500);
            distr = new WebSocketDistributor(19006, 100, quoteSetsPath, false);
            distr.Start();

            Thread.Sleep(4500);
            listener.Connect(uri);
            Thread.Sleep(3500);

            distr.EnqueueQuotes(quotes.Take(5).ToList());
            Thread.Sleep(1000 * 3);
            distr.EnqueueQuotes(quotes.Take(1).ToList());
            Thread.Sleep(1000 * 3);
            distr.Stop();

            listener.Logout();

            //Assert.AreEqual(quotes.Count, quotesRecv.Count);
        }
    }
}
