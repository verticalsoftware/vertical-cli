using Microsoft.Extensions.DependencyInjection;

namespace Vertical.Cli.DependencyInjection;

internal sealed class DependencyInjectionOptions
{
    public DependencyInjectionOptions()
    {
        ServiceCollection = new ServiceCollection();
        LazyServiceProvider = new Lazy<IServiceProvider>(() => ServiceCollection.BuildServiceProvider());
    }
    
    public IServiceCollection ServiceCollection { get; }

    public Lazy<IServiceProvider> LazyServiceProvider { get; }
}