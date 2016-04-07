using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using NodaTime.TimeZones;

namespace TestJsonWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Fields            
            bool loadAgain = true;
            // Stores list of all the cities in the world
            Dictionary<string, string> cityAllList = new Dictionary<string, string>();
            #endregion

            #region Reads JSON directly from a file and populates dictionary
            using (StreamReader file = File.OpenText(@"c:\rmcs\city.list.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                reader.SupportMultipleContent = true;

                while (true)
                {
                    if (!reader.Read())
                    {
                        break;
                    }

                    JObject o2 = (JObject)JToken.ReadFrom(reader);

                    cityAllList.Add(o2["_id"].ToString(), o2["name"].ToString() + " (" + o2["country"].ToString() + ")");
                }
            }
            #endregion

            do
            {
                string[] citySearchResultList = new string[5];
                ArrayList cityCountList2 = new ArrayList();
                int i = 0;

                #region Handle User Input
                Console.WriteLine("Enter city name - ");
                string input = System.Console.ReadLine();
                input = input.ToLower();

                foreach (KeyValuePair<string, string> entry in cityAllList)
                {
                    if (entry.Value.ToLower().Contains(input))
                    {
                        cityCountList2.Add(entry.Key);
                        citySearchResultList[i] = entry.Key;
                        i++;
                    }

                    if (i == 5) { break; }
                }
                #endregion

                #region City Search
                Console.WriteLine("Top 5 matches found - ");
                int count = 1;

                for (int idx = 0; idx < cityCountList2.Count; idx++)
                {
                    if (citySearchResultList[idx] != null)
                    {
                        Console.WriteLine(count + ". " + cityAllList[citySearchResultList[idx]]);
                        count++;
                    }
                }
                #endregion

                Console.WriteLine("Choose the one option from above. E.g: 1");
                string selectedCity = Console.ReadLine();
                int selection = int.Parse(selectedCity);

                if (selection > 0 && selection < cityCountList2.Count + 1)
                {

                    var url = "http://api.openweathermap.org/data/2.5/group?units=metric&appid=3bd5b74046356dd09403a84d8ee7caad&id=" + citySearchResultList[selection - 1];

                    var cityWeather = FetchJson._download_serialized_json_data<CityWeather>(url);

                    Console.WriteLine("*-----------------------------------------------------------");
                    Console.WriteLine("| City:                  " + cityWeather.list[0].name);
                    Console.WriteLine("|-----------------------------------------------------------");
                    Console.WriteLine("| Country:             | " + cityWeather.list[0].sys.country);
                    Console.WriteLine("| Current Weather:     | " + cityWeather.list[0].weather[0].description);
                    Console.WriteLine("| Current Temperature: | " + Math.Round(cityWeather.list[0].main.temp, 1) + "°C");
                    //Console.WriteLine("|                      | [Max: " + Math.Round(cityWeather.list[0].main.temp_max, 1) + "°C" + "  Min:" + Math.Round(cityWeather.list[0].main.temp_min, 1) + "°C]");
                    Console.WriteLine("| Sunrise:             | " + FetchJson.UnixTimeStampToDateTime(cityWeather.list[0].sys.sunrise));
                    Console.WriteLine("| Sunset:              | " + FetchJson.UnixTimeStampToDateTime(cityWeather.list[0].sys.sunset));
                    Console.WriteLine("*-----------------------------------------------------------");

                    //IEnumerable<string> windowsZoneIds = TzdbDateTimeZoneSource.Default.ZoneLocations
                    //    .Where(x => x.CountryCode == cityWeather.list[0].sys.country)
                    //    .Select(tz => TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones
                    //    .FirstOrDefault(x => x.TzdbIds.Contains(tz.ZoneId)))
                    //    .Where(x => x != null)
                    //    .Select(x => x.WindowsId)
                    //    .Distinct();

                    //foreach(string temp in windowsZoneIds)
                    //{
                    //    Console.WriteLine(temp);
                    //}                    
                }

                Console.WriteLine("Look for more cities [y/n] ? - ");
                input = Console.ReadLine();
                if (input.ToLower() == "n") { loadAgain = false; }
                

            } while (loadAgain);
        }
    }
}
