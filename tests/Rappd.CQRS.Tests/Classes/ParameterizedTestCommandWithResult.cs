namespace Rappd.CQRS.Tests;

public record ParameterizedTestCommandWithResult : ParameterizedCommand<ParameterizedTestCommandWithResult, int, int>;
public record ParameterizedTestCommandWithResultHandler : ParameterizedTestCommandWithResult.Handler
{
    public static Func<int, Result<int>> Result { get; set; } = (_) => -1;
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Result(Arguments));
}