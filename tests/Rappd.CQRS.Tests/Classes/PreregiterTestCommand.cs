namespace Rappd.CQRS.Tests;

public sealed class PreregisterTestAttribute : HandlerSelectionAttribute
{
    public override bool IsMatch()
        => true;
}

public record PreregiterTestCommand : Command<PreregiterTestCommand>;

[PreregisterTest]
public record UnregiteredPreregiterTestCommandHandler : PreregiterTestCommand.Handler
{
    public override Task<Result> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result>(Results.Failed);
}

public record PreregiteredPreregiterTestCommandHandler : PreregiterTestCommand.Handler
{
    public override Task<Result> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Results.Ok);
}