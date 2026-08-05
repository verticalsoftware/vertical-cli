using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Validation;

/// <summary>
/// Represents a context for validation.
/// </summary>
public sealed class ValidationContext
{
    private readonly List<CommandLineError> _errorList = [];
    
    private ValidationContext(object model, IEnumerable<IValidatable> subjects, IHelpProvider helpProvider)
    {
        Model = model;
        Subjects = subjects;
        HelpProvider = helpProvider;
    }

    /// <summary>
    /// Aggregates the results of validation on the given symbols.
    /// </summary>
    /// <param name="context">The invocation context.</param>
    /// <param name="subjects">The subject collection to validate.</param>
    /// <param name="model">The constructed model.</param>
    /// <returns><see cref="IEnumerable{T}"/></returns>
    public static IEnumerable<CommandLineError> GetErrors(
        InvocationContext context,
        IEnumerable<IValidatable> subjects,
        object model)
    {
        return new ValidationContext(
                model,
                subjects,
                context.Configuration.HelpOptions.HelpProvider)
            .Validate();
    }

    /// <summary>
    /// Gets the model that contains the data to validate.
    /// </summary>
    public object Model { get; }

    /// <summary>
    /// Gets the symbols being validated.
    /// </summary>
    public IEnumerable<IValidatable> Subjects { get; }

    /// <summary>
    /// Gets the help provider.
    /// </summary>
    public IHelpProvider HelpProvider { get; }

    public void AddError(SymbolValidationError error)
    {
        _errorList.Add(error);
    }

    private List<CommandLineError> Validate()
    {
        foreach (var symbol in Subjects)
        {
            symbol.Validate(this);
        }

        return _errorList;
    }
}