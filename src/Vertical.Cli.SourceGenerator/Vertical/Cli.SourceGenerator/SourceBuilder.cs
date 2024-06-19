using System.Text;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public class SourceBuilder(SourceGenerationModel model)
{
    private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    /// <summary>
    /// Generates the code given the model.
    /// </summary>
    /// <returns></returns>
    public string Generate()
    {
        var code = new CodeBlock(new StringBuilder(5000));
        
        code.AppendLine($"namespace {Constants.Namespace};");
        code.AppendLine();

        WriteExtensionClass(ref code);
        
        return code.ToString();
    }

    private void WriteExtensionClass(ref CodeBlock code)
    {
        code.AppendLine(Constants.GeneratedCodeAttribute);
        code.AppendLine($"public static class {Constants.ExtensionClassName}");
        code.AppendBlock(BlockStyle.ClassBody, WriteExtensionClassMethod);
    }

    private void WriteExtensionClassMethod(ref CodeBlock code)
    {
        code.AppendLine("/// <summary>");
        code.AppendLine("/// Parses the arguments, binds to a new instance of the model type, and invokes");
        code.AppendLine("/// the handler defined in the command.");
        code.AppendLine("/// </summary>");
        code.AppendLine("/// <param name=\"command\">The root command definition.</param>");
        code.AppendLine("/// <param name=\"arguments\">The arguments to parse.</param>");
        code.AppendLine("/// <param name=\"cancellationToken\">Token that can be observed for cancellation requests..</param>");
        code.AppendLine("/// <returns>The value returned by the command handler.</returns>");
        code.AppendLine("/// <exception cref=\"Vertical.Cli.CommandLineException\">");
        code.AppendLine("/// The model failed binding, a value couldn't be converted, or one or more unmapped arguments were found.");
        code.AppendLine("/// </exception>");
        code.Append("public static async global::System.Threading.Tasks.Task<int> InvokeAsync");
        code.AppendBlock(BlockStyle.ParameterList, (ref CodeBlock inner) =>
        {
            var csv = new Separator(SeparatorStyle.CsvList);
            var typeArguments = $"{model.RootDefinition.ModelType.Format()}";
            inner.AppendListItem(csv, $"this {Constants.RootCommandClass}<{typeArguments}> command");
            inner.AppendListItem(csv, "string[] arguments");
            inner.AppendListItem(csv, "global::System.Threading.CancellationToken cancellationToken = default");
        });
        code.AppendBlock(BlockStyle.MethodBody, WriteExtensionClassMethodBody);
    }

    private void WriteExtensionClassMethodBody(ref CodeBlock code)
    {
        code.AppendUnIndentedLine("#if DEBUG");
        code.AppendLine("command.VerifyConfiguration();");
        code.AppendUnIndentedLine("#endif");
        code.AppendLine("var context = global::Vertical.Cli.Engine.CliEngine.GetContext(command, arguments);");
        code.AppendLine();
        code.AppendLine("// See if arguments trigger a modeless task.");
        code.AppendLine("if (context.TryGetModelessTask(out var task))");
        code.AppendBlock(BlockStyle.BracedBody, (ref CodeBlock inner) =>
        {
            inner.AppendLine("return await task;");
        });
        code.AppendLine();
        code.AppendLine("var modelType = context.ModelType;");
        code.AppendLine("var converters = command.Options.ValueConverters;");
        code.AppendLine();
        code.AppendLine("// Throws CommandLineException if there are unmapped arguments");
        code.AppendLine("context.AssertArguments();");
        code.AppendLine();

        var modelTypes = new[] { model.RootDefinition.ModelType }
            .Concat(model.SubDefinitions.Select(def => def.ModelType))
            .Distinct(SymbolEqualityComparer.Default)
            .Cast<INamedTypeSymbol>();

        var id = 1;
        foreach (var modelType in modelTypes)
        {
            if (WriteModelBinding(ref code, modelType, id))
                ++id;
        }

        if (id > 1) code.AppendLine();
        
        code.AppendLine("return await context.DefaultCallSite(cancellationToken);");
    }

    private bool WriteModelBinding(ref CodeBlock code, INamedTypeSymbol modelType, int id)
    {
        if (!modelType.TryGetCompatibleConstructor(out var constructor))
            return false;

        var typeFormat = modelType.Format();
        code.AppendLine($"// Bind/invoke {typeFormat} command");
        code.AppendLine($"if (context.TryGetCallSite<{typeFormat}>(out var callSite{id}))");
        code.AppendBlock(BlockStyle.BracedBody, (ref CodeBlock inner) => WriteModelBindingImplementation(
            ref inner, 
            modelType,
            constructor!,
            id));
        return true;
    }

    private void WriteModelBindingImplementation(ref CodeBlock code, 
        INamedTypeSymbol modelType,
        IMethodSymbol constructor,
        int id)
    {
        if (WriteConverterImplementations(ref code, modelType, constructor))
            code.AppendLine();

        WriteModelConstructor(ref code, modelType, constructor, id);
         
        code.AppendLine();
        code.AppendLine("// Invoke client configured validators");
        code.AppendLine($"context.AssertBinding(model{id});");
        code.AppendLine();
        code.AppendLine("// Route control to the command handler with the constructed model");
        code.AppendLine($"return await callSite{id}(model{id}, cancellationToken);");
    }

    private void WriteModelConstructor(
        ref CodeBlock code,
        INamedTypeSymbol modelType,
        IMethodSymbol constructor,
        int id)
    {
        // Records will expose constructor properties as writeable, so we'd end up
        // writing two assignments, track with this hash set
        var nameSet = new HashSet<string>();
        
        code.AppendLine($"// Create an instance of {modelType}");
        code.Append($"var model{id} = new {modelType.Format()}");

        if (constructor.Parameters.Length > 0)
        {
            code.AppendBlock(BlockStyle.ParameterList, (ref CodeBlock inner) =>
                WriteConstructorParameterAssignments(ref inner, constructor, nameSet));
        }
        else code.Append("()");

        var assignableProperties = modelType
            .GetAllProperties()
            .Where(property => nameSet.Add(property.Name))
            .ToArray();
        
        if (assignableProperties.Length > 0)
        {
            code.AppendBlock(BlockStyle.PropertyAssignmentBody, (ref CodeBlock inner) => 
                WritePropertyAssignments(ref inner, assignableProperties));   
        }
        
        code.AppendLine(";");
    }

    private static void WriteConstructorParameterAssignments(ref CodeBlock code, 
        IMethodSymbol constructor,
        ISet<string> nameSet)
    {
        var csv = new Separator(SeparatorStyle.CsvList);
        
        foreach (var parameter in constructor.Parameters)
        {
            nameSet.Add(parameter.Name);
            code.AppendListItem(csv, GetContextValueInvocation(parameter.Type, parameter.Name));    
        }
    }

    private static void WritePropertyAssignments(ref CodeBlock code, IEnumerable<IPropertySymbol> properties)
    {
        var csv = new Separator(SeparatorStyle.CsvList);

        foreach (var property in properties)
        {
            var invocation = GetContextValueInvocation(property.Type, property.Name);
            code.AppendListItem(csv, $"{property.Name} = {invocation}");
        }
    }

    private static string GetContextValueInvocation(ITypeSymbol bindingType, string name)
    {
        if (bindingType is IArrayTypeSymbol array)
        {
            return $"context.GetArray<{array.ElementType.Format()}, {array.Format()}>(\"{name}\")";
        }

        if (bindingType.IsSupportedCollectionType())
        {
            var methodSuffix = bindingType.Name switch
            {
                "IEnumerable" => "Array",
                "ICollection" => "List",
                "IReadOnlyCollection" => "Array",
                "IList" => "List",
                "IReadOnlyList" => "Array",
                "List" => "List",
                "LinkedList" => "LinkedList",
                "ISet" => "HashSet",
                "IReadOnlySet" => "HashSet",
                "HashSet" => "HashSet",
                "SortedSet" => "SortedSet",
                "Stack" => "Stack",
                "Queue" => "Queue",
                "ImmutableArray" => "ImmutableArray",
                "ImmutableList" => "ImmutableList",
                "ImmutableHashSet" => "ImmutableHashSet",
                "ImmutableSortedSet" => "ImmutableSortedSet",
                "ImmutableStack" => "ImmutableStack",
                "ImmutableQueue" => "ImmutableQueue",
                _ => throw new NotSupportedException()
            };

            var collectionType = (INamedTypeSymbol)bindingType;
            var genericArgumentType = collectionType.TypeArguments[0];
            return $"context.Get{methodSuffix}<{genericArgumentType.Format()}, {collectionType.Format()}>(\"{ name }\")";
        }

        return $"context.GetValue<{bindingType.Format()}>(\"{ name }\")";
    }

    private static bool WriteConverterImplementations(
        ref CodeBlock code,
        INamedTypeSymbol modelType,
        IMethodSymbol constructor)
    {
        var convertibleTypes = constructor
            .Parameters
            .Select(parameter => parameter.Type)
            .Concat(modelType.GetAllProperties().Select(property => property.Type))
            .Distinct(SymbolEqualityComparer.Default)
            .Cast<ITypeSymbol>();

        var written = false;

        foreach (var type in convertibleTypes)
        {
            if (!written) code.AppendLine("// Add converters required for this model type");
            WriteConverterImplementation(ref code, type);
            written = true;
        }

        return written;
    }

    private static void WriteConverterImplementation(ref CodeBlock code, ITypeSymbol type)
    {
        var resolvedType = type switch
        {
            IArrayTypeSymbol array => array.ElementType,
            INamedTypeSymbol named when named.TryGetSupportedCollectionArgumentType(out var genericArgumentType) =>
                genericArgumentType!,
            _ => type
        };

        var invocation = resolvedType switch
        {
            { Name: "String", ContainingNamespace.Name: "System" } =>
                "global::Vertical.Cli.Conversion.StringConverter.Default",
            
            { Name: "FileInfo", ContainingNamespace: { Name: "IO", ContainingNamespace.Name: "System" }} => 
                "global::Vertical.Cli.Conversion.FileInfoConverter.Default",
            
            { Name: "DirectoryInfo", ContainingNamespace: { Name: "IO", ContainingNamespace.Name: "System" }} => 
                "global::Vertical.Cli.Conversion.DirectoryInfoConverter.Default",
            
            { Name: "Uri", ContainingNamespace.Name: "System" } =>
                "global::Vertical.Cli.Conversion.UriConverter.Default",
            
            INamedTypeSymbol namedSymbol when namedSymbol.IsNullableValueType() && namedSymbol.TypeArguments[0].ImplementsIParsableInterface() =>
                $"new global::Vertical.Cli.Conversion.NullableParsableConverter<{namedSymbol.TypeArguments[0].Format()}>()",
            
            INamedTypeSymbol namedSymbol when namedSymbol.IsNullableValueType() && namedSymbol.TypeArguments[0].IsEnum() =>
                $"new global::Vertical.Cli.Conversion.NullableEnumConverter<{namedSymbol.TypeArguments[0].Format()}>()",
            
            not null when resolvedType.ImplementsIParsableInterface() => 
                $"new global::Vertical.Cli.Conversion.ParsableConverter<{resolvedType.Format()}>()",
            
            not null when resolvedType.IsEnum() =>
                $"new global::Vertical.Cli.Conversion.EnumConverter<{resolvedType.Format()}>()",
            
            _ => default
        };
        
        if (invocation == null)
            return;
        
        code.AppendLine($"converters.Add({invocation});");
    }
}