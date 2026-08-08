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



