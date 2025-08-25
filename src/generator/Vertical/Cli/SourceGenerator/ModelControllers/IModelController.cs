using Microsoft.CodeAnalysis;
using Vertical.Cli.SourceGenerator.Utilities;

namespace Vertical.Cli.SourceGenerator.ModelControllers;

public interface IModelController
{
    ITypeSymbol TypeSymbol { get; }
    
    string ImplementationTypeName { get; }

    void WriteImplementation(CodeFormatter formatter);
}