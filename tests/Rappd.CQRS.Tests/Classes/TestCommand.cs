namespace Rappd.CQRS.Tests;

public record TestCommand : Command<TestCommand>;
public record TestCommandHandler : TestCommand.Handler
{
    public static Func<Result> Result { get; set; } = () => Results.Failed;
    public override Task<Result> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Result());
}