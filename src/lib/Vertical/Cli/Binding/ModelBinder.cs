namespace Vertical.Cli.Binding;

/// <summary>
/// Defines a delegate that creates a model instance from a binding context.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public delegate TModel ModelBinder<TModel>(BindingContext<TModel> bindingContext) where TModel : class;