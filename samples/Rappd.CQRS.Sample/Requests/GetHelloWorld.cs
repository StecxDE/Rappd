namespace Rappd.CQRS.Sample.Requests;

internal record GetHelloWorld : Query<GetHelloWorld, string>;
internal record GetHelloWorldHandler : GetHelloWorld.Handler
{
    public override async Task<Result<string>> HandleAsync(CancellationToken cancellationToken)
    {
        // Do something async
        await Task.Delay(1000);
        // Say hello
        return "Hello, World!";
    }
}