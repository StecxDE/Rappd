using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Rappd.Data.AspNet.Tests.Classes;
using System.Data.Common;
using System.Text.Json.Serialization.Metadata;

[assembly: AssemblyFixture(typeof(TestServerFixture))]

namespace Rappd.Data.AspNet.Tests.Classes
{
    public class TestServerFixture : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;

        public TestServer TestServer { get; }

        public TestServerFixture()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                EnvironmentName = "Development"
            });
            builder.Services.AddInterfaceHandling<TestServerFixture>();
            builder.WebHost.UseTestServer();
            var app = builder.Build();
            app.MapPost("/serialize", (TestObject @object, IOptions<JsonOptions> options) => TypedResults.Json(@object, JsonTypeInfo.CreateJsonTypeInfo<ITestInterface>(options.Value.SerializerOptions)));
            app.MapPost("/deserialize", (ITestInterface @interface) => TypedResults.Ok(@interface is TestObject { Additional: "" } ? true : false));
            TestServer = app.GetTestServer();
            app.RunAsync(_cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}