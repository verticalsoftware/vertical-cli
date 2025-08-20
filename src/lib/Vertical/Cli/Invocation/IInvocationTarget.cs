using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents a command that implements application logic.
/// </summary>
public interface IInvocationTarget
{
    /// <summary>
    /// Creates a handler request.
    /// </summary>
    /// <param name="configuration">The root configuration.</param>
    /// <param name="modelConfigurationFactory">The service that creates model configuration instances.</param>
    /// <returns><see cref="HandlerRequest"/></returns>
    HandlerContextBuilder CreateRequestBuilder(
        IRootConfiguration configuration, 
        IModelConfigurationFactory modelConfigurationFactory);
}