namespace Rappd.CQRS.Tests;

public sealed class TestHandlerSelectionAttribute : HandlerSelectionAttribute
{
    public static int[] Values { get; set; } = Array.Empty<int>();

    private readonly int _value;

    public TestHandlerSelectionAttribute(int value)
        => _value = value;

    public override bool IsMatch()
        => Values.Contains(_value);
}