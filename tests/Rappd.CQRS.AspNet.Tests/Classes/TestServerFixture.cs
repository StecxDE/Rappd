using Microsoft.AspNetCore.TestHost;
using Rappd.CQRS.AspNet.Tests.Classes;
using System.Data.Common;

[assembly: AssemblyFixture(typeof(TestServerFixture))]

namespace Rappd.CQRS.AspNet.Tests.Classes
{
    public class TestServerFixture
    {
        public TestServer TestServer { get; }

        public TestServerFixture()
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                EnvironmentName = "Development"
            });
            builder.Services.AddSingleton<ITestService, TestService>();
            builder.WebHost.UseTestServer();
            var app = builder.Build();
            app.ConfigureCqrs();
            TestServer = app.GetTestServer();
        }
    }
}