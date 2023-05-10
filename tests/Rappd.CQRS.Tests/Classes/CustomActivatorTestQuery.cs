namespace Rappd.CQRS.Tests;

public record CustomActivatorTestQuery : Query<CustomActivatorTestQuery, int>;
public record CustomActivatorTestQueryHandler : CustomActivatorTestQuery.Handler
{
    private readonly int _value;
    public CustomActivatorTestQueryHandler(int value)
        => _value = value;
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result<int>>(_value);
}