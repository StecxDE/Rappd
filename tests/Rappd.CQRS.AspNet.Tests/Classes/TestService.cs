namespace Rappd.CQRS.AspNet.Tests;
public interface ITestService
{
    public int Result { get; }
}
public sealed class TestService : ITestService
{
    public int Result => 1;
}
