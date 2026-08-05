namespace Vertical.Cli.Configuration.Assertion.Types;

public sealed class InvalidAliasAssertion : ConfigurationAssertion
{
    internal InvalidAliasAssertion(Type modelType, CliSymbol symbol, string[] aliases)
    {
        ModelType = modelType;
        Symbol = symbol;
        Aliases = aliases;
    }

    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Gets the symbol with the invalid 
    /// </summary>
    public CliSymbol Symbol { get; }

    /// <summary>
    /// Gets the alias(es) that are invalid.
    /// </summary>
    public string[] Aliases { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(ModelType);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"{Symbol.BindingName}: {string.Join(", ", Aliases)}";
    }
}