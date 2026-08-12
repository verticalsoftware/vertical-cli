using Microsoft.Extensions.DependencyInjection;

namespace Vertical.Cli.DependencyInjection;

/// <summary>
/// Defines extensions for <see cref="CommandLineApplication"/>
/// </summary>
public static class CommandLineApplicationExtensions
{
    extension(CommandLineApplication app)
    {
        /// <summary>
        /// Gets the application's service collection.
        /// </summary>
        public IServiceCollection Services => app.GetOptions<DependencyInjectionOptions>().ServiceCollection;

        /// <summary>
        /// Registers middleware into the pipeline that manages the lifecycle of the
        /// service provider.
        /// </summary>
        /// <returns>A reference to the <see cref="CommandLineApplication"/> instance.</returns>
        public CommandLineApplication UseServices()
        {
            app.ConfigureMiddleware(builder => builder.AddFirst(ServiceProviderMiddleware.InvokeAsync));
            return app;
        }
    }
}