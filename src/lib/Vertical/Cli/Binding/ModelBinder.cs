namespace Vertical.Cli.Binding;

/// <summary>
/// Defines a method that creates models using data from a <see cref="BindingContext{TModel}"/>
/// </summary>
/// <typeparam name="TModel">The model instance.</typeparam>
public delegate TModel ModelBinder<TModel>(BindingContext<TModel> bindingContext) where TModel : class;