namespace Rappd.CQRS.Tests;

public record TestQuery : Query<TestQuery, int>;
public record TestQueryHandler : TestQuery.Handler
{
    public static Func<Result<int>> Result { get; set; } = () => -1;
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Result());
}