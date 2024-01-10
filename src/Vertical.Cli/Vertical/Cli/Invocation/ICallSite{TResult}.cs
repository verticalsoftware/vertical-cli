namespace Vertical.Cli.Invocation;

/// <summary>
/// A wrapper around a command, help, or error handler.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface ICallSite<out TResult>
{
    /// <summary>
    /// Gets whether the underlying handler invokes help.
    /// </summary>
    bool IsHelpSite { get; }
    
    /// <summary>
    /// Gets the model type.
    /// </summary>
    Type ModelType { get; }

    /// <summary>
    /// Creates a function that passes the given parameter to the
    /// underlying function.
    /// </summary>
    /// <param name="model">Model parameter to capture.</param>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <returns>A function that can be invoked to execute the underlying function.</returns>
    Func<CancellationToken, TResult> WrapParameter<TModel>(TModel model);
}