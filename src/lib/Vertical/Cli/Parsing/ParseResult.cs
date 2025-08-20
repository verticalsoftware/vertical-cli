using Vertical.Cli.Binding;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the result of token parsing.
/// </summary>
public sealed partial class ParseResult
{
    private ParseResult(
        Dictionary<string, string[]> valueDictionary,
        Token[] pendingTokens,
        ISymbolBinding[] bindings,
        List<UsageError> errors)
    {
        _valueDictionary = valueDictionary;

        PendingTokens = pendingTokens;
        SymbolBindings = bindings;
        Errors = errors;
    }


    private readonly Dictionary<string, string[]> _valueDictionary;

    /// <summary>
    /// Gets the usage error list.
    /// </summary>
    public List<UsageError> Errors { get; }

    /// <summary>
    /// Gets tokens that were not matched to symbols.
    /// </summary>
    public Token[] PendingTokens { get; }

    /// <summary>
    /// Gets the symbols bindings available during the parsing operation.
    /// </summary>
    public ISymbolBinding[] SymbolBindings { get; }

    /// <summary>
    /// Gets whether the specified binding name is mapped to an entry in the value collection.
    /// </summary>
    /// <param name="bindingName"></param>
    /// <returns>
    /// <c>true</c> if <paramref name="bindingName"/> is mapped to an entry in the value collection.
    /// </returns>
    public bool ContainsBinding(string bindingName) => _valueDictionary.ContainsKey(bindingName);

    /// <summary>
    /// Gets the count of values parsed for the given symbol binding name.
    /// </summary>
    /// <param name="bindingName">The symbol's binding property name.</param>
    /// <returns>The count of values.</returns>
    public int GetValueCount(string bindingName)
    {
        return GetValues(bindingName).Length;
    }

    /// <summary>
    /// Gets the values for a symbol binding.
    /// </summary>
    /// <param name="bindingName">The symbol's binding property name.</param>
    /// <returns>String value array</returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="bindingName"/> does not reference a valid symbol binding name.
    /// </exception>
    public string[] GetValues(string bindingName)
    {
        return TryGetValues(bindingName, out var values)
            ? values
            : throw Exceptions.InvalidBindingName(bindingName);
    }

    /// <summary>
    /// Tries to get the binding values.
    /// </summary>
    /// <param name="bindingName">The name of the model property.</param>
    /// <param name="values">Upon return, the retrieved string array.</param>
    /// <returns><c>true</c> if <paramref name="bindingName"/> is mapped to an entry in the value
    /// collection.</returns>
    public bool TryGetValues(string bindingName, out string[] values)
    {
        if (_valueDictionary.TryGetValue(bindingName, out var dictionaryValues))
        {
            values = dictionaryValues;
            return true;
        }

        values = [];
        return false;
    }
}