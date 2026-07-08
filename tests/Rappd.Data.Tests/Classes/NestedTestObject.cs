using System;
using System.Collections.Generic;
using System.Text;

namespace Rappd.Data.Tests.Classes;

public interface INestedTestInterface
{
    Guid Identifier { get; }
    string Name { get; }
    ITestInterface? Nested {  get; }
}

[Implements<INestedTestInterface>]
public class NestedTestObject : INestedTestInterface
{
    public Guid Identifier { get; init; }
    public string Name { get; init; } = string.Empty;
    public ITestInterface? Nested { get; set; } = default;
    public string Additional { get; set; } = string.Empty;
}
