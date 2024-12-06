using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the configuration of an application.
/// </summary>
public sealed class CliApplication
{
    internal CliApplication(
        string applicationName,
        RoutingConfiguration routingConfiguration,
        IReadOnlyDictionary<Type, ModelConfiguration> modelConfigurations,
        IReadOnlyCollection<IValueConverter> valueConverters,
        string? helpSwitch)
    {
        Router = routingConfiguration.CreateRouter();
        ProgramArgument = ArgumentSyntax.Parse(applicationName);
        Name = applicationName;
        RoutingConfiguration = routingConfiguration;
        ModelConfigurations = modelConfigurations;
        Converters = valueConverters;
        HelpSwitch = helpSwitch;
    }

    internal string? HelpSwitch { get; }

    /// <summary>
    /// Gets the program argument.
    /// </summary>
    public ArgumentSyntax ProgramArgument { get; }

    /// <summary>
    /// Gets the application name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the router instance.
    /// </summary>
    public Router Router { get; }

    /// <summary>
    /// Gets the router configuration.
    /// </summary>
    public IRoutingConfiguration RoutingConfiguration { get; }

    /// <summary>
    /// Gets the model configurations.
    /// </summary>
    public IReadOnlyDictionary<Type, ModelConfiguration> ModelConfigurations { get; }

    /// <summary>
    /// Gets the converters registered by the application.
    /// </summary>
    public IReadOnlyCollection<IValueConverter> Converters { get; }

    /// <inheritdoc />
    public override string ToString() => Name;
}