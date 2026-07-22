using Microsoft.AspNetCore.Http.HttpResults;
using Rappd.Api;
using Rappd.Api.Sample.Abstractions;
using Rappd.CQRS;
using Rappd.Data;

using Requests = Rappd.Api.Sample.Requests;

[assembly: ImplementsFrom<Program>]

await new LightApi(
    // Add services to the container
    configure: static (config, services) => services
        .AddInterfaceHandling()
    // Configure the HTTP request pipeline
    , build: static (app, pipeline) => pipeline
        .ConfigureCqrs()
    // Define the HTTP endpoints
    , map: static (config, route) => route.Map(
        "/weather".Get(async () =>
        {
            var response = await Requests.GetWeatherQuery.SendAsync();
            if (!response.IsSuccess)
                return Results.InternalServerError();
            return Results.Ok(response.Result);
        }),
        "/request/{type}".Get(async (string type) => 
            type switch
            {
                IHelloRequest.TYPE => Results.Ok(Implementations.CreateIHelloRequest("World")),
                IWorldRequest.TYPE => Results.Ok(Implementations.CreateIWorldRequest("Hello")),
                _ => Results.BadRequest()
            }
        ),
        "/request".Post(async (IRequest request) =>
            request switch
            {
                IHelloRequest hello => Results.Ok($"{hello.Type}: {hello.Hello}"),
                IWorldRequest world => Results.Ok($"{world.Type}: {world.World}"),
                _ => Results.BadRequest()
            }
        )
    )
).RunAsync(args);