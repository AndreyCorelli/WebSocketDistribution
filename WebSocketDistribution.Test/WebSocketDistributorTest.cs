using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using WebSocket4Net;
using WebSocketDistribution.Model;

namespace WebSocketDistribution.Test
{
    [TestFixture]
    public class WebSocketDistributorTest
    {
        private readonly List<string> quotes = new List<string>();

        private const int Port = 19006;

        [TestFixtureSetUp]
        public void Setup()
        {
            ExecutablePath.InitializeFake(string.Empty);
            var names = new[] { "EURUSD", "GBPUSD" };
            var prices = new[] { 1.1000, 1.4000 };
            for (var i = 0; i < 500; i++)
            {
                foreach (var name in names)
                    quotes.Add($"{name}: {i}");
            }
        }

        [Test]
        [Ignore("not exactly unit test")]
        public void TestDistribute()
        {
            var distr = new WebSocketDistributor(Port, 100);

            distr.EnqueueQuotes(quotes);

            distr.Start();
            Thread.Sleep(1000 * 1);
            distr.Stop();

            distr.Start();
            distr.EnqueueQuotes(quotes);
            Thread.Sleep(1000 * 1);
            distr.Stop();
        }

        [Test]
        [Ignore("not exactly unit test")]
        public void TestDistributeLong()
        {
            var distr = new WebSocketDistributor(Port, 100);
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
            var uri = $"ws://127.0.0.1:{Port}";
            var distr = new WebSocketDistributor(Port, 100);
            distr.Start();
            Thread.Sleep(1000);

            var client = new WebSocketClient();
            client.Setup(uri, "basic", WebSocketVersion.Rfc6455, s => Debug.WriteLine($"message received: {s}"),
                (@event, s) => Debug.WriteLine($"Connection event: {s}"));
            client.Start();
            Thread.Sleep(1000);
            distr.EnqueueQuotes(quotes);
            for (var i = 0; i < 15; i++)
            {
                distr.EnqueueQuotes(new List<string>(1) { quotes[i] });
                Thread.Sleep(150);
            }
            Thread.Sleep(2300);

            distr.Stop();
            distr = new WebSocketDistributor(Port, 100);
            distr.Start();

            Thread.Sleep(2500);

            for (var i = 0; i < 15; i++)
            {
                distr.EnqueueQuotes(new List<string>(1) { quotes[i] });
                Thread.Sleep(150);
            }
            distr.Stop();
        }        
    }
}
