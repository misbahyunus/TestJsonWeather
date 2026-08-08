using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestJsonWeather
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Defaults can come from environment variables
            string cityListPath = Environment.GetEnvironmentVariable("CITY_LIST_PATH") ?? @"c:\\rmcs\\city.list.json";
            string apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? string.Empty;

            // Simple command-line parsing: --citylist <path> --apikey <key>
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--citylist" && i + 1 < args.Length)
                {
                    cityListPath = args[++i];
                }
                else if (args[i] == "--apikey" && i + 1 < args.Length)
                {
                    apiKey = args[++i];
                }
            }

            if (!File.Exists(cityListPath))
            {
                Console.WriteLine($"City list file not found: {cityListPath}");
                Console.WriteLine("Please download OpenWeather's city.list.json and set CITY_LIST_PATH or pass --citylist <path>");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("OpenWeather API key not provided. Set OPENWEATHER_API_KEY or pass --apikey <key>");
                return 1;
            }

            bool loadAgain = true;

            // Build index from file (streaming)
            Dictionary<string, string> cityAllList = new Dictionary<string, string>();
            try
            {
                using var fs = File.OpenRead(cityListPath);
                cityAllList = await Utilities.CityIndexBuilder.BuildIndexFromStreamAsync(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to build city index: {ex.Message}");
                return 1;
            }

            do
            {
                string[] citySearchResultList = new string[5];
                ArrayList cityCountList2 = new ArrayList();
                int found = 0;

                Console.WriteLine("Enter city name - ");
                string input = System.Console.ReadLine() ?? string.Empty;
                input = input.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                var matches = Utilities.Searcher.GetTopMatches(cityAllList, input, 5);

                Console.WriteLine("Top 5 matches found - ");
                int count = 1;

                for (int idx = 0; idx < matches.Length; idx++)
                {
                    if (matches[idx] != null)
                    {
                        Console.WriteLine(count + ". " + cityAllList[matches[idx]]);
                        cityCountList2.Add(matches[idx]);
                        citySearchResultList[idx] = matches[idx];
                        count++;
                    }
                }

                Console.WriteLine("Choose the one option from above. E.g: 1");
                string selectedCity = Console.ReadLine() ?? string.Empty;

                if (!int.TryParse(selectedCity, out int selection))
                {
                    Console.WriteLine("Invalid selection");
                }
                else if (selection > 0 && selection <= cityCountList2.Count)
                {
                    var cityId = citySearchResultList[selection - 1];
                    var url = $"http://api.openweathermap.org/data/2.5/group?units=metric&appid={apiKey}&id={cityId}";

                    var cityWeather = await FetchJson._download_serialized_json_data<CityWeather>(url);

                    if (cityWeather?.list?.Length > 0)
                    {
                        Console.WriteLine("*-----------------------------------------------------------");
                        Console.WriteLine("| City:                  " + cityWeather.list[0].name);
                        Console.WriteLine("|-----------------------------------------------------------");
                        Console.WriteLine("| Country:             | " + cityWeather.list[0].sys.country);
                        Console.WriteLine("| Current Weather:     | " + (cityWeather.list[0].weather != null && cityWeather.list[0].weather.Length>0 ? cityWeather.list[0].weather[0].description : "N/A"));
                        Console.WriteLine("| Current Temperature: | " + Math.Round(cityWeather.list[0].main.temp, 1) + "°C");
                        Console.WriteLine("| Sunrise:             | " + FetchJson.UnixTimeStampToDateTime(cityWeather.list[0].sys.sunrise));
                        Console.WriteLine("| Sunset:              | " + FetchJson.UnixTimeStampToDateTime(cityWeather.list[0].sys.sunset));
                        Console.WriteLine("*-----------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("No weather data returned from API or failed to parse response.");
                    }
                }

                Console.WriteLine("Look for more cities [y/n] ? - ");
                input = Console.ReadLine() ?? string.Empty;
                if (input.Trim().ToLower() == "n") { loadAgain = false; }

            } while (loadAgain);

            return 0;
        }
    }
}
