using Vertical.Cli.Binding;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents call site data.
/// </summary>
/// <typeparam name="TResult">Invocation return type.</typeparam>
public interface ICallSiteContext<TResult>
{
    /// <summary>
    /// Gets the call site.
    /// </summary>
    ICallSite<TResult> CallSite { get; }
    
    /// <summary>
    /// Gets the argument binding context.
    /// </summary>
    IBindingContext BindingContext { get; }
        
    /// <summary>
    /// Gets the binding exception that occurred or <c>null</c>.
    /// </summary>
    Exception? BindingException { get; }
    
    /// <summary>
    /// Gets the Cli options used within the context.
    /// </summary>
    CliOptions Options { get; }

    /// <summary>
    /// Creates an instance of <typeparamref name="TModel"/>.
    /// </summary>
    /// <param name="bindingFunction">A function that uses the binding context data to create the model.</param>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <returns>The call site function that wraps the created model value.</returns>
    /// <remarks>
    /// The binding function is not called when an application has registered a custom binder instance.
    /// </remarks>
    Func<CancellationToken, TResult> BindModelToCallSite<TModel>(
        Func<IBindingContext, TModel>? bindingFunction = null)
        where TModel : class;
}