# TestJsonWeather

High-level: A small .NET console app that looks up a city from an OpenWeather city list (city.list.json), fetches current weather from OpenWeatherMap, and prints a short summary to the console.

This repository has been converted to a single .NET 6 console project that builds with the `dotnet` CLI.

## Requirements
- .NET 6 SDK (or newer)
- An OpenWeather API key
- OpenWeather's city list JSON (city.list.json). You can download it from OpenWeather and place it locally.

## Build and run
From the repository root:

```bash
# build
dotnet build TestJsonWeather

# run (provide either env vars or CLI args)
# using CLI args
dotnet run --project TestJsonWeather -- --citylist "/path/to/city.list.json" --apikey "YOUR_API_KEY"

# or using environment variables
export CITY_LIST_PATH="/path/to/city.list.json"
export OPENWEATHER_API_KEY="YOUR_API_KEY"
dotnet run --project TestJsonWeather
```

Notes:
- The program will show the top 5 matches for a provided city name and let you pick one to fetch current weather.
- Default city list path is `c:\rmcs\city.list.json` on Windows if no value is provided.
- The app uses Newtonsoft.Json for JSON parsing.

## Changes made
- Converted the project file to SDK-style (TargetFramework net6.0) so it builds with `dotnet` CLI.
- Replaced WebClient usage with HttpClient and made the fetch method async.
- Parameterized the city list path and API key via environment variables or command-line flags `--citylist` and `--apikey`.
- Simplified to a single project under `TestJsonWeather`.

If you'd like, I can also:
- Add a small sample city list subset for quick testing.
- Replace Newtonsoft.Json with System.Text.Json.
- Add GitHub Actions workflow to build and run a smoke test.
