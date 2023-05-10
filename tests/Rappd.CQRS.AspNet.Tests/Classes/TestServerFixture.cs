using Microsoft.AspNetCore.TestHost;

namespace Rappd.CQRS.AspNet.Tests;

public class TestServerFixture : IDisposable
{
    public TestServer TestServer { get; }

    public TestServerFixture()
    {
        TestServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<ITestService, TestService>();
            })
            .UseEnvironment("Development")
            .Configure(app =>
            {
                app.ConfigureCqrs();
            })
        );
    }

    public void Dispose()
    {
        TestServer.Dispose();
    }
}
[CollectionDefinition(nameof(TestServerCollection))]
public class TestServerCollection : ICollectionFixture<TestServerFixture> { }
