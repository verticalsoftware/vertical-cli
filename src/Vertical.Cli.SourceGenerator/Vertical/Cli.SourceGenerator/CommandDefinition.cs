using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public sealed class CommandDefinition
{
    public CommandDefinition(INamedTypeSymbol modelType, bool isRootDefinition)
    {
        ModelType = modelType;
        FormattedModelType = modelType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        IsRootDefinition = isRootDefinition;
    }

    public string FormattedModelType { get; set; }

    public INamedTypeSymbol ModelType { get; }
    
    public bool IsRootDefinition { get; }
}