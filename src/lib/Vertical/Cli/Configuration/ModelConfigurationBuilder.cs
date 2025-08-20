using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Manages configuration of model types.
/// </summary>
internal sealed class ModelConfigurationBuilder
{
    internal ModelConfigurationBuilder()
    {
        AddConfiguration<EmptyModel>(config => config.UseActivator(() => EmptyModel.Instance));    
    }
    
    private delegate void ConfigurationAction(
        IModelConfigurationFactory modelConfigurationFactory,
        HandlerContextBuilder builder);
    
    private readonly record struct Entry(Type ModelType, ConfigurationAction Configure);
    private readonly List<Entry> _entries = [];
    
    /// <summary>
    /// Adds a model configuration registration.
    /// </summary>
    /// <param name="configureModel">Action that configures the model.</param>
    /// <typeparam name="TModel">Model type.</typeparam>
    public void AddConfiguration<TModel>(Action<ModelConfiguration<TModel>> configureModel) where TModel : class
    {
        _entries.Add(new Entry(typeof(TModel), BuildConfigureAction));
        return;

        void BuildConfigureAction(IModelConfigurationFactory modelConfigurationFactory,
            HandlerContextBuilder contextBuilder)
        {
            var configuration = modelConfigurationFactory.CreateInstance<TModel>();
            configureModel(configuration);
            
            configuration.ConfigureContext(contextBuilder);
        }
    }

    /// <summary>
    /// Configures a request builder.
    /// </summary>
    /// <param name="builder">Builder instance to configure</param>
    /// <param name="modelType">Model type</param>
    /// <param name="modelConfigurationFactory">The model configuration factory</param>
    /// <returns><paramref name="builder"/></returns>
    public void ConfigureRequestBuilder(HandlerContextBuilder builder,
        Type modelType,
        IModelConfigurationFactory modelConfigurationFactory) 
    {
        foreach (var type in modelType.GetSelfAndAllBaseTypes())
        {
            var actions = _entries
                .Where(e => e.ModelType == type)
                .Select(e => e.Configure);
            
            ConfigureRequestBuilder(actions, modelConfigurationFactory, builder);
        }
    }

    private static void ConfigureRequestBuilder(
        IEnumerable<ConfigurationAction> actions, 
        IModelConfigurationFactory modelConfigurationFactory,
        HandlerContextBuilder builder)
    {
        foreach (var action in actions)
        {
            action(modelConfigurationFactory, builder);
        }
    }
}