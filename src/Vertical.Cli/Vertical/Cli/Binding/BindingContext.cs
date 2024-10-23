using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Internal;
using Vertical.Cli.Parsing;
using ValidationContext = Vertical.Cli.Validation.ValidationContext;

namespace Vertical.Cli.Binding;

/// <summary>
/// Describes the context for client code to bind a model and invoke the command
/// handler.
/// </summary>
[NoGeneratorBinding]
public sealed partial class BindingContext
{
    private readonly IReadOnlyDictionary<string, CliSymbol> _symbols;
    private readonly ILookup<string, string> _valueLookup;
    private readonly Task<int>? _shortTask;

    internal BindingContext(
        CliCommand commandTarget,
        string path,
        IEnumerable<CliSymbol> symbols,
        ILookup<string, string> valueLookup,
        CliOptions options,
        Task<int>? shortTask = null)
    {
        Path = path;
        CommandTarget = commandTarget;
        Options = options;
        _valueLookup = valueLookup;
        _shortTask = shortTask;
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
    /// Gets the cli options.
    /// </summary>
    public CliOptions Options { get; }

    /// <summary>
    /// Gets the call site.
    /// </summary>
    /// <typeparam name="TModel">Command model type</typeparam>
    /// <returns>Function that implement's the command logic.</returns>
    public Func<TModel, CancellationToken, Task<int>> GetCallSite<TModel>() where TModel : class
    {
        return ((CliCommand<TModel>)CommandTarget).Handler;
    }

    /// <summary>
    /// Tries to get the action call site.
    /// </summary>
    /// <param name="callSite">If the action call site is registered, a reference to the task.</param>
    /// <returns><c>true</c> if the callsite was assigned.</returns>
    public bool TryGetModelessTask([NotNullWhen(true)] out Task<int>? callSite)
    {
        return (callSite = _shortTask) != null;
    }

    /// <summary>
    /// Attempts to obtain a call site.
    /// </summary>
    /// <param name="callSite">If successful, the function used to invoke the command handler.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns><c>bool</c> indicating if the call site was matched</returns>
    public bool TryGetCallSite<TModel>([NotNullWhen(true)] out Func<TModel, CancellationToken, Task<int>>? callSite)
        where TModel : class
    {
        callSite = typeof(TModel) == ModelType
            ? ((CliCommand<TModel>)CommandTarget).Handler
            : null;

        return callSite != null;
    }

    /// <summary>
    /// Gets the default call site.
    /// </summary>
    public readonly Func<CancellationToken, Task<int>> DefaultCallSite = _ => throw new NotImplementedException("Command not implemented");
    
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
        var validationContext = ValidateModel(model);
        
        if (validationContext.IsValid)
            return;

        throw Exceptions.ValidationFailed(Path, validationContext.Errors);
    }

    /// <summary>
    /// Asserts unmapped arguments.
    /// </summary>
    /// <exception cref="Exception">There are one or more unmapped arguments.</exception>
    public void AssertArguments()
    {
        var unmappedArguments = UnmappedArguments().ToArray();
        if (unmappedArguments.Length != 0)
        {
            throw Exceptions.UnmappedArgument(CommandTarget, Path,unmappedArguments.First());
        }
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
        for (var command = CommandTarget; command != null; command = command.Parent)
        {
            command.ValidateModel(model, context);
        }
        return context;
    }

    /// <summary>
    /// Gets the arguments that were not matched to command symbols.
    /// </summary>
    /// <returns>Collection of arguments</returns>
    public IEnumerable<string> UnmappedArguments() => _valueLookup[ArgumentValueLookup.UnmappedArgumentKey];

    private ValueConverter<TValue> GetConverter<TValue>()
    {
        var targetType = typeof(TValue);
        var clientConverter = Options
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