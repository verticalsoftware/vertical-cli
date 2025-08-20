using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public static class CodeStringBuilderExtensions
{
    public static void WriteModelImplementationPropertyDeclaration(
        this CodeFormattedStringBuilder builder,
        IPropertySymbol property)
    {
        builder.Write(property.DeclaredAccessibility.ToString().ToLower()).Write(' ');

        if (property is { IsAbstract: true, ContainingType.TypeKind: not TypeKind.Interface })
            builder.Write("override ");

        if (property.IsRequired || property.ContainingType.TypeKind is TypeKind.Interface)
        {
            builder.Write("required ");
        }

        builder.Write($"{property.Type} {property.Name} {{ get; ");

        var setAccessor = property switch
        {
            { ContainingType.TypeKind: TypeKind.Interface } => "init; ",
            { SetMethod.IsInitOnly: true } => "init; ",
            { IsReadOnly: false } => "set; ",
            _ => string.Empty
        };
        
        builder.Write($"{setAccessor}}}");

        switch (property)
        {
            case { IsRequired: true }:
            case { Type.IsValueType: true }:
            case { Type.NullableAnnotation: NullableAnnotation.Annotated }:
                builder.WriteLine();
                break;
            
            default:
                builder.WriteLine(" = default!;");
                break;
        }
    }
    
    public static CodeFormattedStringBuilder WriteGeneratedCodeAttribute(this CodeFormattedStringBuilder builder)
    {
        return builder
            .WriteLine("[System.CodeDom.Compiler.GeneratedCode(\"Vertical.Cli.SourceGenerator.CommandLineBuilderExtensionsGenerator\", \"1.3.0\")]");
    }
}