using Rappd.Data;
using Rappd.Data.AspNet.Sample;

[assembly: ImplementsFrom<Program>]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInterfaceHandling();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weather", () =>
{
    return Implementations.CreateIWeatherData(
        "London",
        DateTime.Now,
        Random.Shared.Next(10, 20)
    );
});
app.MapPost("/waether", (IWeatherData waether) =>
{
    return waether.Location;
});

app.Run();