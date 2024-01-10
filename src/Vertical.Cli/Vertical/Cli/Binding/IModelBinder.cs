namespace Vertical.Cli.Binding;

/// <summary>
/// Binds argument values to a model instance.
/// </summary>
/// <typeparam name="TModel">Model type.</typeparam>
public interface IModelBinder<out TModel> where TModel : class
{
    
}