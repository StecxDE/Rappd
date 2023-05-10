namespace Rappd.CQRS.Tests;

public record SingleHandlerSelectionTestQuery : Query<SingleHandlerSelectionTestQuery, int>;
[TestHandlerSelection(1)]
public record FirstSingleHandlerSelectionTestQueryHandler : SingleHandlerSelectionTestQuery.Handler
{
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result<int>>(1);
}
[TestHandlerSelection(2)]
public record SecondSingleHandlerSelectionTestQueryHandler : SingleHandlerSelectionTestQuery.Handler
{
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result<int>>(2);
}