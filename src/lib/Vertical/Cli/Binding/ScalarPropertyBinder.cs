using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Binding;

internal sealed class ScalarPropertyBinder<TModel, TValue> : PropertyBinder where TModel : class
{
    private readonly CliSymbol<TModel, TValue> _symbol;

    /// <inheritdoc />
    public ScalarPropertyBinder(CliSymbol<TModel, TValue> symbol) : base(symbol)
    {
        _symbol = symbol;
    }

    /// <inheritdoc />
    public override IBindingResult CreateBindingResult(PropertyBindingInfo bindingInfo)
    {
        var argumentValues = bindingInfo
            .ParseResult
            .GetArgumentValues(_symbol.BindingName)
            .ToArray();
        
        var bindingName = _symbol.BindingName;

        return (argumentValues, _symbol) switch
        {
            { argumentValues: [{Length: > 0}] } => bindingInfo.CreateScalarBindingResult(_symbol, argumentValues[0]),
            
            { argumentValues.Length: 1 } => new BindingResult<TValue>(
                    bindingName, 
                    default!,
                    new MissingParameterError(_symbol)),
            
            { argumentValues.Length: 0, _symbol.DefaultProvider: not null } =>
                new BindingResult<TValue>(bindingName, _symbol.DefaultProvider()),
            
            { argumentValues.Length: 0, _symbol.Arity.Minimum: 0 } => new BindingResult<TValue>(bindingName, default!),
            
            _ => new BindingResult<TValue>(bindingName, default!, new SymbolArityError(_symbol, []))
        };
    }
}