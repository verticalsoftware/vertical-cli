using Vertical.Cli.Configuration;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents an argument that has semantic meaning.
/// </summary>
public sealed class SemanticArgument
{
    private readonly Action<int> _removeIndexCallback;
    private readonly int _operandPosition;

    internal SemanticArgument(
        Action<int> removeIndexCallback,
        SymbolSyntax argumentSyntax,
        int ordinalPosition,
        SymbolDefinition? optionSymbol = null,
        SymbolSyntax? operandSourceSyntax = null,
        int operandPosition = -1,
        bool terminated = false)
    {
        _removeIndexCallback = removeIndexCallback;
        _operandPosition = operandPosition;
        ArgumentSyntax = argumentSyntax;
        OrdinalPosition = ordinalPosition;
        OptionSymbol = optionSymbol;
        OperandSourceSyntax = operandSourceSyntax;
        Terminated = terminated;
        OperandValue = (argumentSyntax.HasOperand
            ? argumentSyntax.OperandValue
            : operandSourceSyntax?.Text) ?? string.Empty;
    }

    /// <summary>
    /// Gets the expanded argument syntax.
    /// </summary>
    public SymbolSyntax ArgumentSyntax { get; }

    /// <summary>
    /// Gets the argument position.
    /// </summary>
    public int OrdinalPosition { get; }
    
    /// <summary>
    /// Gets the option symbol the argument was matched to.
    /// </summary>
    public SymbolDefinition? OptionSymbol { get; }

    /// <summary>
    /// Gets the syntax for the symbol that may be the option's candidate operand.
    /// </summary>
    public SymbolSyntax? OperandSourceSyntax { get; }

    /// <summary>
    /// Gets whether the argument occurred after a terminator symbol.
    /// </summary>
    public bool Terminated { get; }

    /// <summary>
    /// Gets the resolved operand value.
    /// </summary>
    public string OperandValue { get; }

    /// <summary>
    /// Gets whether the argument has an operand value.
    /// </summary>
    public bool HasOperand => !string.IsNullOrWhiteSpace(OperandValue);
    
    /// <summary>
    /// Signals the argument was consumed by a binding.
    /// </summary>
    public void Accept() => _removeIndexCallback(OrdinalPosition);

    /// <summary>
    /// Signals the argument was consumed by a binding.
    /// </summary>
    public void AcceptOperand()
    {
        if (_operandPosition == -1)
            return;
        _removeIndexCallback(_operandPosition);
    }

    /// <inheritdoc />
    public override string ToString() => ArgumentSyntax.Text;
}