using Vertical.Cli.Configuration;
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
    
    private ValidationContext(object model, IEnumerable<CliSymbol> symbols, IHelpProvider helpProvider)
    {
        Model = model;
        Symbols = symbols;
        HelpProvider = helpProvider;
    }

    /// <summary>
    /// Aggregates the results of validation on the given symbols.
    /// </summary>
    /// <param name="context">The invocation context.</param>
    /// <param name="symbols">The symbols to validate.</param>
    /// <param name="model">The constructed model.</param>
    /// <returns><see cref="IEnumerable{T}"/></returns>
    public static IEnumerable<CommandLineError> GetErrors(
        InvocationContext context,
        IEnumerable<CliSymbol> symbols,
        object model)
    {
        return new ValidationContext(
                model,
                symbols,
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
    public IEnumerable<CliSymbol> Symbols { get; }

    /// <summary>
    /// Gets the help provider.
    /// </summary>
    public IHelpProvider HelpProvider { get; }

    public void AddError(SymbolValidationError error)
    {
        _errorList.Add(error);
    }

    private IEnumerable<CommandLineError> Validate()
    {
        foreach (var symbol in Symbols)
        {
            symbol.Validate(this);
        }

        return _errorList;
    }
}