namespace Vertical.Cli.Invocation;

internal sealed class ServiceContext
{
    public static ServiceContext Default => new()
    {
        ServiceProviderFactory = null,
        Dispose = false
    };
    
    /// <summary>
    /// Gets a method that creates or provides a service provider.
    /// </summary>
    public required Func<InvocationContext, IServiceProvider>? ServiceProviderFactory { get; init; }
    
    /// <summary>
    /// Gets whether to dispose of the provider.
    /// </summary>
    public required bool Dispose { get; init; }
}