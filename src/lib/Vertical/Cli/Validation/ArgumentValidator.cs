using System.Linq.Expressions;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

internal abstract class ArgumentValidator<TModel>(CliParameter parameter, string message) where TModel : class
{
    public CliParameter Parameter { get; } = parameter;
    
    public string Message { get; } = message;

    public abstract object? GetValue(TModel model);

    public abstract bool Validate(TModel model);
}

internal sealed class ArgumentValidator<TModel, TValue>(
    CliParameter parameter,
    string message,
    Expression<Func<TModel, TValue>> binding,
    Func<TModel, TValue, bool> evaluator)
    : ArgumentValidator<TModel>(parameter, message) where TModel : class
{
    private readonly Func<TModel, TValue> accessor = binding.Compile();


    /// <inheritdoc />
    public override object? GetValue(TModel model) => accessor(model);

    /// <inheritdoc />
    public override bool Validate(TModel model) => evaluator(model, accessor(model));
}