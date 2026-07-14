namespace Rappd.Api.Sample.Abstractions;

public interface IWeatherData
{
    string Location { get; }
    DateTime Time { get; }
    int Temperature { get; }
}