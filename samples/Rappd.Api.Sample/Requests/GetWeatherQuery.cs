using Rappd.Api.Sample.Abstractions;
using Rappd.CQRS;
using Rappd.Data;

namespace Rappd.Api.Sample.Requests;

public record GetWeatherQuery : Query<GetWeatherQuery, IWeatherData>;
public record GetWeatherQueryHandler : GetWeatherQuery.Handler
{
    public override Task<Result<IWeatherData>> HandleAsync(CancellationToken cancellationToken)
        => Task.FromResult(Results.Data(Implementations.CreateIWeatherData(
            "London",
            DateTime.Now,
            Random.Shared.Next(10, 20)
        )));
}