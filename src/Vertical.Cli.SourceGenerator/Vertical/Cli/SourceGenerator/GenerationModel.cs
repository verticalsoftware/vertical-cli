using Microsoft.CodeAnalysis;

namespace Vertical.Cli.SourceGenerator;

public class GenerationModel
{
    private readonly CommandModel[] _models;
    private readonly HashSet<string> _generatedModelVariables = new();

    public GenerationModel(CommandModel[] models)
    {
        _models = models;
        RootCommandModel = models.First(model => model.IsRootCommand);
        ResultType = RootCommandModel.ResultType;
        ResultTypeName = ResultType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        IsAsyncFlow = ResultType.IsTaskType();
        RootCommandInterfaceName = $"global::Vertical.Cli.IRootCommand<{RootCommandModel.ModelTypeName}, {RootCommandModel.ResultTypeName}>";
        BindingContextTypeName = $"global::Vertical.Cli.Binding.IBindingContext<{ResultTypeName}>";
        CallSiteContextTypeName = $"global::Vertical.Cli.Invocation.ICallSiteContext<{ResultTypeName}>";
    }

    public string BindingContextTypeName { get; set; }
    public string CallSiteContextTypeName { get; set; }

    public string RootCommandInterfaceName { get; set; }

    public bool IsAsyncFlow { get; }

    public string ResultTypeName { get; }

    public CommandModel RootCommandModel { get; }
    
    public ITypeSymbol ResultType { get; }

    public string ReturnsKeyword => ResultType.ToDisplayString() == "System.Threading.Tasks.Task"
        ? string.Empty
        : "return ";

    public string AsyncKeyword => IsAsyncFlow ? "async " : string.Empty;

    public string AwaitKeyword => IsAsyncFlow ? "await " : string.Empty;
    
    public string InvokeMethodName => IsAsyncFlow ? "InvokeAsync" : "Invoke";

    public string DefaultValue
    {
        get
        {
            if (!IsAsyncFlow)
            {
                return $"global::Vertical.Cli.Binding.DefaultOf<{ResultTypeName}>.Value";
            }

            var valueType = ((INamedTypeSymbol)ResultType).TypeArguments.FirstOrDefault();
            return valueType != null
                ? $"global::Vertical.Cli.Binding.DefaultOf<{valueType.ToFullName()}>.TaskValue"
                : "global::System.Threading.Tasks.Task.CompletedTask";
        }
    }

    public IEnumerable<ITypeSymbol> ModelTypes => new HashSet<ITypeSymbol>(
        _models.Select(model => model.ModelType),
        SymbolEqualityComparer.Default);

    public string GenerateModelVariableName(ITypeSymbol typeSymbol)
    {
        var baseVariableName = typeSymbol.CreateVariableName();
        var candidateName = baseVariableName;
        var nextId = 2;

        while (!_generatedModelVariables.Add(candidateName))
        {
            candidateName = $"{baseVariableName}{nextId++}";
        }

        return candidateName;
    }
}