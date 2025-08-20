namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an object that 
/// </summary>
public interface IModelConfigurationFactory
{
    /// <summary>
    /// Creates a model configuration.
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns><see cref="ModelConfiguration{TModel}"/></returns>
    ModelConfiguration<TModel> CreateInstance<TModel>() where TModel : class;
}