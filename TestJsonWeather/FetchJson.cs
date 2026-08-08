using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestJsonWeather
{
    /// <summary>
    /// Helper to fetch JSON from the web and deserialize it.
    /// Uses HttpClient which is recommended for .NET Core.
    /// </summary>
    public static class FetchJson
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<T> _download_serialized_json_data<T>(string url) where T : new()
        {
            try
            {
                var json_data = await _httpClient.GetStringAsync(url);
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
            catch (Exception)
            {
                return new T();
            }
        }

        public static T _get_serialized_json_data<T>(string json_data) where T : new()
        {
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
