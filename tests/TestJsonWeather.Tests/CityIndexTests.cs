using System.IO;
using System.Text;
using System.Threading.Tasks;
using TestJsonWeather.Utilities;
using Xunit;
using System.Collections.Generic;

namespace TestJsonWeather.Tests
{
    public class CityIndexTests
    {
        [Fact]
        public async Task BuildIndexFromStreamAsync_ParsesSimpleArray()
        {
            var json = "[ { \"_id\": 1, \"name\": \"Paris\", \"country\": \"FR\" }, { \"_id\": 2, \"name\": \"Paris\", \"country\": \"US\" } ]";
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var dict = await CityIndexBuilder.BuildIndexFromStreamAsync(ms);

            Assert.Equal(2, dict.Count);
            Assert.Equal("Paris (FR)", dict["1"]);
            Assert.Equal("Paris (US)", dict["2"]);
        }

        [Fact]
        public async Task BuildIndexFromStreamAsync_HandlesMissingCountry()
        {
            var json = "[ { \"_id\": 10, \"name\": \"Nowhere\" } ]";
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var dict = await CityIndexBuilder.BuildIndexFromStreamAsync(ms);

            Assert.Single(dict);
            Assert.Equal("Nowhere ()", dict["10"]);
        }
    }
}
