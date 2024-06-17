using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public sealed class CommandDefinition
{
    public CommandDefinition(INamedTypeSymbol modelType, INamedTypeSymbol resultType, bool isRootDefinition)
    {
        ModelType = modelType;
        FormattedModelType = modelType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        ResultType = resultType;
        FormattedResultType = resultType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        IsRootDefinition = isRootDefinition;
        ReturnsValue = FormattedResultType != "global::System.Threading.Tasks.Task" &&
                       FormattedResultType != "global::System.Void";
    }

    public bool ReturnsValue { get; set; }

    public string FormattedResultType { get; set; }

    public string FormattedModelType { get; set; }

    public INamedTypeSymbol ModelType { get; }

    public INamedTypeSymbol ResultType { get; }
    
    public bool IsRootDefinition { get; }
}