namespace Rappd.Data.AspNet.Sample;

public interface IWeatherData
{
    string Location { get; }
    DateTime Time { get; }
    int Temperature { get; }
}