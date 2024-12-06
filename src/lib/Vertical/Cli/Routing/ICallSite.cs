namespace Vertical.Cli.Routing;

/// <summary>
/// Represents an asynchronous call site.
/// </summary>
public interface ICallSite<in TModel> where TModel : class
{
    /// <summary>
    /// When implemented, handles the call site's functionality.
    /// </summary>
    /// <param name="model">Model</param>
    /// <param name="cancellationToken">Token observed for cancellation.</param>
    /// <returns><see cref="Task"/></returns>
    Task<int> HandleAsync(TModel model, CancellationToken cancellationToken);
}