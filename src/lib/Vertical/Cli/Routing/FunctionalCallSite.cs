namespace Vertical.Cli.Routing;

internal sealed class FunctionalCallSite<TModel>(AsyncCallSite<TModel> implementation) : 
    ICallSite<TModel> where TModel : class
{
    /// <inheritdoc />
    public async Task<int> HandleAsync(TModel model, CancellationToken cancellationToken)
    {
        return await implementation(model, cancellationToken);
    }
}