using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Contains argument bindings that can be used to construct parameter models.
/// </summary>
public interface IBindingContext<TResult> : IBindingPath
{
    /// <summary>
    /// Gets the bindings available in the context.
    /// </summary>
    IReadOnlyDictionary<string, ArgumentBinding> BindingDictionary { get; }
    
    /// <summary>
    /// Gets a binding that matches the given id.
    /// </summary>
    /// <param name="bindingId">Binding id to match.</param>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <returns><see cref="ArgumentBinding{T}"/></returns>
    /// <exception cref="InvalidOperationException">The binding is not found.</exception>
    ArgumentBinding<T> GetBinding<T>(string bindingId);

    /// <summary>
    /// Gets the single binding value.
    /// </summary>
    /// <param name="bindingId">Binding id to match.</param>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <returns>The single value.</returns>
    T GetValue<T>(string bindingId);

    /// <summary>
    /// Gets multiple binding values.
    /// </summary>
    /// <param name="bindingId">Binding id to match.</param>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <returns>The binding values.</returns>
    IEnumerable<T> GetValues<T>(string bindingId);
    
    /// <summary>
    /// Gets a collection of the original semantic arguments not consumed by parsing.
    /// </summary>
    SemanticArgumentCollection OriginalSemanticArguments { get; }
    
    /// <summary>
    /// Gets the subject command.
    /// </summary>
    ICommandDefinition<TResult> Subject { get; }
    
    /// <summary>
    /// Gets the binding exception that occurred or <c>null</c>.
    /// </summary>
    Exception? BindingException { get; }

    /// <summary>
    /// Throws any binding exceptions. 
    /// </summary>
    void ThrowBindingExceptions();

    /// <summary>
    /// Creates a call site.
    /// </summary>
    /// <param name="model">The model to pass to the handler.</param>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <returns>A function that invokes the subject's handler with the created model instance.</returns>
    Func<TResult> CreateCallSite<TModel>(TModel model) where TModel : class;
}