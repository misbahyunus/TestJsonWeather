using System;
using System.Collections.Generic;
using System.Linq;

namespace TestJsonWeather.Utilities
{
    public static class Searcher
    {
        public static string[] GetTopMatches(Dictionary<string, string> cityIndex, string input, int maxResults = 5)
        {
            if (cityIndex == null || string.IsNullOrWhiteSpace(input)) return Array.Empty<string>();

            var q = input.Trim();

            // simple case-insensitive contains search; preserve original behavior
            var matches = cityIndex
                .Where(kv => kv.Value.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(maxResults)
                .Select(kv => kv.Key)
                .ToArray();

            return matches;
        }
    }
}
