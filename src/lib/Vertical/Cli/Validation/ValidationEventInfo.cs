using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Validation;

public class ValidationEventInfo<TModel, TValue> where TModel : class
{
    protected ValidationContext Context { get; }

    internal ValidationEventInfo(
        ValidationContext context, 
        CliSymbol symbol,
        TModel model,
        TValue value)
    
    {
        Context = context;
        Symbol = symbol;
        Model = model;
        Value = value;
    }

    public CliSymbol Symbol { get; }

    public TModel Model { get; }

    public TValue Value { get; }

    public ValidationEventInfo<TModel, TValue> OK => this;

    public ValidationEventInfo<TModel, TValue> Error(string message)
    {
        Context.AddError(new SymbolValidationError(Symbol, Model, Value, message));
        return this;
    }
}

public sealed class ValidationEventInfo<TModel, TElement, TCollection> : ValidationEventInfo<TModel, TCollection>
    where TModel : class
    where TCollection : IEnumerable<TElement>
{
    /// <inheritdoc />
    internal ValidationEventInfo(
        ValidationContext context,
        CliSymbol<TModel, TCollection> symbol,
        TModel model,
        TCollection value)
        : base(context, symbol, model, value)
    {
    }

    public ValidationEventInfo<TModel, TElement, TCollection> ForEachValue(
        Action<ValidationEventInfo<TModel, TElement>> validate)
    {
        foreach (var value in Value)
        {
            var elementInfo = new ValidationEventInfo<TModel, TElement>(Context, Symbol, Model, value);
            validate(elementInfo);
        }

        return this;
    }
}