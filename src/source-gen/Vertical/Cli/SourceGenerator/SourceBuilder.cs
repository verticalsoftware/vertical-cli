using System.Text;
using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public static class SourceBuilder
{
    public static string Build(Compilation compilation, GenerationModel model)
    {
        var code = new StringBuilder(25000);

        WriteHeader(code);
        WriteImports(code);
        WriteNamespaceDeclaration(code);
        WriteNullableAnnotations(code, compilation);
        WriteRootCommandExtensionsClass(code, model);
        
        return code.ToString();
    }

    private static void WriteRootCommandExtensionsClass(StringBuilder code, GenerationModel model)
    {
        var tab = Tab.None;

        code.AppendLine("/// <summary>");
        code.AppendLine("/// Defines methods to invoke and validate a root command.");
        code.AppendLine("/// </summary>");
        code.AppendLine("public static class RootCommandExtensions");
        code.AppendLine("{");
        WriteInvokeMethod(code, model, tab.Indent);
        code.AppendLine();
        WriteCreateHandlerMethod(code, model, tab.Indent);
        code.AppendLine("}");
    }

    private static void WriteInvokeMethod(StringBuilder code, GenerationModel model, Tab tab)
    {
        var indent = tab.Indent;
        
        code.AppendLine("/// <summary>");
        code.AppendLine("/// Invokes the selected command handler function using the path and values");
        code.AppendLine("/// provided by the programs string arguments.");
        code.AppendLine("/// </summary>");
        code.AppendLine("/// <param name=\"rootCommand\">The root command instance.</param>");
        code.AppendLine("/// <param name=\"args\">The collection of program arguments.</param>");
        code.AppendLine("/// <returns>The result provided by the command handler implementation.</returns>");
        code.AppendLine($"{tab}public static {model.AsyncKeyword}{model.ResultTypeName} {model.InvokeMethodName}(");
        code.AppendLine($"{indent}this {model.RootCommandInterfaceName} rootCommand,");
        code.AppendLine($"{indent}global::System.Collections.Generic.IEnumerable<string> args)");
        code.AppendLine($"{tab}{{");
        code.AppendLine($"{indent}if (rootCommand == null)");
        code.AppendLine($"{indent}{{");
        code.AppendLine($"{indent.Indent}throw new global::System.ArgumentNullException(nameof(rootCommand));");
        code.AppendLine($"{indent}}}");
        code.AppendLine();
        code.AppendLine($"{indent}if (args == null)");
        code.AppendLine($"{indent}{{");
        code.AppendLine($"{indent.Indent}throw new global::System.ArgumentNullException(nameof(args));");
        code.AppendLine($"{indent}}}");
        code.AppendLine();
        code.AppendLine($"{indent}var context = global::Vertical.Cli.Binding.BindingContext.Create(rootCommand, args);");
        code.AppendLine();
        code.AppendLine($"{indent}context.ThrowBindingExceptions();");
        code.AppendLine();
        code.AppendLine($"{indent}var callsite = GetCallSite(context);");
        code.AppendLine($"{indent}return {model.AwaitKeyword}callsite();");
        code.AppendLine($"{tab}}}");
    }

    private static void WriteCreateHandlerMethod(StringBuilder code, GenerationModel model, Tab tab)
    {
        var indent = tab.Indent;
        
        code.AppendLine($"{tab}private static global::System.Func<{model.ResultTypeName}> GetCallSite(");
        code.AppendLine($"{indent}{model.BindingContextTypeName} context)");
        code.AppendLine($"{tab}{{");
        code.AppendLine($"{indent}var modelType = context.Subject.ModelType;");
        code.AppendLine();
        
        model.ModelTypes.ForEach((modelType, index) =>
        {
            if (index > 0) code.AppendLine();
            code.AppendLine($"{indent}if (modelType == typeof({modelType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}))");
            code.AppendLine($"{indent}{{");
            WriteCreateModelCase(code, model, modelType, indent.Indent);
            code.AppendLine($"{indent}}}");
        });

        code.AppendLine();
        code.AppendLine($"{indent}throw new global::System.InvalidOperationException();");
        code.AppendLine($"{tab}}}");
    }

    private static void WriteCreateModelCase(
        StringBuilder code,
        GenerationModel model,
        ITypeSymbol modelType,
        Tab tab)
    {
        if (modelType.IsNoneModelType())
        {
            code.Append($"{tab}return context.CreateCallSite<{modelType.ToFullName()}>(");
            code.AppendLine("global::Vertical.Cli.None.Default);");
            return;
        }

        var indent = tab.Indent;
        var bindings = modelType.GetBindings();
        var modelVariable = model.GenerateModelVariableName(modelType);
        code.Append($"{tab}var {modelVariable} = new {modelType.ToFullName()}(");

        bindings
            .Where(b => b.Target == ParameterTarget.ConstructorParameter)
            .ForEach((binding, iter) =>
            {
                code.AppendLine(iter > 0 ? "," : string.Empty);
                code.Append($"{indent}{binding.MetadataName}: {binding.GetContextMethod("context")}");
            });

        code.Append(")");

        var memberBindings = bindings.Where(b => b.Target == ParameterTarget.Member).ToArray();
        if (memberBindings.Length > 0)
        {
            code.AppendLine();
            code.AppendLine($"{tab}{{");
            memberBindings.ForEach((binding, iter) =>
            {
                if (iter > 0) code.AppendLine(",");
                code.Append($"{indent}{binding.MetadataName} = {binding.GetContextMethod("context")}");
            });
            code.AppendLine();
            code.Append($"{tab}}}");
        }

        code.AppendLine(";");
        code.AppendLine();
        code.AppendLine($"{tab}return context.CreateCallSite<{modelType.ToFullName()}>({modelVariable});");
    }

    private static void WriteNullableAnnotations(StringBuilder code, Compilation compilation)
    {
        if (compilation.Options.NullableContextOptions is not (NullableContextOptions.Annotations
            or NullableContextOptions.Enable))
            return;

        code.AppendLine("#nullable enable");
        code.AppendLine();
    }

    private static void WriteNamespaceDeclaration(StringBuilder code)
    {
        code.AppendLine("namespace Vertical.Cli;");
        code.AppendLine();
    }

    private static void WriteImports(StringBuilder code)
    {
        code.AppendLine("using System;");
        code.AppendLine("using System.Collections.Generic;");
        code.AppendLine();
    }

    private static void WriteHeader(StringBuilder code)
    {
        const string mitHeader =
            """
            /*
            Copyright (C) 2023-2024 Vertical Software

            Permission is hereby granted, free of charge, to any person obtaining a copy of this software
            and associated documentation files (the “Software”), to deal in the Software without restriction,
            including without limitation the rights to use, copy, modify, merge, publish, distribute,
            sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
            furnished to do so, subject to the following conditions:

            The above copyright notice and this permission notice shall be included in all copies or substantial
            portions of the Software.

            THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
            NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
            IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
            WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
            SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
            */
            
            """;

        code.AppendLine(mitHeader);
    }
}