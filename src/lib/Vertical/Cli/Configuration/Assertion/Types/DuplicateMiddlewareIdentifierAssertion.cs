using Vertical.Cli.Middleware;

namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a middleware symbol was defined for two or more symbols.
/// </summary>
public sealed class DuplicateMiddlewareIdentifierAssertion : ConfigurationAssertion
{
    internal DuplicateMiddlewareIdentifierAssertion(string identifier, MiddlewareSymbol[] symbols)
    {
        Identifier = identifier;
        Symbols = symbols;
    }

    /// <summary>
    /// Gets the shared identifier.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the symbols.
    /// </summary>
    public MiddlewareSymbol[] Symbols { get; }

    /// <inheritdoc />
    public override string GroupingKey => "Middleware configuration:";

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Middleware identifier '{Identifier}' used across multiple symbols:";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return Symbols.Select(AssertionDescriptor.Create);
    }
}