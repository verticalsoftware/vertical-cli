using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public sealed class GenerationInfo(TypeModel[] TypeModels, ITypeSymbol[] conversionTypes)
{
    public TypeModel[] TypeModels { get; } = TypeModels;
    
    public ITypeSymbol[] ConversionTypes { get; } = conversionTypes;
}