using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli;

/// <summary>
/// Object used to configure the command line.
/// </summary>
public sealed partial class CommandLineBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class.
    /// </summary>
    /// <param name="rootCommand">The application's root command.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rootCommand"></paramref>is null.</exception>
    public CommandLineBuilder(IRootCommand rootCommand)
    {
        _middlewareConfiguration = new MiddlewareConfiguration(Options);
        
        RootCommand = rootCommand ?? throw new ArgumentNullException(nameof(rootCommand));
        ConfigureModel<EmptyModel>(model => model.UseBinder(_ => EmptyModel.Instance));
    }
    
    private readonly Lazy<Dictionary<Type, Delegate>> _lazyValueConverters = new(() => []);
    private readonly ModelConfigurationBuilder _modelConfigurationBuilder = new();
    private readonly MiddlewareConfiguration _middlewareConfiguration;

    /// <summary>
    /// Gets the root command.
    /// </summary>
    public IRootCommand RootCommand { get; }

    /// <summary>
    /// Gets the options instance.
    /// </summary>
    public CommandLineOptions Options { get; } = new();

    /// <summary>
    /// Adds a configuration for a model type.
    /// </summary>
    /// <param name="configure">An action that operates on the provided configuration object.</param>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// This is an additive configuration method and can be called multiple times.
    /// </remarks>
    public CommandLineBuilder ConfigureModel<TModel>(Action<ModelConfiguration<TModel>> configure) 
        where TModel : class
    {
        _modelConfigurationBuilder.AddConfiguration(configure);
        return this;
    }

    /// <summary>
    /// Registers an action used to configure the middleware pipeline.
    /// </summary>
    /// <param name="configure">Action that receives a <see cref="MiddlewareConfiguration"/> object.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineBuilder ConfigureMiddleware(Action<MiddlewareConfiguration> configure)
    {
        configure(_middlewareConfiguration);
        return this;
    }

    /// <summary>
    /// Configures the framework's options.
    /// </summary>
    /// <param name="configureOptions">An action that receives the current options instance.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineBuilder ConfigureOptions(Action<CommandLineOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        configureOptions(Options);
        return this;
    }

    /// <summary>
    /// Adds a value converter.
    /// </summary>
    /// <param name="converter">The function that converts a string to the value type.</param>
    /// <typeparam name="T">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter"></paramref> is null</exception>
    public CommandLineBuilder AddValueConverter<T>(ValueConverter<T> converter)
    {
        _lazyValueConverters.Value[typeof(T)] = converter ?? throw new ArgumentNullException(nameof(converter));
        return this;
    }

    /// <summary>
    /// Builds the application.
    /// </summary>
    /// <returns><see cref="CommandLineApplication"/></returns>
    public CommandLineApplication Build() => new(this);
}