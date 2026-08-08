using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestJsonWeather.Utilities
{
    public static class CityIndexBuilder
    {
        // A small record matching the minimal city.list.json fields we need
        public class CityRecord
        {
            public int _id { get; set; }
            public string name { get; set; }
            public string country { get; set; }
        }

        public static async Task<Dictionary<string, string>> BuildIndexFromStreamAsync(Stream stream)
        {
            var dict = new Dictionary<string, string>();

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<CityRecord>(stream, options))
                {
                    if (item == null) continue;

                    var idStr = item._id.ToString();
                    var country = item.country ?? string.Empty;
                    var name = item.name ?? string.Empty;

                    if (!string.IsNullOrEmpty(idStr) && !string.IsNullOrEmpty(name))
                    {
                        if (!dict.ContainsKey(idStr))
                        {
                            dict[idStr] = name + " (" + country + ")";
                        }
                    }
                }

                return dict;
            }
            catch (JsonException)
            {
                // If streaming parse fails (schema drift, partial file), try a more forgiving parse using JsonDocument
                stream.Seek(0, SeekOrigin.Begin);
                using var doc = await JsonDocument.ParseAsync(stream);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in doc.RootElement.EnumerateArray())
                    {
                        if (el.TryGetProperty("_id", out var idProp) && el.TryGetProperty("name", out var nameProp))
                        {
                            string idStr = idProp.ValueKind == JsonValueKind.Number ? idProp.GetInt32().ToString() : idProp.GetRawText().Trim('"');
                            string name = nameProp.GetString() ?? string.Empty;
                            string country = el.TryGetProperty("country", out var cprop) ? cprop.GetString() ?? string.Empty : string.Empty;

                            if (!dict.ContainsKey(idStr))
                                dict[idStr] = name + " (" + country + ")";
                        }
                    }
                }

                return dict;
            }
        }

        public static async Task<Dictionary<string, string>> BuildIndexFromFileAsync(string path)
        {
            using var fs = File.OpenRead(path);
            return await BuildIndexFromStreamAsync(fs);
        }
    }
}
