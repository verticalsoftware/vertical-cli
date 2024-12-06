using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

internal sealed class ModelValidator<TModel> where TModel : class
{
    private readonly List<ArgumentValidator<TModel>> validators = new(8);
    
    public void AddRange(IEnumerable<ArgumentValidator<TModel>> argumentValidators)
    {
        validators.AddRange(argumentValidators);
    }
    
    public void Validate(TModel model, Func<CliParameter, string, object?, CliArgumentException> exceptionFactory)
    {
        foreach (var validator in validators.Where(validator => !validator.Validate(model)))
        {
            throw exceptionFactory(validator.Parameter, validator.Message, validator.GetValue(model));
        }
    }
}