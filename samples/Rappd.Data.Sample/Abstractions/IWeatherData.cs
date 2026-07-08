using System;
using System.Collections.Generic;
using System.Text;

namespace Rappd.Data.Sample.Abstractions;

public interface IWeatherData
{
    string Location { get; }
    DateTime Time { get; }
    int Temperature { get; }
}