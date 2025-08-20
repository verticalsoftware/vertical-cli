namespace Vertical.Cli.Validation;

public static partial class ValidationArgsExtensions
{
    /// <summary>
    /// Chains the given configuration delegate.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="configureValidation">THe action that performs the contextual evaluation.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> ThenEvaluate<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        Action configureValidation) where TModel : class
    {
        configureValidation();
        return context;
    }
}