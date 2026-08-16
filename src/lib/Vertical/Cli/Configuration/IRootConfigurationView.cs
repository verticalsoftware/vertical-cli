using Vertical.Cli.Configuration.Assertion.Builders;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.IO;
using Vertical.Cli.Middleware;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a read-only view of the application's configuration.
/// </summary>
public interface IRootConfigurationView : IConversionProvider
{
    /// <summary>
    /// Gets the root command.
    /// </summary>
    RootCommand RootCommand { get; }

    /// <summary>
    /// Gets the model configuration.
    /// </summary>
    /// <param name="modelType">The model type to construct the configuration for.</param>
    /// <returns><see cref="ModelConfiguration"/></returns>
    ModelConfiguration GetModelConfiguration(Type modelType);

    /// <summary>
    /// Gets the middleware pipeline.
    /// </summary>
    /// <returns>A delegate referencing the first component in the pipeline.</returns>
     MiddlewareDelegate GetMiddlewarePipeline();
    
    /// <summary>
    /// Gets the options manager.
    /// </summary>
    OptionsManager OptionsManager { get; }
    
    /// <summary>
    /// Gets the console implementation.
    /// </summary>
    IConsole Console { get; }
    
    /// <summary>
    /// Gets the object that formats output to the console abstraction.
    /// </summary>
    OutputFormatter OutputFormatter { get; }
    
    /// <summary>
    /// Gets the help options.
    /// </summary>
    HelpSystemOptions HelpOptions { get; }

    /// <summary>
    /// Gets the stream object for a resource annotation.
    /// </summary>
    /// <param name="resource">The resource being referenced.</param>
    /// <returns><see cref="Stream"/></returns>
    Stream GetAnnotationResourceStream(string resource);

    /// <summary>
    /// Gets the configured middleware symbols.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<MiddlewareSymbol> GetMiddlewareSymbols();

    /// <summary>
    /// Determines whether a converter for the given type has been registered.
    /// </summary>
    /// <param name="type">The conversion value type.</param>
    /// <returns><c>true</c> if the converter was registered.</returns>
    bool HasArgumentConverter(Type type);

    /// <summary>
    /// Determines whether a converter for the given element and collection type has been registered.
    /// </summary>
    /// <param name="elementType">Collection type argument</param>
    /// <param name="collectionType">Collection type</param>
    /// <returns><c>true</c> if the converter was registered.</returns>
    bool HasCollectionConverter(Type elementType, Type collectionType);
}