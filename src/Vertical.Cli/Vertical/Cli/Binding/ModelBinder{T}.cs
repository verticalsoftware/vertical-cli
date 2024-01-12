namespace Vertical.Cli.Binding;

/// <summary>
/// Binds argument values to a model instance.
/// </summary>
/// <typeparam name="TModel">Model type.</typeparam>
public abstract class ModelBinder<TModel> : ModelBinder where TModel : class
{
    /// <summary>
    /// Creates a new <typeparamref name="TModel"/> instance using data in the binding
    /// context.
    /// </summary>
    /// <param name="bindingContext">Binding context.</param>
    /// <returns><see cref="TModel"/></returns>
    public abstract TModel BindInstance(IBindingContext bindingContext);

    /// <inheritdoc />
    public sealed override Type ModelType => typeof(TModel);
}