using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection;

/// <summary>
/// Defines extensions for <see cref="Command"/>
/// </summary>
public static class CommandExtensions
{
    extension(Command command)
    {
        /// <summary>
        /// Registers a delegate that resolves a <see cref="IHandler{TModel}"/> instance from
        /// a service provider.
        /// </summary>
        /// <param name="handlerResolver">
        /// A delegate that resolves the <see cref="IHandler{TModel}"/> dependency from
        /// the given service provider.
        /// </param>
        /// <typeparam name="TModel">Model type.</typeparam>
        public void SetHandler<TModel>(Func<IServiceProvider, IHandler<TModel>> handlerResolver)
            where TModel : class
        {
            command.SetHandler(context =>
            {
                var options = context.ApplicationOptions.GetOptions<DependencyInjectionOptions>();
                var serviceProvider = options.LazyServiceProvider.Value;

                return handlerResolver(serviceProvider);
            });
        }

        /// <summary>
        /// Registers a delegate that resolves a <see cref="IHandler{TModel}"/> instance
        /// from the dependency injection system.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="THandler">Handler implementation type</typeparam>
        public void SetHandler<TModel, THandler>()
            where TModel : class
            where THandler : class, IHandler<TModel>
        {
            command.SetHandler(context =>
            {
                var options = context.ApplicationOptions.GetOptions<DependencyInjectionOptions>();
                var serviceProvider = options.LazyServiceProvider.Value;

                return serviceProvider.GetRequiredService<THandler>();
            });
        }
    }
}