using Microsoft.CodeAnalysis;

namespace Vertical.Cli.Analysis;

public interface IConversionMethodCall
{
}

public sealed class ArgumentConversionCall(ITypeSymbol targetType, string converterExpression) : IConversionMethodCall
{
    public ITypeSymbol TargetType { get; } = targetType;
    
    public string ConverterExpression { get; } = converterExpression;
}

public sealed class CollectionConversionCall(
    ArgumentConversionCall? argumentConversionCall,
    ITypeSymbol elementType,
    ITypeSymbol collectionType,
    string collectionCreationExpression) : IConversionMethodCall
{
    public ArgumentConversionCall? ArgumentConversionCall { get; } = argumentConversionCall;
    
    public ITypeSymbol ElementType { get; } = elementType;
    
    public ITypeSymbol CollectionType { get; } = collectionType;
    
    public string CollectionCreationExpression { get; } = collectionCreationExpression;
}