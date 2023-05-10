namespace Rappd.CQRS.Tests;

public record ParameterizedTestQuery : Query<ParameterizedTestQuery, int, int>;
public record ParameterizedTestQueryHandler : ParameterizedTestQuery.Handler
{
    public static Func<int, Result<int>> Result { get; set; } = i => -1;
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Result(Arguments));
}