namespace Vertical.Cli.Invocation;

/// <summary>
/// Designates a class as a handler of an application's function.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IHandler<in TModel> where TModel : class
{
    /// <summary>
    /// When implemented by a class, performs the application function.
    /// </summary>
    /// <param name="options">The constructed options model.</param>
    /// <param name="cancellationToken">A token that can be observed for cancellation.</param>
    /// <returns>A task with the application exit code.</returns>
    Task<int> HandleAsync(TModel options, CancellationToken cancellationToken);
}