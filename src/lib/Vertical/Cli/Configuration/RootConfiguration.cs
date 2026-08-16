using Vertical.Cli.Conversion;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.IO;
using Vertical.Cli.Middleware;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

internal sealed class RootConfiguration(RootCommand rootCommand) : IRootConfigurationView
{
    private readonly List<(Type ModelType, Action<ModelConfiguration> Action)> _modelBuilders = [];
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

    /// <inheritdoc />
    public ModelConfiguration GetModelConfiguration(Type modelType)
    {
        var builderLookup = _modelBuilders.ToLookup(b => b.ModelType, b => b.Action);

        return modelType
            .GetInterfacesAndSelf()
            .Aggregate(
                new ModelConfiguration(modelType),
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
    public Stream GetAnnotationResourceStream(string resource) => AnnotationStreamProvider(resource);

    /// <inheritdoc />
    public IReadOnlyList<MiddlewareSymbol> GetMiddlewareSymbols() => MiddlewareBuilder.Symbols;

    /// <inheritdoc />
    public bool HasArgumentConverter(Type type) => _argumentConverters.ContainsKey(type);

    /// <inheritdoc />
    public bool HasCollectionConverter(Type elementType, Type collectionType) =>
        _collectionConverters.ContainsKey((elementType, collectionType));

    public Func<string, Stream> AnnotationStreamProvider { get; set; } = File.OpenRead;
    
    public void AddArgumentConverter<TValue>(Converter<string, TValue> converter)
    {
        _argumentConverters.Add(typeof(TValue), converter ?? throw new ArgumentNullException(nameof(converter)));
    }

    public void AddCollectionConverter<TElement, TCollection>(Converter<IEnumerable<TElement>, TCollection> converter)
        where TCollection : IEnumerable<TElement>
    {
        _collectionConverters[(typeof(TElement), typeof(TCollection))] =
            converter ??
            throw new ArgumentNullException(nameof(converter));
    }

    /// <inheritdoc />
    public Converter<string,TValue> GetArgumentConverter<TValue>()
    {
        return _argumentConverters.GetValueOrDefault(typeof(TValue)) as Converter<string, TValue>
               ?? throw new InvalidOperationException($"Argument converter for type {typeof(TValue)} not configured.");
    }

    /// <inheritdoc />
    public Converter<IEnumerable<TElement>, TCollection> GetCollectionConverter<TElement, TCollection>()
        where TCollection : IEnumerable<TElement>
    {
        return _collectionConverters.GetValueOrDefault((typeof(TElement), typeof(TCollection)))
                   as Converter<IEnumerable<TElement>, TCollection>
               ?? throw new InvalidOperationException($"Collection converter {typeof(TCollection)} not configured.");
    }

    /// <inheritdoc />
    public ConversionResult<TValue> TryConvertArgument<TValue>(
        ICliSymbol symbol, 
        string argumentValue,
        List<CommandLineError> errorList)
    {
        var converter = GetArgumentConverter<TValue>();
        try
        {
            return new ConversionResult<TValue>(converter(argumentValue), null);
        }
        catch (Exception exception)
        {
            var error = ArgumentConversionError.Create(
                symbol,
                typeof(TValue),
                argumentValue,
                HelpOptions.HelpProvider,
                exception);
            
            errorList.Add(error);
            return new ConversionResult<TValue>(default!, error);
        }
    }

    /// <inheritdoc />
    public ConversionResult<TCollection> TryConvertCollection<TElement, TCollection>(
        ICliSymbol symbol,
        IEnumerable<string> argumentValues,
        List<CommandLineError> errorList)
        where TCollection : IEnumerable<TElement>
    {
        var argumentConverter = GetArgumentConverter<TElement>();
        var collectionConverter = GetCollectionConverter<TElement, TCollection>();
        var results = argumentValues
            .Select(next =>
            {
                try
                {
                    return (value: argumentConverter(next), error: default(CommandLineError));
                }
                catch (Exception exception)
                {
                    var error = ArgumentConversionError.Create(
                        symbol,
                        typeof(TElement),
                        next,
                        HelpOptions.HelpProvider,
                        exception);

                    return (default!, error);
                }
            })
            .ToArray();

        var errors = results
            .Where(result => result.error is not null)
            .Select(result => result.error)
            .Cast<CommandLineError>()
            .ToArray();

        if (errors.Length == 0)
        {
            return new ConversionResult<TCollection>(
                collectionConverter(results.Select(result => result.value)),
                null);
        }

        errorList.AddRange(errors);
        
        return new ConversionResult<TCollection>(
            collectionConverter([]),
            new AggregateCommandLineError(errors));
    }
}