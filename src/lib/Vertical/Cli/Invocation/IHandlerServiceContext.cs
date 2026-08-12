namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents a service context for command handlers.
/// </summary>
/// <typeparam name="TModel">The command's model type.</typeparam>
public interface IHandlerServiceContext<in TModel> : IAsyncDisposable where TModel : class
{
    /// <summary>
    /// Gets the handler instance.
    /// </summary>
    /// <returns>A <see cref="IHandler{TModel}"/> instance.</returns>
    IHandler<TModel> GetHandler();
}