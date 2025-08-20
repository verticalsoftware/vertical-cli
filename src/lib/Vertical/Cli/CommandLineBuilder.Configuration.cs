using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli;

public sealed partial class CommandLineBuilder : IRootConfiguration
{
    /// <inheritdoc />
    IParser IRootConfiguration.CreateParser() => new Parser(Options);

    /// <inheritdoc />
    Func<InvocationContext, Task> IRootConfiguration.CreateMiddlewarePipeline(
        IEnumerable<Middleware> terminalDelegates)
    {
        return _middlewareConfiguration.BuildPipeline(terminalDelegates);
    }

    /// <inheritdoc />
    bool IRootConfiguration.TryGetValueConverter<TValue>([NotNullWhen(true)] out ValueConverter<TValue>? valueConverter)
    {
        valueConverter = _lazyValueConverters.IsValueCreated
                         && _lazyValueConverters.Value.TryGetValue(typeof(TValue), out var function)
            ? Unsafe.As<ValueConverter<TValue>>(function)
            : null;

        return valueConverter != null;
    }

    /// <inheritdoc />
    IContextBuilder IRootConfiguration.ConfigureSymbolBuilder(IContextBuilder builder)
    {
        _middlewareConfiguration.ConfigureContext(builder);
        return builder;
    }

    /// <inheritdoc />
    HandlerContextBuilder IRootConfiguration.ConfigureRequestBuilder(
        HandlerContextBuilder builder, 
        Type modelType,
        IModelConfigurationFactory configurationFactory)
    {
        _modelConfigurationBuilder.ConfigureRequestBuilder(builder, modelType, configurationFactory);
        _middlewareConfiguration.ConfigureContext(builder);
        return builder;
    }
}