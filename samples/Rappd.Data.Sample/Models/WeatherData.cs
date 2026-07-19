using Rappd.Data;
using Rappd.Data.Sample.Abstractions;
using Rappd.Data.Sample.Models;
using System.Timers;

namespace Rappd.Data.Sample.Models;

[Implements<IWeatherData>]
internal partial record WeatherData;