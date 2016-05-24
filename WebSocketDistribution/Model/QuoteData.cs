using Newtonsoft.Json;
using SharpExtensionsUtil.Converter;

namespace WebSocketDistribution.Model
{
    /// <summary>
    /// просто котировка, Bid + Ask + символ
    /// </summary>
    public class QuoteData
    {
        [JsonProperty(PropertyName = "s")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "b")]
        public double Bid { get; set; }

        [JsonProperty(PropertyName = "a")]
        public double Ask { get; set; }

        public QuoteData()
        {
        }

        public QuoteData(QuoteData q)
        {
            Symbol = q.Symbol;
            Bid = q.Bid;
            Ask = q.Ask;
        }

        public QuoteData(string symbol, double bid, double ask)
        {
            Symbol = symbol;
            Bid = bid;
            Ask = ask;
        }

        public override string ToString()
        {
            return Symbol + "," + Bid.ToStringUniformPriceFormat(true) + "," +
                   Ask.ToStringUniformPriceFormat(true);
        }
    }
}
