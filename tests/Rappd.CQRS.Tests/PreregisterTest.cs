using Newtonsoft.Json.Linq;
using Rappd.CQRS.Tests.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rappd.CQRS.Tests;

public class PreregisterTest
{

    private CqrsProviderFixture _cqrsProviderFixture;

    public PreregisterTest(CqrsProviderFixture cqrsProviderFixture)
        => _cqrsProviderFixture = cqrsProviderFixture;

    [Fact]
    public async Task Preregistered_Successful_ReturnsOk()
    {
        // Arrange
        _cqrsProviderFixture.Lock();
        CqrsProvider.Configure(Array.Empty<Assembly>(), t => Activator.CreateInstance(t));
        CqrsProvider.Register<PreregiteredPreregiterTestCommandHandler>();

        // Act
        var response = await PreregiterTestCommand.SendAsync(TestContext.Current.CancellationToken);

        // Cleanup
        _cqrsProviderFixture.Release();

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.IsType<OkResult>(response.Result);
    }

    [Fact]
    public async Task Unregistered_Failing_ReturnsError()
    {
        // Arrange
        _cqrsProviderFixture.Lock();
        CqrsProvider.Configure(Array.Empty<Assembly>(), t => Activator.CreateInstance(t));

        // Act
        var response = await PreregiterTestCommand.SendAsync(TestContext.Current.CancellationToken);

        // Cleanup
        _cqrsProviderFixture.Release();

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<UnknownErrorResult>(response.Error);
    }
}