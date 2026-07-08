namespace Rappd.Data.AspNet.Tests.Classes;

public interface ITestInterface
{
    Guid Identifier { get; }
    string Name { get; }
    string Description { get; set; }
}

[Implements<ITestInterface>]
public class TestObject : ITestInterface
{
    public Guid Identifier { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Additional { get; set; } = string.Empty;
}