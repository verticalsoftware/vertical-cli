using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a binding result.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class BindingResult<TValue> : IBindingResult
{
    internal BindingResult(
        string bindingName,
        TValue value,
        CommandLineError? error = null)
    {
        BindingName = bindingName;
        Value = value;
        Error = error;
    }

    /// <inheritdoc />
    public string BindingName { get; }

    /// <summary>
    /// Gets the binding value.
    /// </summary>
    public TValue Value { get; }

    /// <inheritdoc />
    public Type ValueType => typeof(TValue);
    
    /// <inheritdoc />
    public CommandLineError? Error { get; }
}