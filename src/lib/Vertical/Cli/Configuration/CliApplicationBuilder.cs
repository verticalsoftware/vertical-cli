using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an object used to configure a CLI application.
/// </summary>
public sealed class CliApplicationBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CliApplicationBuilder"/> class.
    /// </summary>
    /// <param name="applicationName"></param>
    public CliApplicationBuilder(string applicationName)
    {
        this.applicationName = applicationName;
    }

    private readonly string applicationName;
    private readonly RoutingConfiguration routingConfiguration = new();
    private readonly List<IValueConverter> valueConverters = [];
    private readonly Dictionary<Type, ModelConfiguration> modelConfigurations = new();
    private string? helpSwitch;
    
    /// <summary>
    /// Creates a route to other commands.
    /// </summary>
    /// <param name="path">A string that describes the path to match.</param>
    /// <param name="helpTag">Data used by the help provider.</param>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder Route(string path, object? helpTag = null)
    {
        return Route<EmptyModel>(path, null, helpTag);
    }
    
    /// <summary>
    /// Creates a route with an implementation that receives a model.
    /// </summary>
    /// <param name="path">A string that describes the path to match.</param>
    /// <param name="callSite">
    /// A function that receives a model instance containing bound arguments and performs the application's logic.
    /// </param>
    /// <param name="helpTag">Data used by the help provider.</param>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder Route<TModel>(string path, 
        CallSite<TModel>? callSite = null,
        object? helpTag = null)
        where TModel : class
    {
        var wrappedCallSite = callSite != null
            ? new AsyncCallSite<TModel>((model, _) => Task.FromResult(callSite(model)))
            : null;

        return RouteAsync(path, wrappedCallSite, helpTag);
    }
    
    /// <summary>
    /// Creates a route to other commands.
    /// </summary>
    /// <param name="path">A string that describes the path to match.</param>
    /// <param name="helpTag">Data used by the help provider.</param>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder RouteAsync(string path, object? helpTag = null)
    {
        return RouteAsync(path, default(AsyncCallSite<EmptyModel>), helpTag);
    }

    /// <summary>
    /// Creates a route with an implementation that receives a model.
    /// </summary>
    /// <param name="path">A string that describes the path to match.</param>
    /// <param name="callSite">
    /// A function that receives a model instance containing bound arguments and performs the application's logic.
    /// </param>
    /// <param name="helpTag">Data used by the help provider.</param>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder RouteAsync<TModel>(string path, 
        AsyncCallSite<TModel>? callSite = null,
        object? helpTag = null)
        where TModel : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        routingConfiguration.AddRoute(path, callSite, helpTag);
        return this;
    }

    /// <summary>
    /// Creates a route with an implementation defined by a service.
    /// </summary>
    /// <param name="path">A string that describes the path to match.</param>
    /// <param name="callSiteFactory">A function that returns the handler implementation instance.</param>
    /// <param name="helpTag">Data used by the help provider.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder RouteAsync<TModel>(string path,
        Func<IAsyncCallSite<TModel>> callSiteFactory,
        object? helpTag = null)
        where TModel : class
    {
        return RouteAsync(path, new AsyncCallSite<TModel>(async (model, cancellationToken) => 
                await callSiteFactory().HandleAsync(model, cancellationToken)),
            helpTag);
    }

    /// <summary>
    /// Maps a help switch.
    /// </summary>
    /// <param name="helpProvider">A function that returns a help provider.</param>
    /// <param name="identifier">The switch identifier associated with the help function</param>
    /// <param name="helpTag">Data provided to the help provider</param>
    /// <returns>A reference to this instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="identifier"/> is not a valid short/long form switch</exception>
    public CliApplicationBuilder MapHelpSwitch(
        Func<IHelpProvider>? helpProvider = null,
        string? identifier = null,
        object? helpTag = null)
    {
        helpSwitch = identifier ?? "--help";
        
        var route = HelpSystemExtensions.Create(helpProvider, 
            helpSwitch, 
            helpTag);
        
        routingConfiguration.AddDefinition(route);

        return this;
    }

    /// <summary>
    /// Configures the symbols that are mapped to properties of a model.
    /// </summary>
    /// <param name="configure">An action used for configuration.</param>
    /// <typeparam name="TModel">The model type being mapped.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder MapModel<TModel>(Action<ModelConfiguration<TModel>> configure) where TModel : class
    {
        if (!modelConfigurations.TryGetValue(typeof(TModel), out var configuration))
        {
            modelConfigurations.Add(typeof(TModel), configuration = new ModelConfiguration<TModel>());
        }

        configure((ModelConfiguration<TModel>)configuration);
        
        return this;
    }

    /// <summary>
    /// Adds one or more <see cref="IValueConverter"/> services.
    /// </summary>
    /// <param name="converters">Value converter(s) to add.</param>
    /// <returns>A reference to this instance.</returns>
    public CliApplicationBuilder AddConverters(IEnumerable<IValueConverter> converters)
    {
        valueConverters.AddRange(converters);
        return this;
    }

    /// <summary>
    /// Builds the application.
    /// </summary>
    /// <returns><see cref="CliApplication"/></returns>
    public CliApplication Build() => new(applicationName,
        routingConfiguration,
        modelConfigurations,
        valueConverters,
        helpSwitch);
}