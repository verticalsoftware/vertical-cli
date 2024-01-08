using Vertical.Cli.Conversion;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Provides a fluent interface for configuring command definitions.
/// </summary>
/// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
public interface ICommandBuilder<out TModel, TResult> where TModel : class
{
    /// <summary>
    /// Constructs a command definition using the configuration in this instance.
    /// </summary>
    /// <returns><see cref="ICommandDefinition{TResult}"/></returns>
    ICommandDefinition<TResult> Build();
    
    /// <summary>
    /// Configures a sub command that uses the same model type as this instance.
    /// </summary>
    /// <param name="id">The identity of the sub command.</param>
    /// <param name="configure">A delegate that performs the sub command configuration.</param>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> ConfigureSubCommand(
        string id,
        Action<ICommandBuilder<TModel, TResult>> configure);
    
    /// <summary>
    /// Configures a sub command.
    /// </summary>
    /// <param name="id">The identity of the sub command.</param>
    /// <param name="configure">A delegate that performs the sub command configuration.</param>
    /// <typeparam name="TChildModel">The parameter model type for the sub command.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> ConfigureSubCommand<TChildModel>(
        string id,
        Action<ICommandBuilder<TChildModel, TResult>> configure)
        where TChildModel : class;
    
    /// <summary>
    /// Sets the handler function.
    /// </summary>
    /// <param name="function">The function that handles the application's implementation of the command.</param>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> SetHandler(Func<TModel, TResult> function);

    /// <summary>
    /// Adds an option definition.
    /// </summary>
    /// <param name="id">The primary identity for the option.</param>
    /// <param name="aliases">An optional array of alias identities.</param>
    /// <param name="arity">An arity that describes the use of the option.</param>
    /// <param name="description">A description of the option.</param>
    /// <param name="scope">The scope of the symbol within the command path.</param>
    /// <param name="defaultProvider">A function that provides a default value.</param>
    /// <param name="converter">An object that performs a custom conversion of the string argument.</param>
    /// <param name="validator">An object that performs cursory validation of the value.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> AddOption<T>(
        string id,
        string[]? aliases = null,
        Arity? arity = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<T>? defaultProvider = null,
        ValueConverter<T>? converter = null,
        Validator<T>? validator = null)
        where T : notnull;

    /// <summary>
    /// Adds an option definition.
    /// </summary>
    /// <param name="id">The primary identity for the option.</param>
    /// <param name="aliases">An optional array of alias identities.</param>
    /// <param name="description">A description of the option.</param>
    /// <param name="scope">The scope of the symbol within the command path.</param>
    /// <param name="defaultProvider">A function that provides a default value.</param>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> AddSwitch(
        string id,
        string[]? aliases = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<bool>? defaultProvider = null);
    
    /// <summary>
    /// Adds an option definition.
    /// </summary>
    /// <param name="id">The primary identity for the option.</param>
    /// <param name="arity">An arity that describes the use of the argument.</param>
    /// <param name="description">A description of the option.</param>
    /// <param name="scope">The scope of the symbol within the command path.</param>
    /// <param name="defaultProvider">A function that provides a default value.</param>
    /// <param name="converter">An object that performs a custom conversion of the string argument.</param>
    /// <param name="validator">An object that performs cursory validation of the value.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> AddArgument<T>(
        string id,
        Arity? arity = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<T>? defaultProvider = null,
        ValueConverter<T>? converter = null,
        Validator<T>? validator = null)
        where T : notnull;

    /// <summary>
    /// Adds a converter service available to this command and any sub command in the path.
    /// </summary>
    /// <param name="converter">Converter instance.</param>
    /// <typeparam name="T">Value type the converter supports.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> AddConverter<T>(ValueConverter<T> converter) where T : notnull;
    
    /// <summary>
    /// Adds a validator service available to this command and any sub command in the path.
    /// </summary>
    /// <param name="validator">Validator instance.</param>
    /// <typeparam name="T">Value type the validator supports.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ICommandBuilder<TModel, TResult> AddValidator<T>(Validator<T> validator) where T : notnull;
}