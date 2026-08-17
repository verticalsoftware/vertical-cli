using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection;

/// <summary>
/// Defines extensions for <see cref="InvocationContext"/>
/// </summary>
public static class InvocationContextExtensions
{
    /// <summary>
    /// Creates an instance of the application's service provider.
    /// </summary>
    /// <returns><see cref="IServiceProvider"/></returns>
    public static IServiceProvider CreateServiceProvider(this InvocationContext context)
    {
        var options = context.AppData.Configure<DependencyInjectionOptions>();
        var serviceCollection = options.ServiceCollection;

        options.ConfigurationAction(context, serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }
}