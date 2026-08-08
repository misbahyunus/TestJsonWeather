using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestJsonWeather
{
    /// <summary>
    /// Helper to fetch JSON from the web and deserialize it.
    /// Uses HttpClient which is recommended for .NET Core.
    /// Includes HTTP error handling and rate-limit awareness.
    /// </summary>
    public static class FetchJson
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<T> _download_serialized_json_data<T>(string url) where T : new()
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Rate-limited — surface retry info if available
                    if (response.Headers.RetryAfter != null)
                    {
                        Console.WriteLine($"Rate limited by API. Retry after: {response.Headers.RetryAfter}");
                    }
                    else
                    {
                        Console.WriteLine("Rate limited by API (429 Too Many Requests)");
                    }

                    return new T();
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP request failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                    return new T();
                }

                var json_data = await response.Content.ReadAsStringAsync();

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return !string.IsNullOrWhiteSpace(json_data) ? JsonSerializer.Deserialize<T>(json_data, options) ?? new T() : new T();
                }
                catch (JsonException jex)
                {
                    Console.WriteLine($"Failed to deserialize JSON: {jex.Message}");
                    return new T();
                }
            }
            catch (HttpRequestException hex)
            {
                Console.WriteLine($"Network error while fetching JSON: {hex.Message}");
                return new T();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while fetching JSON: {ex.Message}");
                return new T();
            }
        }

        public static T _get_serialized_json_data<T>(string json_data) where T : new()
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return !string.IsNullOrEmpty(json_data) ? JsonSerializer.Deserialize<T>(json_data, options) ?? new T() : new T();
            }
            catch (JsonException)
            {
                return new T();
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
