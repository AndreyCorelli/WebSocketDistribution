using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SharpExtensionsUtil.Converter;
using SharpExtensionsUtil.Logging;
using SharpExtensionsUtil.Util;

namespace WebSocketDistribution.Model
{
    public class DistributingQuotesStorage
    {
        private readonly ReaderWriterLockSlim symbolsToDistributeLocker = new ReaderWriterLockSlim();

        private List<QuoteNameAlias> symbolsToDistribute;

        public List<QuoteNameAlias> SymbolsToDistribute
        {
            get
            {
                symbolsToDistributeLocker.EnterReadLock();
                try
                {
                    return symbolsToDistribute.ToList();
                }
                finally
                {
                    symbolsToDistributeLocker.ExitReadLock();
                }
            }
            set
            {
                symbolsToDistributeLocker.EnterWriteLock();
                try
                {
                    symbolsToDistribute = value;
                }
                finally
                {
                    symbolsToDistributeLocker.ExitWriteLock();
                }
                StoreSymbolsSettings();
            }
        }

        private readonly List<Mt4Quote> quotesList = new List<Mt4Quote>();

        private readonly ReaderWriterLockSlim quotesListLocker = new ReaderWriterLockSlim();

        private readonly Dictionary<string, Mt4Quote> lastQuoteByTicker = new Dictionary<string, Mt4Quote>();

        private readonly ReaderWriterLockSlim lastQuoteLocker = new ReaderWriterLockSlim();

        private readonly bool determineTrend;

        private readonly FloodSafeLogger logNoFlood = new FloodSafeLogger(1000 * 60 * 15);

        private const int LogMsgDeliverQuote = 1;

        private readonly bool filterNotMatched;

        private readonly Dictionary<string, string> symbolAlias = new Dictionary<string, string>();

        private readonly string filePath;

        public DistributingQuotesStorage(string filePath,
            bool determineTrend, bool filterNotMatched)
        {
            this.filePath = filePath;
            this.determineTrend = determineTrend;
            this.filterNotMatched = filterNotMatched;
            LoadSymbolsSettings();
        }

        public void StoreQuotes(List<QuoteData> quotesToDistribute, bool removeDuplicates)
        {
            if (quotesToDistribute.Count == 0) return;
            var quotesMt4 = quotesToDistribute.Select(q => new Mt4Quote(q.Symbol, q)).ToList();
            quotesMt4 = FilterAndRenameQuotes(quotesMt4);
            // определить тренд
            if (determineTrend) DetermineTrend(quotesMt4);
            if (quotesMt4.Count == 0) return;
            quotesListLocker.EnterWriteLock();
            try
            {
                if (removeDuplicates)
                {
                    foreach (var quote in quotesMt4)
                    {
                        var existedIndex = quotesList.FindIndex(q => q.Symbol == quote.Symbol);
                        if (existedIndex >= 0)
                            quotesList[existedIndex] = quote;
                        else quotesList.Add(quote);
                    }
                }
                else quotesList.AddRange(quotesMt4);
            }
            finally
            {
                quotesListLocker.ExitWriteLock();
            }
            logNoFlood.LogMessageFormatCheckFlood(LogEntryType.Info,
                LogMsgDeliverQuote, 1000 * 60 * 60 * 2,
                "TcpDistributor - storing [{0}] quotes",
                string.Join(",", quotesMt4.Select(q => q.Symbol + ":" +
                    q.Bid.ToStringUniformMoneyFormat())).ToString());
        }

        public List<Mt4Quote> FlushQuotes()
        {
            quotesListLocker.EnterWriteLock();
            try
            {
                var list = quotesList.ToList();
                quotesList.Clear();
                return list;
            }
            finally
            {
                quotesListLocker.ExitWriteLock();
            }
        }

        public List<Mt4Quote> ReadQuotes()
        {
            quotesListLocker.EnterReadLock();
            try
            {
                var list = quotesList.ToList();
                return list;
            }
            finally
            {
                quotesListLocker.ExitReadLock();
            }
        }

        public List<Mt4Quote> FilterAndRenameQuotes(List<Mt4Quote> quotes)
        {
            if (symbolsToDistribute.Count == 0)
                return quotes;
            var resultedQuotes = new List<Mt4Quote>();
            foreach (var q in quotes)
            {
                string alias;
                if (symbolAlias.TryGetValue(q.Symbol, out alias))
                {
                    if (string.IsNullOrEmpty(alias)) continue;
                    q.Symbol = alias;
                    resultedQuotes.Add(q);
                    continue;
                }

                var pattern = symbolsToDistribute.FirstOrDefault(p => p.IsMatched(q.Symbol));
                if (pattern != null)
                {
                    alias = pattern.GetNameFromAlias(q.Symbol);
                    symbolAlias.Add(q.Symbol, alias);
                    q.Symbol = alias;
                    resultedQuotes.Add(q);
                    continue;
                }
                alias = filterNotMatched ? "" : q.Symbol;
                symbolAlias.Add(q.Symbol, alias);
                if (!filterNotMatched)
                    resultedQuotes.Add(q);
            }
            return resultedQuotes;
        }

        private void DetermineTrend(List<Mt4Quote> quotes)
        {
            lastQuoteLocker.EnterWriteLock();
            try
            {
                foreach (var quote in quotes)
                {
                    Mt4Quote lastQ;
                    if (!lastQuoteByTicker.TryGetValue(quote.Symbol, out lastQ))
                    {
                        lastQuoteByTicker.Add(quote.Symbol, quote);
                        continue;
                    }
                    quote.Trend = lastQ.Bid < quote.Bid
                        ? 1 : lastQ.Bid > quote.Bid ? -1 : lastQ.Ask < quote.Ask ? 1 : lastQ.Ask > quote.Ask ? -1 : 0;
                    lastQuoteByTicker[quote.Symbol] = quote;
                }
            }
            finally
            {
                lastQuoteLocker.ExitWriteLock();
            }
        }

        private void StoreSymbolsSettings()
        {
            try
            {
                TypedStreamReaderWriter.SaveInFile(filePath, SymbolsToDistribute,
                    @alias => @alias.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("Error in DistributingQuotesStorage.StoreSymbolsSettings()", ex);
                throw;
            }
        }

        private void LoadSymbolsSettings()
        {
            try
            {
                symbolsToDistribute = TypedStreamReaderWriter.ReadFromStream(filePath, QuoteNameAlias.ParseLine);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in DistributingQuotesStorage.LoadSymbolsSettings()", ex);
                throw;
            }
        }
    }
}
