using Vertical.Cli.Configuration;
using Vertical.Cli.Configuration.Assertion;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;
using Vertical.Cli.Middleware;
using Vertical.Cli.Middleware.Components;

namespace Vertical.Cli;

/// <summary>
/// Represents a command line application.
/// </summary>
public class CommandLineApplication
{
    private readonly RootConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineApplication"/> class.
    /// </summary>
    /// <param name="rootCommand">The application's root command.</param>
    public CommandLineApplication(RootCommand rootCommand)
    {
        ArgumentNullException.ThrowIfNull(rootCommand);
        _configuration = new RootConfiguration(rootCommand);
    }

    internal IRootConfigurationView GetConfiguration() => _configuration;

    /// <summary>
    /// Gets the root command.
    /// </summary>
    public RootCommand RootCommand => _configuration.RootCommand;

    /// <summary>
    /// Registers an action that configures the help system.
    /// </summary>
    /// <param name="configure">A delegate that manipulates the given <see cref="HelpSystemOptions"/> object.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication ConfigureHelp(Action<HelpSystemOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_configuration.HelpOptions);
        return this;
    }

    /// <summary>
    /// Configures the middleware pipeline.
    /// </summary>
    /// <param name="configure">An action that manipulates the given <see cref="MiddlewareBuilder"/>.</param>
    /// <returns></returns>
    public CommandLineApplication ConfigureMiddleware(Action<MiddlewareBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_configuration.MiddlewareBuilder);
        return this;
    }

    /// <summary>
    /// Registers an action that configures the parser for a model type.
    /// </summary>
    /// <param name="configure">An action that manipulates the provided <see cref="ModelBuilder{TModel}"/>.</param>
    /// <typeparam name="TModel">Model type being configured.</typeparam>
    public CommandLineApplication ConfigureParser<TModel>(Action<ModelBuilder<TModel>> configure) 
        where TModel : class
    {
        _configuration.AddModelBuilder(configure);
        return this;
    }

    /// <summary>
    /// Adds an argument converter to the application.
    /// </summary>
    /// <param name="converter">A delegate that converts a string argument value to <typeparamref name="TValue"/>.</param>
    /// <typeparam name="TValue">The target value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication AddArgumentConverter<TValue>(Converter<string, TValue> converter)
    {
        _configuration.AddArgumentConverter(converter);
        return this;
    }

    /// <summary>
    /// Adds a collection converter to the application.
    /// </summary>
    /// <param name="converter">A delegate that converts enumerations of <typeparamref name="TElement"/>
    /// into a specific collection type.</param>
    /// <typeparam name="TElement">Element type</typeparam>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <returns>A reference to this instance</returns>
    public CommandLineApplication AddCollectionConverter<TElement, TCollection>(
        Converter<IEnumerable<TElement>, TCollection> converter)
        where TCollection : IEnumerable<TElement>
    {
        _configuration.AddCollectionConverter(converter);
        return this;
    }

    /// <summary>
    /// Registers an asynchronous directive handler.
    /// </summary>
    /// <param name="identifier">The identifier for the directive.</param>
    /// <param name="handler">An asynchronous handler that is invoked when a token is matched.</param>
    /// <param name="helpTopic">Optional help topic to associate with the directive.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication HandleDirective(
        string identifier,
        Func<DirectiveEventInfo, Task> handler,
        SymbolHelpTopic? helpTopic = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentNullException.ThrowIfNull(handler);
        
        var directive = new DirectiveSymbol(identifier, parameterArity: null, handler, helpTopic);
        _configuration.AddDirectiveSymbol(directive);
        return this;
    }

    /// <summary>
    /// Registers an asynchronous directive handler.
    /// </summary>
    /// <param name="identifier">The identifier for the directive.</param>
    /// <param name="handler">An asynchronous handler that is invoked when a token is matched.</param>
    /// <param name="defaultProvider">A function that returns a default value.</param>
    /// <param name="helpTopic">Optional help topic to associate with the directive.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication HandleParameterizedDirective<TValue>(
        string identifier,
        Func<DirectiveEventInfo<TValue>, Task> handler,
        Func<TValue>? defaultProvider = null,
        SymbolHelpTopic? helpTopic = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentNullException.ThrowIfNull(handler);

        var directive = new ParameterizedDirectiveSymbol<TValue>(
            identifier,
            defaultProvider != null ? ParameterArity.ZeroOrOne : ParameterArity.One,
            handler, 
            defaultProvider, 
            helpTopic);
            
        _configuration.AddDirectiveSymbol(directive);
        return this;
    }

    /// <summary>
    /// Registers the given object as the console abstraction.
    /// </summary>
    /// <param name="console">The console abstraction.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication UseConsole(IConsole console)
    {
        _configuration.Console = console ?? throw new ArgumentNullException(nameof(console));
        return this;
    }

    /// <summary>
    /// Registers the given object to format output to the console abstraction.
    /// </summary>
    /// <param name="outputFormatter">The output formatter instance to use.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication UseOutputFormatter(OutputFormatter outputFormatter)
    {
        _configuration.OutputFormatter = outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter));
        return this;
    }
    
    /// <summary>
    /// Configures an options object.
    /// </summary>
    /// <param name="configure">An action that manipulates the object.</param>
    /// <typeparam name="TOptions">Creatable options type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication ConfigureOptions<TOptions>(Action<TOptions> configure) where TOptions : class, new()
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configuration.OptionsManager.Configure(configure);
        return this;
    }

    /// <summary>
    /// Gets a reference to an options object.
    /// </summary>
    /// <typeparam name="TOptions">Creatable options type.</typeparam>
    /// <returns>A reference to the single options instance.</returns>
    public TOptions GetOptions<TOptions>() where TOptions : class, new()
    {
        return _configuration.OptionsManager.GetOptions<TOptions>();
    }

    /// <summary>
    /// Parse the application's argument array and 
    /// </summary>
    /// <param name="args">The application's input arguments.</param>
    /// <returns>The result code to return.</returns>
    public async Task<int> RunAsync(string[] args)
    {
        this.AssertConfiguration();
        
        var context = new InvocationContext(_configuration, args);

        var middlewarePipeline = _configuration
            .MiddlewareBuilder
            .BuildPipeline();

        await middlewarePipeline(
            context,
            async ctx => await RouteCommandTargetMiddleware.InvokeAsync(ctx, _ => Task.CompletedTask));

        return context.Result ?? -1;
    }
}