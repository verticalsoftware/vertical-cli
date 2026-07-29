namespace Vertical.Cli.Invocation;

internal sealed class DelegatedHandler<TModel>(Func<TModel, CancellationToken, Task<int>> handler) : IHandler<TModel> 
    where TModel : class
{
    /// <inheritdoc />
    public Task<int> HandleAsync(TModel options, CancellationToken cancellationToken)
    {
        return handler(options, cancellationToken);
    }
}