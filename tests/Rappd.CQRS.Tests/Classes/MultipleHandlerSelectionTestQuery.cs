namespace Rappd.CQRS.Tests;

public record MultipleHandlerSelectionTestQuery : Query<MultipleHandlerSelectionTestQuery, int>;
[TestHandlerSelection(0), TestHandlerSelection(1)]
public record FirstMultipleHandlerSelectionTestQueryHandler : MultipleHandlerSelectionTestQuery.Handler
{
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result<int>>(1);
}
[TestHandlerSelection(0), TestHandlerSelection(2)]
public record SecondMultipleHandlerSelectionTestQueryHandler : MultipleHandlerSelectionTestQuery.Handler
{
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result<int>>(2);
}