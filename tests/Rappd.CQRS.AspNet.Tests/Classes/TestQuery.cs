namespace Rappd.CQRS.AspNet.Tests;

public record TestQuery : Query<TestQuery, int>;
public record TestQueryHandler(ITestService TestService) : TestQuery.Handler
{
    public override Task<Result<int>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult<Result<int>>(TestService.Result);
}