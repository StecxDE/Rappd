using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Rappd.CQRS;

/// <summary>
/// Extension methods for configuring CQRS in an application.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures CQRS for the given application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder ConfigureCqrs(this IApplicationBuilder app)
        => app.ConfigureCqrs(Array.Empty<Assembly>());
    /// <summary>
    /// Configures CQRS for the given application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="type">A type contained in the assembly containing the handlers.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder ConfigureCqrs(this IApplicationBuilder app, Type type)
        => app.ConfigureCqrs(new[] { type.Assembly });
    /// <summary>
    /// Configures CQRS for the given application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="assemblies">The assemblies containing the handlers.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder ConfigureCqrs(this IApplicationBuilder app, params Assembly[] assemblies)
    {
        CqrsProvider.Configure(assemblies, t => ActivatorUtilities.CreateInstance(app.ApplicationServices, t));
        return app;
    }
}