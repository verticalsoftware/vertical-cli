using Vertical.Cli.Binding;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the final view of bindings for a mode type.
/// </summary>
public sealed class ModelConfiguration
{
    private readonly List<IBindingSource> _bindingSources = new(32);
    private Delegate? _binder;

    internal ModelConfiguration(Type modelType)
    {
        ModelType = modelType;
    }

    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Gets the binding sources.
    /// </summary>
    public IReadOnlyCollection<IBindingSource> BindingSources => _bindingSources;

    /// <summary>
    /// Adds a binding source.
    /// </summary>
    /// <param name="source">The source to add.</param>
    public void AddBindingSource(IBindingSource source)
    {
        _bindingSources.Add(source ?? throw new ArgumentNullException(nameof(source)));
    }

    /// <summary>
    /// Sets the action that binds property values to the model instance.
    /// </summary>
    /// <param name="binder">The action that binds property values to model instances.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    public void SetBinder<TModel>(ModelBinder<TModel> binder) where TModel : class
    {
        _binder = binder ?? throw new ArgumentNullException(nameof(binder));
    }

    /// <summary>
    /// Gets the action tht binds property values to the model instance.
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns><see cref="ModelBinder{TModel}"/></returns>
    public ModelBinder<TModel> GetModelBinder<TModel>() where TModel : class
    {
        return _binder as ModelBinder<TModel> ?? throw Exceptions.ModelBinderNotConfigured(typeof(TModel));
    }

    internal bool HasModelBinder => _binder is not null;
}