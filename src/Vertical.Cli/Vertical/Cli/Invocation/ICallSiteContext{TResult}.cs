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
    
    Func<CancellationToken, TResult> BindModelParameter<TModel>() where TModel : class;

    Func<CancellationToken, TResult> BindModelParameter<TModel>(Func<IBindingContext, TModel> bindingFunction)
        where TModel : class;
}