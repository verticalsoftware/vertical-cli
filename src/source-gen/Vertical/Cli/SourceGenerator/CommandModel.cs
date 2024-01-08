using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public class CommandModel
{
    public CommandModel(
        bool isRootCommand,
        ITypeSymbol modelType,
        ITypeSymbol resultType)
    {
        IsRootCommand = isRootCommand;
        ModelType = modelType;
        ResultType = resultType;
        ModelTypeName = modelType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        ResultTypeName = resultType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public string ResultTypeName { get; set; }

    public string ModelTypeName { get; set; }

    public bool IsRootCommand { get; }
    
    public ITypeSymbol ModelType { get; }
    
    public ITypeSymbol ResultType { get; } 
}