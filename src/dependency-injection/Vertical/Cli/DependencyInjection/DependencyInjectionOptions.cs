using Microsoft.Extensions.DependencyInjection;

namespace Vertical.Cli.DependencyInjection;

internal sealed class DependencyInjectionOptions
{
    public IServiceCollection ServiceCollection { get; } = new ServiceCollection();
}