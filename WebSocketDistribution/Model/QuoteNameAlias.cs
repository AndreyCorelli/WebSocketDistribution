using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SharpExtensionsUtil.Logging;

namespace WebSocketDistribution.Model
{
    [Serializable]
    public class QuoteNameAlias
    {
        private Regex symbolRegex;

        [JsonProperty("symbol_regex")]
        public string SymbolRegex
        {
            get { return symbolRegex?.ToString() ?? ""; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    symbolRegex = null;
                    return;
                }
                try
                {
                    symbolRegex = new Regex(value);
                }
                catch
                {
                    symbolRegex = null;
                }
            }
        }

        private Regex nameExtractorRegex;

        [JsonProperty("name_part_regex")]
        public string NamePartRegex
        {
            get { return nameExtractorRegex == null ? "" : nameExtractorRegex.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    nameExtractorRegex = null;
                    return;
                }
                try
                {
                    nameExtractorRegex = new Regex(value);
                }
                catch
                {
                    nameExtractorRegex = null;
                }
            }
        }

        [JsonProperty("match_string")]
        public string MatchString { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        public QuoteNameAlias()
        {
        }

        public QuoteNameAlias(string matchString, string alias)
        {
            this.MatchString = matchString;
            this.Alias = alias;
        }

        public static QuoteNameAlias ParseLine(string line)
        {
            // NGASb,alias:NGAS
            // regex:[A-Z0-9],name_part_regex:[A-Z0-9]
            // GBPUSD
            if (string.IsNullOrEmpty(line)) return null;
            var aliasParts = line.Split(new[] { ",alias:" }, StringSplitOptions.RemoveEmptyEntries);
            var alias = string.Empty;
            if (aliasParts.Length == 2)
            {
                alias = aliasParts[1];
                line = aliasParts[0];
            }

            var patternConversionParts = line.Split(new[] { ",name_part_regex:" }, StringSplitOptions.RemoveEmptyEntries);
            Regex nameRegex = null;
            if (patternConversionParts.Length == 2)
            {
                try
                {
                    nameRegex = new Regex(patternConversionParts[1]);
                }
                catch
                {
                    Logger.Error("Mt4QuoteNamePattern: error parsing name regex [" + line + "]");
                    return null;
                }
                line = patternConversionParts[0];
            }

            if (line.StartsWith("regex:"))
            {
                try
                {
                    var regex = new Regex(line.Substring("regex:".Length));
                    return new QuoteNameAlias
                    {
                        symbolRegex = regex,
                        nameExtractorRegex = nameRegex,
                        Alias = alias
                    };
                }
                catch
                {
                    Logger.Error("Mt4QuoteNamePattern: error parsing line [" + line + "]");
                    return null;
                }
            }
            return new QuoteNameAlias
            {
                MatchString = line,
                nameExtractorRegex = nameRegex,
                Alias = alias
            };
        }

        public bool IsMatched(string symbolName)
        {
            if (symbolRegex != null)
                return symbolRegex.IsMatch(symbolName);
            return symbolName == MatchString;
        }

        public string GetNameFromAlias(string symbolName)
        {
            if (!string.IsNullOrEmpty(Alias))
                return Alias;
            if (nameExtractorRegex == null) return symbolName;
            var match = nameExtractorRegex.Match(symbolName);
            return match.Value;
        }

        public override string ToString()
        {
            var clauses = new List<string>();
            if (symbolRegex != null)
                clauses.Add("regex:" + symbolRegex);
            if (!string.IsNullOrEmpty(MatchString))
                clauses.Add(MatchString);
            if (nameExtractorRegex != null)
                clauses.Add("name_part_regex:" + nameExtractorRegex);
            if (!string.IsNullOrEmpty(Alias))
                clauses.Add("alias:" + Alias);
            return string.Join(",", clauses);
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(MatchString) && symbolRegex == null;
        }
    }
}
