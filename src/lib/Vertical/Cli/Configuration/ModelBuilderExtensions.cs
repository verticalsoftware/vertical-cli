using System.Collections.Immutable;
using System.Linq.Expressions;
using Vertical.Cli.Help;
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
            Action<ValidationEventInfo<TModel, IEnumerable<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, IEnumerable<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ICollection<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ICollection<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, IReadOnlyCollection<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, IReadOnlyCollection<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, List<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, List<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, IList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, IList<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, IReadOnlyList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, IReadOnlyList<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, LinkedList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, LinkedList<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ISet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ISet<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, IReadOnlySet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, IReadOnlySet<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, HashSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, HashSet<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, SortedSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, SortedSet<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Expression<Func<TModel, Stack<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<Stack<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, Stack<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, Stack<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Expression<Func<TModel, Queue<TElement>>> expression,
            int ordinalPosition,
            Arity? arity = null,
            Func<Queue<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, Queue<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, Queue<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ImmutableArray<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ImmutableArray<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ImmutableList<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ImmutableList<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ImmutableHashSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ImmutableHashSet<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ImmutableSortedSet<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ImmutableSortedSet<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ImmutableStack<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ImmutableStack<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            Action<ValidationEventInfo<TModel, ImmutableQueue<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedArgument<TElement, ImmutableQueue<TElement>>(
                expression, 
                ordinalPosition, 
                arity, 
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, IEnumerable<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ICollection<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, IReadOnlyCollection<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, IList<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, IReadOnlyList<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, List<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, LinkedList<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ISet<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, IReadOnlySet<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, HashSet<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, SortedSet<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            Expression<Func<TModel, Stack<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<Stack<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, Stack<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedOption<TElement, Stack<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            Expression<Func<TModel, Queue<TElement>>> expression,
            string[]? aliases = null,
            Arity? arity = null,
            Func<Queue<TElement>>? defaultProvider = null,
            Action<ValidationEventInfo<TModel, Queue<TElement>>>? validate = null,
            SymbolHelpTopic? helpTopic = null)
        {
            return builder.MapMultiValuedOption<TElement, Queue<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ImmutableArray<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ImmutableList<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ImmutableHashSet<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ImmutableSortedSet<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ImmutableStack<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
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
            return builder.MapMultiValuedOption<TElement, ImmutableQueue<TElement>>(
                expression, 
                aliases, 
                arity,
                defaultProvider, 
                validate, 
                helpTopic);
        }
    }
}