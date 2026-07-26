using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rappd.Data.Bson.Sample.Abstractions;

internal interface IPerson
{
    string? GivenName { get; }
    string LastName { get; }

    IBase Poly { get; }
}