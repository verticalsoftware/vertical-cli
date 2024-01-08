namespace Vertical.Cli.Parsing;

public sealed class SemanticArgument
{
    private readonly Action<int> _removeIndexCallback;

    internal SemanticArgument(
        Action<int> removeIndexCallback,
        SymbolSyntax argumentSyntax,
        int ordinalPosition,
        SymbolSyntax? candidateOperandSyntax = null,
        int? operandPosition = null)
    {
        _removeIndexCallback = removeIndexCallback;
        ArgumentSyntax = argumentSyntax;
        OrdinalPosition = ordinalPosition;
        CandidateOperandSyntax = candidateOperandSyntax;
        OperandPosition = operandPosition;
    }

    public SymbolSyntax ArgumentSyntax { get; }

    public int OrdinalPosition { get; }

    public SymbolSyntax? CandidateOperandSyntax { get; }

    public int? OperandPosition { get; }

    public void Accept() => _removeIndexCallback(OrdinalPosition);

    public void AcceptOperand() => _removeIndexCallback(OrdinalPosition + 1);

    /// <inheritdoc />
    public override string ToString() => ArgumentSyntax.Text;
}