using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rappd.Data.Sample.Abstractions;

public interface IPerson
{
    [DefaultValue("")]
    string GivenName { get; set; }
    string LastName { get; }
}