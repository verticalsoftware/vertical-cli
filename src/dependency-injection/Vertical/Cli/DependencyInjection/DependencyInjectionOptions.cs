using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection;

internal sealed class DependencyInjectionOptions
{
    public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

    public Action<InvocationContext, IServiceCollection> ConfigurationAction { get; set; } =
        (_, _) => { };
}