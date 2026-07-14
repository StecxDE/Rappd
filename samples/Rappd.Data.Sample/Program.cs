using Rappd.Data;
using Rappd.Data.Sample.Abstractions;
using Rappd.Data.Sample.Models;
using System.Text.Json;

[assembly: ImplementsFrom<Program>]

var factory = new InterfaceConverterFactory();
var options = new JsonSerializerOptions();
options.Converters.Add(factory);

IWeatherData weatherData = new WeatherData()
{
    Location = "London",
    Time = DateTime.Now,
    Temperature = Random.Shared.Next(10, 20)
};
var weatherDataJson = JsonSerializer.Serialize(weatherData, options);
Console.WriteLine(weatherDataJson);

var personJson = @"
    {
        ""GivenName"": ""Sample"",
        ""LastName"": ""Person""
    }
";
var person = JsonSerializer.Deserialize<IPerson>(personJson, options);
Console.WriteLine($"GivenName: {person?.GivenName} LastName: {person?.LastName}");