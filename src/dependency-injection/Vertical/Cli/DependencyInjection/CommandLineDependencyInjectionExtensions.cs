using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection;

/// <summary>
/// Defines extensions for <see cref="CommandLineApplication"/>
/// </summary>
public static class CommandLineDependencyInjectionExtensions
{
    extension(CommandLineApplication app)
    {
        /// <summary>
        /// Registers an action that configures the application's services.
        /// </summary>
        /// <param name="configure">A delegate that receives and manipulates the service collection.</param>
        /// <returns>A reference to this instance.</returns>
        public CommandLineApplication ConfigureServices(Action<IServiceCollection> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            return app.ConfigureServices((_, services) => configure(services));
        }
        
        /// <summary>
        /// Registers an action that configures the application's services.
        /// </summary>
        /// <param name="configure">A delegate that receives the invocation context and the service
        /// collection.</param>
        /// <returns>A reference to this instance.</returns>
        public CommandLineApplication ConfigureServices(Action<InvocationContext, IServiceCollection> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            app.AppData.Configure<DependencyInjectionOptions>(options =>
            {
                var currentAction = options.ConfigurationAction;

                options.ConfigurationAction = (context, services) =>
                {
                    currentAction(context, services);
                    configure(context, services);
                };
            });
            
            return app;
        }
    }
}