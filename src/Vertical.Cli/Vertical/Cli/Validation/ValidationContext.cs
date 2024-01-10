using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

/// <summary>
/// Describes a value being validated and any reported errors.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ValidationContext<T>
{
    private readonly ICollection<string> _errorCollection;
    
    internal ValidationContext(
        SymbolDefinition symbol,
        T value,
        ICollection<string> errorCollection)
    {
        _errorCollection = errorCollection;
        Symbol = symbol;
        Value = value;
    }

    /// <summary>
    /// Gets the option, argument, or switch definition that value is being bound with.
    /// </summary>
    public SymbolDefinition Symbol { get; }

    /// <summary>
    /// Gets the value being validated.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets whether any errors have been reported.
    /// </summary>
    public bool IsValidState => _errorCollection.Count == 0;

    /// <summary>
    /// Adds an error to the context.
    /// </summary>
    /// <param name="message"></param>
    public void AddError(string message)
    {
        Guard.IsNotNullOrWhiteSpace(message);
        _errorCollection.Add(message);
    }

    /// <inheritdoc />
    public override string ToString() => $"\"{Value}\" (valid={IsValidState})";
}