using Microsoft.Extensions.DependencyInjection;

namespace Vertical.Cli;

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
    }
}