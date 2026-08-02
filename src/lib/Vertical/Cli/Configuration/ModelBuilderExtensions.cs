using System.Collections.Immutable;
using System.Linq.Expressions;
using Vertical.Cli.Binding;
using Vertical.Cli.Help;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

public static class ModelBuilderExtensions
{
    extension<TModel>(ModelBuilder<TModel> builder) where TModel : class
    {
        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, IEnumerable<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<IEnumerable<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, IEnumerable<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IEnumerable<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IEnumerable<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ICollection<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ICollection<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ICollection<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ICollection<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ICollection<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, IReadOnlyCollection<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<IReadOnlyCollection<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, IReadOnlyCollection<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IReadOnlyCollection<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IReadOnlyCollection<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, IList<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<IList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, IList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IList<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, IReadOnlyList<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<IReadOnlyList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, IReadOnlyList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IReadOnlyList<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IReadOnlyList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, List<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<List<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, List<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, List<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, List<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, LinkedList<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<LinkedList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, LinkedList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, LinkedList<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, LinkedList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ISet<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ISet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ISet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ISet<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ISet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, IReadOnlySet<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<IReadOnlySet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, IReadOnlySet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IReadOnlySet<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IReadOnlySet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, HashSet<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<HashSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, HashSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, HashSet<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, HashSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, SortedSet<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<SortedSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, SortedSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, SortedSet<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, SortedSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ImmutableArray<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ImmutableArray<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ImmutableArray<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableArray<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableArray<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ImmutableList<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ImmutableList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ImmutableList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableList<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ImmutableHashSet<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ImmutableHashSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ImmutableHashSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableHashSet<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableHashSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ImmutableSortedSet<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ImmutableSortedSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ImmutableSortedSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableSortedSet<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableSortedSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ImmutableStack<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ImmutableStack<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ImmutableStack<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableStack<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableStack<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with position argument input that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="ordinalPosition">
        /// The expected position of the argument in relation to other position arguments.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedArgument<TElement>(
            Expression<Func<TModel, ImmutableQueue<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<ImmutableQueue<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, TElement, ImmutableQueue<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableQueue<TElement>>(
                expression,
                SymbolKind.PositionArgument,
                ordinalPosition,
                aliases: [],
                arity: arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableQueue<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, IEnumerable<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<IEnumerable<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, IEnumerable<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IEnumerable<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IEnumerable<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ICollection<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ICollection<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ICollection<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ICollection<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ICollection<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, IReadOnlyCollection<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<IReadOnlyCollection<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, IReadOnlyCollection<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IReadOnlyCollection<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IReadOnlyCollection<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, IList<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<IList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, IList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IList<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, IReadOnlyList<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<IReadOnlyList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, IReadOnlyList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IReadOnlyList<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IReadOnlyList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, List<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<List<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, List<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, List<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, List<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, LinkedList<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<LinkedList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, LinkedList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, LinkedList<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, LinkedList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ISet<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ISet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ISet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ISet<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ISet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, IReadOnlySet<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<IReadOnlySet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, IReadOnlySet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, IReadOnlySet<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, IReadOnlySet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, HashSet<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<HashSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, HashSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, HashSet<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, HashSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, SortedSet<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<SortedSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, SortedSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, SortedSet<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, SortedSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ImmutableArray<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ImmutableArray<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ImmutableArray<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableArray<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableArray<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ImmutableList<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ImmutableList<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ImmutableList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableList<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableList<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ImmutableHashSet<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ImmutableHashSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ImmutableHashSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableHashSet<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableHashSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ImmutableSortedSet<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ImmutableSortedSet<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ImmutableSortedSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableSortedSet<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableSortedSet<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ImmutableStack<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ImmutableStack<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ImmutableStack<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableStack<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableStack<TElement>>(self)));

            return builder;
        }

        /// <summary>
        /// Associates a model property with named option inputs that can be specified more than once.
        /// </summary>
        /// <param name="expression">Expression that identifies the model's property.</param>
        /// <param name="aliases">
        /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
        /// property name.
        /// </param>
        /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
        /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
        /// <param name="validate">A delegate that performs data validation checks.</param>
        /// <param name="helpTopic">The help topic associated with the argument.</param>
        /// <typeparam name="TElement">The value type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ModelBuilder<TModel> MapMultiValuedOption<TElement>(
            Expression<Func<TModel, ImmutableQueue<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<ImmutableQueue<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, ImmutableQueue<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            var bindingName = expression.BindingName;

            builder.Configuration.AddBindingSource(new CliSymbol<TModel, ImmutableQueue<TElement>>(
                expression,
                SymbolKind.Option,
                0,
                ArgumentSyntax.ValidateAliasesOrGetDefault(bindingName, aliases),
                arity ?? Arity.ZeroOrMore,
                defaultProvider,
                helpTopic,
                ValidationHelpers.TryCreateValidationAction(validate),
                self => new CollectionPropertyBinder<TModel, TElement, ImmutableQueue<TElement>>(self)));

            return builder;
        }
    }
}