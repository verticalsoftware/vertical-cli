using Vertical.Cli.Invocation;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Represents data about a middleware symbol event.
/// </summary>
/// <typeparam name="TValue">Parameter value type.</typeparam>
public sealed class ParameterizedMiddlewareDirectiveInfo<TValue>
{
    internal ParameterizedMiddlewareDirectiveInfo(
        InvocationContext context,
        TValue value)
    {
        Context = context;
        Value = value;
    }

    /// <summary>
    /// Gets the invocation context.
    /// </summary>
    public InvocationContext Context { get; }

    /// <summary>
    /// Gets the parameter value.
    /// </summary>
    public TValue Value { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Value}";
}