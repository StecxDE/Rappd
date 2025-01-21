using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Rappd.Data
{
    /// <summary>
    /// Extension methods to add interface handling.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all needed services for the interface handling to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assembly">The assembly containing the interface implementations.</param>
        /// <returns>A <see cref="IServiceCollection"/> that can be used to further add services.</returns>
        public static IServiceCollection AddInterfaceHandling(this IServiceCollection services, Assembly assembly)
            => services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new InterfaceConverterFactory(assembly));
            });
        /// <summary>
        /// Adds all needed services for the interface handling to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">The assemblies containing the interface implementations.</param>
        /// <returns>A <see cref="IServiceCollection"/> that can be used to further add services.</returns>
        public static IServiceCollection AddInterfaceHandling(this IServiceCollection services, params Assembly[] assemblies)
            => services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new InterfaceConverterFactory(assemblies));
            });
        /// <summary>
        /// Adds all needed services for the interface handling to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="type">A type in the assembly containing the interface implementations.</param>
        /// <returns>A <see cref="IServiceCollection"/> that can be used to further add services.</returns>
        public static IServiceCollection AddInterfaceHandling(this IServiceCollection services, Type type)
            => services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new InterfaceConverterFactory(type.Assembly));
            });
        /// <summary>
        /// Adds all needed services for the interface handling to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="T">A type in the assembly containing the interface implementations.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>A <see cref="IServiceCollection"/> that can be used to further add services.</returns>
        public static IServiceCollection AddInterfaceHandling<T>(this IServiceCollection services)
            => services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new InterfaceConverterFactory(typeof(T).Assembly));
            });
    }
}