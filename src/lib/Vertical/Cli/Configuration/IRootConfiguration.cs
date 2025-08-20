using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the main configuration object for the application.
/// </summary>
public interface IRootConfiguration
{
    /// <summary>
    /// Gets the root command.
    /// </summary>
    IRootCommand RootCommand { get; }
    
    /// <summary>
    /// Creates a parser instance.
    /// </summary>
    /// <returns><see cref="IParser"/></returns>
    IParser CreateParser();

    /// <summary>
    /// Creates the middleware pipeline.
    /// </summary>
    /// <param name="terminalDelegates">Terminal delegates to add to the end of the pipeline.</param>
    Func<InvocationContext, Task> CreateMiddlewarePipeline(IEnumerable<Middleware> terminalDelegates);

    /// <summary>
    /// Gets a value converter instance.
    /// </summary>
    /// <param name="valueConverter">
    /// If registered by the client, the <see cref="ValueConverter{TValue}"/> instance.
    /// </param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns><c>true</c> if <paramref name="valueConverter"/> was assigned.</returns>
    bool TryGetValueConverter<TValue>([NotNullWhen(true)] out ValueConverter<TValue>? valueConverter);

    /// <summary>
    /// Configures the provided symbol builder.
    /// </summary>
    /// <param name="builder">The symbol builder instance</param>
    IContextBuilder ConfigureSymbolBuilder(IContextBuilder builder);

    /// <summary>
    /// Configures a request builder.
    /// </summary>
    /// <param name="builder">Builder instance to configure</param>
    /// <param name="modelType">Model type</param>
    /// <param name="modelConfigurationFactory"></param>
    /// <returns><paramref name="builder"/></returns>
    HandlerContextBuilder ConfigureRequestBuilder(
        HandlerContextBuilder builder, 
        Type modelType,
        IModelConfigurationFactory modelConfigurationFactory);
}