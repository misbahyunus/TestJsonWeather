using System.Collections.Generic;
using TestJsonWeather.Utilities;
using Xunit;

namespace TestJsonWeather.Tests
{
    public class SearchTests
    {
        [Fact]
        public void GetTopMatches_ReturnsExpectedKeys()
        {
            var dict = new Dictionary<string, string>
            {
                ["1"] = "Paris (FR)",
                ["2"] = "Paris (US)",
                ["3"] = "London (GB)",
                ["4"] = "Los Angeles (US)",
                ["5"] = "Portland (US)"
            };

            var results = Searcher.GetTopMatches(dict, "paris", 5);

            Assert.Equal(2, results.Length);
            Assert.Contains("1", results);
            Assert.Contains("2", results);
        }

        [Fact]
        public void GetTopMatches_EmptyQuery_ReturnsEmpty()
        {
            var dict = new Dictionary<string, string>
            {
                ["1"] = "Paris (FR)"
            };

            var results = Searcher.GetTopMatches(dict, "  ", 5);
            Assert.Empty(results);
        }
    }
}
