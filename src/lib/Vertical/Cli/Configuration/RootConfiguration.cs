using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;
using Vertical.Cli.Middleware;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

internal sealed class RootConfiguration(RootCommand rootCommand) : IRootConfigurationView
{
    private readonly List<(Type ModelType, Action<ModelConfiguration> Action)> _modelBuilders = [];
    private readonly List<IDirectiveSymbol> _directiveSymbols = [];
    private readonly Dictionary<Type, Delegate> _argumentConverters = [];
    private readonly Dictionary<(Type, Type), Delegate> _collectionConverters = [];
    
    public RootCommand RootCommand => rootCommand;

    public MiddlewareBuilder MiddlewareBuilder { get; } = MiddlewareBuilder.CreateDefault();

    public void AddModelBuilder<TModel>(Action<ModelBuilder<TModel>> configure) where TModel : class
    {
        _modelBuilders.Add((
            typeof(TModel),
            configuration => configure(new ModelBuilder<TModel>(configuration))));
    }

    public void AddDirectiveSymbol(IDirectiveSymbol symbol) => _directiveSymbols.Add(symbol);

    /// <inheritdoc />
    public ModelConfiguration GetModelConfiguration(Type modelType)
    {
        var builderLookup = _modelBuilders.ToLookup(b => b.ModelType, b => b.Action);

        return modelType
            .GetInterfacesAndSelf()
            .Aggregate(
                new ModelConfiguration(),
                (config, type) =>
                {
                    foreach (var builder in builderLookup[type])
                    {
                        builder(config);
                    }

                    return config;
                });
    }

    /// <inheritdoc />
    public MiddlewareDelegate GetMiddlewarePipeline() => MiddlewareBuilder.BuildPipeline();

    /// <inheritdoc />
    public OptionsManager OptionsManager { get; } = new();

    /// <inheritdoc />
    public IConsole Console { get; set; } = new SystemConsole();

    /// <inheritdoc />
    public OutputFormatter OutputFormatter { get; set; } = OutputFormatter.VerticalTheme;

    /// <inheritdoc />
    public HelpSystemOptions HelpOptions { get; set; } = new();

    /// <inheritdoc />
    public Stream GetAnnotationStream(string resource) => AnnotationStreamProvider(resource);

    /// <inheritdoc />
    public IReadOnlyList<IDirectiveSymbol> GetDirectives() => _directiveSymbols;

    public Func<string, Stream> AnnotationStreamProvider { get; set; } = File.OpenRead;

    public ServiceContext ServiceContext { get; set; } = ServiceContext.Default;

    public void AddArgumentConverter<TValue>(ArgumentConverter<TValue> converter)
    {
        _argumentConverters.Add(typeof(TValue), converter ?? throw new ArgumentNullException(nameof(converter)));
    }

    public void AddCollectionConverter<TElement, TCollection>(CollectionConverter<TElement, TCollection> converter)
        where TCollection : IEnumerable<TElement>
    {
        _collectionConverters[(typeof(TElement), typeof(TCollection))] = converter ?? 
                                                                         throw new ArgumentNullException(nameof(converter));
    }

    /// <inheritdoc />
    public ArgumentConverter<TValue> GetArgumentConverter<TValue>()
    {
        return _argumentConverters.GetValueOrDefault(typeof(TValue)) as ArgumentConverter<TValue>
               ?? throw new InvalidOperationException($"Argument converter for type {typeof(TValue)} not configured.");
    }

    /// <inheritdoc />
    public CollectionConverter<TElement, TCollection> GetCollectionConverter<TElement, TCollection>()
        where TCollection : IEnumerable<TElement>
    {
        return _collectionConverters.GetValueOrDefault((typeof(TElement), typeof(TCollection)))
                   as CollectionConverter<TElement, TCollection>
               ?? throw new InvalidOperationException($"Collection converter {typeof(TCollection)} not configured.");
    }
}