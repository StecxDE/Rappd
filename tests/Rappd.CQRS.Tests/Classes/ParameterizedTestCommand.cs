namespace Rappd.CQRS.Tests;

public record ParameterizedTestCommand : Command<ParameterizedTestCommand, int>;
public record ParameterizedTestCommandHandler : ParameterizedTestCommand.Handler
{
    public static Func<int, Result> Result { get; set; } = (_) => Results.Error;
    public override Task<Result> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Result(Arguments));
}