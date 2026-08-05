using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Binding;

internal sealed class CollectionPropertyBinder<TModel, TElement, TCollection> : PropertyBinder
    where TModel : class
    where TCollection : IEnumerable<TElement>
{
    private readonly CliSymbol<TModel, TCollection> _symbol;

    /// <inheritdoc />
    public CollectionPropertyBinder(CliSymbol<TModel, TCollection> symbol) : base(symbol)
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

        if (argumentValues.Any(value => value.Length == 0))
        {
            return new BindingResult<TCollection>(
                _symbol.BindingName,
                default!,
                MissingParameterError.Create(_symbol, bindingInfo.HelpProvider));
        }
        
        var (min, max) = _symbol.Arity;
        var count = argumentValues.Length;

        return (count, _symbol) switch
        {
            { count: 0, _symbol.DefaultProvider: not null } => new BindingResult<TCollection>(_symbol.BindingName,
                _symbol.DefaultProvider()),
            
            { count: 0, _symbol.Arity.Minimum: 0 } => bindingInfo
                .CreateCollectionBindingResult<TModel, TElement, TCollection>(_symbol, []),
            
            { } when count >= min && count <= max => bindingInfo
                .CreateCollectionBindingResult<TModel, TElement, TCollection>(_symbol, argumentValues),
            
            _ => new BindingResult<TCollection>(
                _symbol.BindingName, 
                default!, 
                SymbolArityError.Create(_symbol, argumentValues, bindingInfo.HelpProvider))
        };
    }
}