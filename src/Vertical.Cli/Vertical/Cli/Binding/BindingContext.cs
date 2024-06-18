using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Internal;
using ValidationContext = Vertical.Cli.Validation.ValidationContext;

namespace Vertical.Cli.Binding;

/// <summary>
/// Describes the context for client code to bind a model and invoke the command
/// handler.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public sealed partial class BindingContext<TResult>
{
    private static readonly (Type ModelType, Func<CliCommand, CancellationToken, TResult> WrappedCallSite)[]
        StaticCallSites = 
            [
                (typeof(Empty), (command, token) => InvokeCallSite(command, Empty.Default, token)),
                (typeof(CliCommand), (command, token) => InvokeCallSite(command, command, token))
            ];
    
    private readonly IReadOnlyDictionary<string, CliSymbol> _symbols;
    private readonly ILookup<string, string> _valueLookup;
    private readonly CliOptions _options;

    internal BindingContext(
        CliCommand<TResult> commandTarget,
        string path,
        IEnumerable<CliSymbol> symbols,
        ILookup<string, string> valueLookup,
        CliOptions options)
    {
        Path = path;
        CommandTarget = commandTarget;
        _valueLookup = valueLookup;
        _options = options;
        _symbols = symbols.ToDictionary(symbol => symbol.BindingName);
    }

    /// <summary>
    /// Gets the command path.
    /// </summary>
    public string Path { get; }
    
    /// <summary>
    /// Gets the command.
    /// </summary>
    public CliCommand CommandTarget { get; }

    /// <summary>
    /// Gets the model type of the target command.
    /// </summary>
    public Type ModelType => CommandTarget.ModelType;

    /// <summary>
    /// Gets the call site.
    /// </summary>
    /// <typeparam name="TModel">Command model type</typeparam>
    /// <returns>Function that implement's the command logic.</returns>
    public Func<TModel, CancellationToken, TResult> GetCallSite<TModel>() where TModel : class
    {
        return ((CliCommand<TModel, TResult>)CommandTarget).Handler;
    }

    /// <summary>
    /// Attempts to obtain a call site.
    /// </summary>
    /// <param name="callSite">If successful, the function used to invoke the command handler.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns><c>bool</c> indicating if the call site was matched</returns>
    public bool TryGetCallSite<TModel>([NotNullWhen(true)] out Func<TModel, CancellationToken, TResult>? callSite)
        where TModel : class
    {
        callSite = typeof(TModel) == ModelType
            ? ((CliCommand<TModel, TResult>)CommandTarget).Handler
            : null;

        return callSite != null;
    }

    /// <summary>
    /// Attempts to get a static call site (one where model is internally provided).
    /// </summary>
    /// <param name="callSite">If successful, the function used to invoke the command handler.</param>
    /// <returns><c>bool</c> indicating if the call site was matched</returns>
    public bool TryGetStaticCallSite([NotNullWhen(true)] out Func<CancellationToken, TResult>? callSite)
    {
        var wrappedCallSite = StaticCallSites
            .FirstOrDefault(cs => cs.ModelType == ModelType)
            .WrappedCallSite;

        return (callSite = wrappedCallSite != null
            ? token => wrappedCallSite(CommandTarget, token)
            : null) != null;
    }

    private static TResult InvokeCallSite<TModel>(CliCommand command, 
        TModel model, 
        CancellationToken cancellationToken) 
        where TModel : class
    {
        return ((CliCommand<TModel, TResult>)command).Handler(model, cancellationToken);
    }
    
    /// <summary>
    /// Gets a value for the specified binding.
    /// </summary>
    /// <param name="bindingName">Binding name</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>The binding value</returns>
    public TValue GetValue<TValue>(string bindingName)
    {
        return GetValues<TValue, TValue>(bindingName).SingleOrDefault()!;
    }
    
    /// <summary>
    /// Gets a range of values for the specific collection binding.
    /// </summary>
    /// <param name="bindingName">Binding name</param>
    /// <typeparam name="TValue">Collection value type</typeparam>
    /// <typeparam name="TBinding">Symbol binding type</typeparam>
    /// <returns>A collection of values</returns>
    public IEnumerable<TValue> GetValues<TValue, TBinding>(string bindingName)
    {
        var symbol = GetSymbol<TBinding>(bindingName);
        var converter = GetConverter<TValue>();
        var valueList = _valueLookup[bindingName]
            .Where(value => value.Length > 0)
            .Select(value => ResolveValue(symbol, value, converter))
            .ToList();

        if (valueList.Count == 0 && symbol.DefaultProvider != null)
        {
            var defaultValue = (object)symbol.DefaultProvider()!;

            switch (defaultValue)
            {
                case IEnumerable<TValue> enumerable:
                    valueList.AddRange(enumerable);
                    break;
                
                case TValue value:
                    valueList.Add(value);
                    break;
                
                default:
                    // Shouldn't happen because type safety is enforced in symbol
                    throw new InvalidOperationException();
            }
        }
        
        // Validate arity
        if (valueList.Count < symbol.Arity.MinCount)
            throw Exceptions.MinimumArityNotMet(Path, symbol, valueList.Count);
        
        if (symbol.Arity.MaxCount.HasValue && valueList.Count > symbol.Arity.MaxCount)
            throw Exceptions.MaximumArityExceeded(Path, symbol, valueList.Count);

        return valueList;
    }

    /// <summary>
    /// Checks the binding state.
    /// </summary>
    /// <param name="model">Model to validate.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <exception cref="Exception">
    /// Thrown when there are validation errors or unused arguments.
    /// </exception>
    public void AssertBinding<TModel>(TModel model) where TModel : class
    {
        var unmappedArguments = GetUnmappedArguments().ToArray();
        if (unmappedArguments.Length != 0)
        {
            throw Exceptions.UnmappedArgument(Path,unmappedArguments.First());
        }

        var validationContext = ValidateModel(model);
        if (validationContext.IsValid)
            return;

        throw Exceptions.ValidationFailed(Path, validationContext.Errors);
    }

    /// <summary>
    /// Validates a bound model.
    /// </summary>
    /// <param name="model">Target model</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns><see cref="ValidationContext"/> that describes the results.</returns>
    public ValidationContext ValidateModel<TModel>(TModel model) where TModel : class
    {
        var context = new ValidationContext();
        for (CliCommand? command = CommandTarget; command != null; command = command.ParentCommand)
        {
            command.ValidateModel(model, context);
        }
        return context;
    }
    
    /// <summary>
    /// Gets the arguments that were not matched to command symbols.
    /// </summary>
    /// <returns>Collection of arguments</returns>
    public IEnumerable<string> GetUnmappedArguments()
    {
        return _valueLookup["#unmapped"];
    }
    
    private ValueConverter<TValue> GetConverter<TValue>()
    {
        var targetType = typeof(TValue);
        var clientConverter = _options
            .ValueConverters
            .FirstOrDefault(converter => converter.TargetType == targetType);

        return clientConverter as ValueConverter<TValue> ?? throw Exceptions.NoDefaultConverter<TValue>();
    }
    
    private TValue ResolveValue<TValue>(CliSymbol symbol, string value, ValueConverter<TValue> converter)
    {
        try
        {
            return converter.Convert(value);
        }
        catch (Exception exception)
        {
            throw Exceptions.ConversionFailed<TValue>(Path, symbol, value, exception);
        }
    }

    private CliSymbol<TValue> GetSymbol<TValue>(string bindingName)
    {
        if (_symbols.TryGetValue(bindingName, out var obj))
            return (CliSymbol<TValue>)obj;

        throw new InvalidOperationException($"No symbol has been mapped for binding \"{CommandTarget.ModelType}.{bindingName}\".");
    }
}