using SharpExtensionsUtil.Converter;

namespace WebSocketDistribution.Model
{
    public class Mt4Quote
    {
        public string Symbol { get; set; }
        public float Bid { get; set; }
        public float Ask { get; set; }
        public int Volume { get; set; }

        public int Trend { get; set; }

        public Mt4Quote(string symbol, float bid, float ask, int trend)
        {
            Symbol = symbol;
            Bid = bid;
            Ask = ask;
            Trend = trend;
        }

        public Mt4Quote(string symbol, QuoteData quote)
        {
            Symbol = symbol;
            Bid = (float)quote.Bid;
            Ask = (float)quote.Ask;
        }

        public override string ToString()
        {
            return ToStringMtFormat();
        }

        public string ToStringMtFormat()
        {
            return string.Format(CultureProvider.Common, "{0} {1} {2};", Symbol, Bid, Ask);
        }

        public string ToStringWithTrend()
        {
            return string.Format(CultureProvider.Common, "{0} {1} {2} {3};", Symbol, Bid, Ask, Trend);
        }
    }
}
