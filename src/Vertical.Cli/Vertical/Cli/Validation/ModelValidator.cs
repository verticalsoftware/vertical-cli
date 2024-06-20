using System.Linq.Expressions;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation
{
    /// <summary>
    /// Represents an object that validates models after arguments are parsed but before being
    /// delegated to the handler.
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    public sealed class ModelValidator<TModel> where TModel : class
    {
        private record Registration(CliSymbol Symbol, Func<TModel, ValidationResult> Evaluator);
        private readonly List<Registration> _registrations = new();
    
        /// <summary>
        /// Validates the given model.
        /// </summary>
        /// <param name="model">Model to validate.</param>
        /// <param name="context">Validation context.</param>
        public void Validate(TModel model, ValidationContext context)
    {
        foreach (var registration in _registrations)
        {
            var result = registration.Evaluator(model);
            if (result.IsValid)
                continue;
            
            context.AddError(registration.Symbol, result.AttemptedValue, result.Error);
        }
    }

        /// <summary>
        /// Adds a symbol specific evaluation.
        /// </summary>
        /// <param name="symbol">The symbol that is associated to the model property.</param>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="evaluator">A function that evaluates the values and returns a result.</param>
        /// <typeparam name="TValue">The value type.</typeparam>
        public void AddEvaluation<TValue>(
            CliSymbol symbol,
            Expression<Func<TModel, TValue>> memberExpression,
            Func<TModel, TValue, ValidationResult> evaluator)
    {
        _registrations.Add(new Registration(symbol, model =>
        {
            var provider = memberExpression.Compile();
            var value = provider(model);
            return evaluator(model, value);
        }));
    }
    }
}