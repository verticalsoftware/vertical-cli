using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
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

    public CommandLineApplication(RootCommand rootCommand)
    {
        ArgumentNullException.ThrowIfNull(rootCommand);
        _configuration = new RootConfiguration(rootCommand);
    }

    /// <summary>
    /// Adds directive handling for the given symbol.
    /// </summary>
    /// <param name="symbol">The symbol the directive is identified as.</param>
    /// <param name="arity">The parameter arity.</param>
    /// <param name="asyncHandler">A method that handles the directive logic.</param>
    /// <param name="helpTopic">An optional help topic to associate with the directive.</param>
    /// <returns>A reference to this instance.</returns>
    public CommandLineApplication HandleDirective(
        string symbol,
        Func<DirectiveEventInfo, Task> asyncHandler,
        DirectiveParameterArity arity = DirectiveParameterArity.NotSupported,
        SymbolHelpTopic? helpTopic = null)
    {
        _configuration.DirectiveSymbols.Add(new DirectiveSymbol(
            symbol, 
            arity, 
            asyncHandler, 
            helpTopic));
        
        return this;
    }

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
    /// Registers an action that configures a model type.
    /// </summary>
    /// <param name="configure">An action that manipulates the provided <see cref="ModelBuilder{TModel}"/>.</param>
    /// <typeparam name="TModel">Model type being configured.</typeparam>
    public CommandLineApplication ConfigureModel<TModel>(Action<ModelBuilder<TModel>> configure) 
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
    public CommandLineApplication AddArgumentConverter<TValue>(ArgumentConverter<TValue> converter)
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
        CollectionConverter<TElement, TCollection> converter)
        where TCollection : IEnumerable<TElement>
    {
        _configuration.AddCollectionConverter(converter);
        return this;
    }

    /// <summary>
    /// Informs the framework of the application's service provider. When set, command handlers
    /// can be resolved using dependency injection.
    /// </summary>
    /// <param name="serviceProviderFactory">
    /// A method that creates or provides a reference to the application's <see cref="IServiceProvider"/>.
    /// </param>
    /// <param name="dispose">
    /// When set <c>true</c>, the framework will manage the lifecycle of the service provider.
    /// </param>
    /// <returns></returns>
    public CommandLineApplication UseServices(Func<IServiceProvider> serviceProviderFactory, bool dispose = true)
    {
        _configuration.ServiceContext = new ServiceContext
        {
            ServiceProviderFactory = serviceProviderFactory,
            Dispose = dispose
        };

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
    /// Parse the application's argument array and 
    /// </summary>
    /// <param name="args">The application's input arguments.</param>
    /// <returns>The result code to return.</returns>
    public async Task<int> RunAsync(string[] args)
    {
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