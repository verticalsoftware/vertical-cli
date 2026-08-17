using System.Linq.Expressions;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an object used to configure parsing operations.
/// </summary>
/// <typeparam name="TModel">The binding model type.</typeparam>
public interface IParserBuilder<TModel> where TModel : class
{
    /// <summary>
    /// Associates a model property with a position argument input.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="ordinalPosition">
    /// The expected position of the argument in relation to other position arguments.
    /// </param>
    /// <param name="required">A flag indicating whether a value for the argument is required.</param>
    /// <param name="useDefault">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseArgument<TValue>(
        Expression<Func<TModel, TValue>> expression,
        int ordinalPosition,
        bool required = false,
        Func<TValue>? useDefault = null,
        Action<IValidationEventInfo<TModel, TValue>>? validate = null,
        SymbolHelpTopic? helpTopic = null);

    /// <summary>
    /// Associates a model property with position argument input that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="ordinalPosition">
    /// The expected position of the argument in relation to other position arguments.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="useDefault">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseRepeatableArgument<TElement>(
        Expression<Func<TModel, TElement[]>> expression,
        int ordinalPosition,
        Arity? arity = null,
        Func<TElement[]>? useDefault = null,
        Action<IValidationEventInfo<TModel, TElement, TElement[]>>? validate = null,
        SymbolHelpTopic? helpTopic = null);

    /// <summary>
    /// Associates a model property with position argument input that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="ordinalPosition">
    /// The expected position of the argument in relation to other position arguments.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="useDefault">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <typeparam name="TCollection">The property's collection type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseRepeatableArgument<TElement, TCollection>(
        Expression<Func<TModel, TCollection>> expression,
        int ordinalPosition,
        Arity? arity = null,
        Func<TCollection>? useDefault = null,
        Action<IValidationEventInfo<TModel, TElement, TCollection>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
        where TCollection : IEnumerable<TElement>;

    /// <summary>
    /// Associates a model property with a named option input.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="required">A flag indicating whether a value for the argument is required.</param>
    /// <param name="useDefault">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseOption<TValue>(
        Expression<Func<TModel, TValue>> expression,
        AliasList? aliases = null,
        bool required = false,
        Func<TValue>? useDefault = null,
        Action<IValidationEventInfo<TModel, TValue>>? validate = null,
        SymbolHelpTopic? helpTopic = null);

    /// <summary>
    /// Associates a model property with named option inputs that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="useDefault">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseRepeatableOption<TElement>(
        Expression<Func<TModel, TElement[]>> expression,
        AliasList? aliases = null,
        Arity? arity = null,
        Func<TElement[]>? useDefault = null,
        Action<IValidationEventInfo<TModel, TElement, TElement[]>>? validate = null,
        SymbolHelpTopic? helpTopic = null);

    /// <summary>
    /// Associates a model property with named option inputs that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="useDefault">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <typeparam name="TCollection">The property's collection type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseRepeatableOption<TElement, TCollection>(
        Expression<Func<TModel, TCollection>> expression,
        AliasList? aliases = null,
        Arity? arity = null,
        Func<TCollection>? useDefault = null,
        Action<IValidationEventInfo<TModel, TCollection>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
        where TCollection : IEnumerable<TElement>;

    /// <summary>
    /// Associates a boolean model property to a switch input.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <returns>A reference to this instance.</returns>
    ModelBuilder<TModel> ParseSwitch(
        Expression<Func<TModel, bool>> expression,
        AliasList? aliases = null,
        Action<IValidationEventInfo<TModel, bool>>? validate = null,
        HelpTopic? helpTopic = null);
}