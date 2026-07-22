using Rappd.Data;
using Rappd.Data.Sample.Abstractions;
using Rappd.Data.Sample.Models;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

//[assembly: ImplementsFrom<Program>]

[assembly: Implements<IPerson>]
[assembly: Implements<ISub>]

var factory = new InterfaceConverterFactory();
var options = new JsonSerializerOptions(JsonSerializerOptions.Web);
options.Converters.Add(factory);

var sub = Implementations.CreateISub(
    "123",
    "subbb"
);
var subJson = JsonSerializer.Serialize(sub, options);
Console.WriteLine(subJson);

var @base = JsonSerializer.Deserialize<IBase>(subJson, options);
Console.WriteLine($"Type: {@base?.Type} RefType: {@base?.GetType()}");

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

KnownTypesRegistry.Instance.RegisterConverter<Rappd.Data.Sample.Abstractions.IPerson>((implementation) 
    => new Rappd.Data.InterfaceImplementations.IPersonImplementation
    {
        GivenName = implementation.GivenName,
        LastName = implementation.LastName,
    }
);