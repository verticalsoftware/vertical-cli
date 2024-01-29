using System.Collections.Immutable;
using System.Text;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration.Metadata;
using Vertical.Cli.Conversion; 
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines methods to perform verbose validation of a command.
/// </summary>
public static class ConfigurationValidator
{
    private const string Indent = "    ";
    
    private sealed class StateHelper
    {
        internal StateHelper(CliOptions options) => Options = options;
        public CliOptions Options { get; set; }
        internal ErrorMessageCollection Errors { get; } = new();
        internal HashSet<ICommandDefinition> VisitedCommands { get; } = new();
        internal Dictionary<Type, BindingModel> ModelMetadata { get; } = new();
    }
    
    /// <summary>
    /// Performs verbose inspection of a root command's configuration.
    /// </summary>
    /// <param name="rootCommand">The command to validate.</param>
    /// <typeparam name="TModel">The type of model used by the root handler implementation.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
    /// <returns>A collection of error messages, or empty if the configuration is valid.</returns>
    public static IReadOnlyCollection<string> GetErrors<TModel, TResult>(
        this IRootCommand<TModel, TResult> rootCommand)
        where TModel : class
    {
        Guard.IsNotNull(rootCommand);

        var state = new StateHelper(rootCommand.Options);
        var path = ImmutableList.Create((ICommandDefinition<TResult>)rootCommand);

        ValidateRootCommand(rootCommand, state);
        ValidateOptions(rootCommand.Options, state);
        
        return Visit(path, state)
            .Errors
            .ToArray();
    }

    /// <summary>
    /// Performs verbose inspection of a root command's configuration and throws an exception if there
    /// are any errors.
    /// </summary>
    /// <param name="rootCommand">The command to validate.</param>
    /// <typeparam name="TModel">The type of model used by the root handler implementation.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
    public static void ThrowIfInvalid<TModel, TResult>(this IRootCommand<TModel, TResult> rootCommand)
        where TModel : class
    {
        var errors = rootCommand.GetErrors();

        if (errors.Count == 0)
            return;

        var sb = new StringBuilder(5000);
        sb.AppendLine("Configuration error(s) found.");

        foreach (var error in errors)
        {
            sb.AppendLine(error);
        }

        throw new InvalidOperationException(sb.ToString());
    }

    private static void ValidateRootCommand<TModel, TResult>(
        IRootCommand<TModel, TResult> rootCommand, 
        StateHelper state) 
        where TModel : class
    {
        ValidateSpecialSymbols(rootCommand, state);
    }

    private static void ValidateSpecialSymbols<TModel, TResult>(
        IRootCommand<TModel, TResult> rootCommand,
        StateHelper state) 
        where TModel : class
    {
        var repeatedSymbols = rootCommand
            .Symbols
            .Where(symbol => symbol.SpecialType != SymbolSpecialType.None)
            .GroupBy(symbol => symbol.SpecialType)
            .Where(group => group.Count() > 1)
            .ToArray();

        if (repeatedSymbols.Length == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine("The following root command special symbols are defined more than once:");
            foreach (var grouping in repeatedSymbols)
            {
                var description = grouping.Key switch
                {
                    SymbolSpecialType.HelpOption => "help option",
                    _ => "response file option"
                };
                message.AppendLine($"{Indent}{description}");
            }
        });
    }

    private static void ValidateOptions(CliOptions options, StateHelper state)
    {
        ValidateServiceValueTypes(options.Converters, converter => converter.ValueType, "value converters", state);
        ValidateServiceValueTypes(options.Validators, validator => validator.ValueType, "validators", state);
    }

    private static void ValidateServiceValueTypes<T>(
        IEnumerable<T> services,
        Func<T, Type> typeSelector,
        string description,
        StateHelper state)
    {
        var types = services
            .Select(typeSelector)
            .GroupBy(type => type)
            .Where(type => type.Count() > 1)
            .ToArray();

        if (types.Length == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"Multiple {description} defined for the following types:");
            foreach (var type in types)
            {
                message.AppendLine($"{Indent}{type.Key} ({type.Count()} services)");
            }
        });
    }

    private static StateHelper Visit<TResult>(ImmutableList<ICommandDefinition<TResult>> path, StateHelper state)
    {
        var subject = path.Last();
        
        if (state.VisitedCommands.Add(subject))
        {
            ValidateCommandSubject(subject, state);
        }

        ValidatePath(subject, state);

        foreach (var id in subject.SubCommandIdentities)
        {
            _ = subject.TryCreateChild(id, out var child);
            Visit(path.Add(child!), state);
        }

        return state;
    }

    private static void ValidateCommandSubject(
        ICommandDefinition subject,
        StateHelper state)
    {
        ValidateSubjectSymbolIdentities(subject, state);
        ValidateSubjectCommandChildIdentitySyntax(subject, state);
        ValidateSubjectCommandChildIdentitySet(subject, state);
        ValidateHandler(subject, state);
    }

    private static void ValidatePath(ICommandDefinition subject, StateHelper state)
    {
        ValidatePathSymbols(subject, state);
        ValidatePathArgumentArity(subject, state);
        ValidatePathConverters(subject, state);
        ValidateBindingModel(subject, state);
    }

    private static void ValidatePathArgumentArity(ICommandDefinition subject, StateHelper state)
    {
        // 1. There can only be one argument with a multi-valued arity
        // 2. If there is such an argument, it has to be the last definition in the set of argument definitions. 

        var argumentSymbols = subject
            .GetAllSymbols()
            .Where(symbol => symbol.Kind == SymbolKind.Argument)
            .OrderBy(symbol => symbol.Position)
            .ToArray();

        var multiArityArgumentSymbols = argumentSymbols
            .Where(symbol => symbol.Arity.IsMultiValue)
            .ToArray();

        if (!multiArityArgumentSymbols.Any())
            return;

        if (multiArityArgumentSymbols.Length != 1)
        {
            state.Errors.Add(message =>
            {
                message.AppendLine($"Command path \"{subject.GetPathString()}\" has more than one multi-valued " +
                                   "argument symbol defined:");
                foreach (var symbol in multiArityArgumentSymbols)
                {
                    message.AppendLine($"{Indent}{symbol.GetFUllDisplayString()}");
                }
            });
            return;
        }

        var multiAritySymbol = multiArityArgumentSymbols.Single();
        
        if (ReferenceEquals(multiAritySymbol, argumentSymbols.Last()))
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"The multi-arity argument symbol in \"{subject.GetPathString()}\" must be defined " +
                               "last among the arguments:");

            foreach (var symbol in argumentSymbols.SkipWhile(item => !ReferenceEquals(item, multiAritySymbol)))
            {
                message.AppendLine(ReferenceEquals(symbol, multiAritySymbol)
                    ? $"{Indent}*remove >> {symbol.GetFUllDisplayString()}"
                    : $"{Indent}{symbol.GetFUllDisplayString()}");
            }
            message.AppendLine($"{Indent}*insert >> {multiAritySymbol.GetFUllDisplayString()}");
        });
    }

    private static void ValidateBindingModel(ICommandDefinition subject, StateHelper state)
    {
        var modelType = subject.ModelType;

        if (modelType == typeof(None))
            return;

        if (!state.ModelMetadata.TryGetValue(modelType, out var modelMetadata))
        {
            modelMetadata = new BindingModel(modelType);
            state.ModelMetadata.Add(modelType, modelMetadata);
            ValidateBindingModelConstructors(modelMetadata, state);
            ValidateBindingModelAttributes(modelMetadata, state);
        }
        
        ValidateBindingModelParameters(subject, modelMetadata, state);
    }

    private static void ValidateBindingModelAttributes(BindingModel modelMetadata, StateHelper state)
    {
        if (!MetadataHelpers.HasModelBinderAttribute(modelMetadata.Type))
            return;

        
        if (state.Options.ContainsBinderRegistration(modelMetadata.Type))
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"Class {modelMetadata.Type} is marked for custom binding, but there is not a " +
                               "binder registered in options.");
        });
    }

    private static void ValidateBindingModelParameters(
        ICommandDefinition subject,
        BindingModel model,
        StateHelper state)
    {
        if (!subject.HasHandler)
            return;
        
        // Verify..
        // 1. each symbol is bound to a parameter
        // 2. each parameter is bound to a symbol
        // 3. parameter types are compatible with the symbol definition.
        // 4. the generic types of collections or element type of arrays are compatible with the symbol definition  
        var parameters = model.BindingParameters;
        var errors = new List<string>();
        var matchedParameters = new HashSet<BindingParameter>();
        
        foreach (var symbol in subject.GetAllSymbols())
        {
            if (symbol.SpecialType != SymbolSpecialType.None)
                continue;
            
            var matched = model.FindParameters(symbol).ToArray();

            if (matched.Length == 0)
            {
                errors.Add($"{Indent}Symbol \"{symbol.GetFUllDisplayString()}\" is not bound to any constructor parameter, " +
                           "property, or field.");
                continue;
            }

            var hasMultiValuedArity = symbol.Arity.IsMultiValue;

            foreach (var parameter in matched)
            {
                matchedParameters.Add(parameter);

                var collectionType = MetadataHelpers.GetGenericCollectionType(parameter.Type);
                var isCollectionTypeParameter = collectionType != null;

                if (hasMultiValuedArity)
                {
                    if (!isCollectionTypeParameter)
                    {
                        errors.Add($"{Indent}Values for {symbol.GetDisplayString(quoteIdentities: true)} cannot be bound to " +
                                   $"{parameter} because it is not an array or supported collection type.");
                        continue;
                    }

                    if (!collectionType!.IsAssignableFrom(symbol.ValueType))
                    {
                        errors.Add($"{Indent}Values for {symbol.GetDisplayString(quoteIdentities: true)} cannot be bound to " +
                                   $"{parameter} because {symbol.ValueType} is not assignable to the array " +
                                   $"or collection type {collectionType}.");
                    }
                    continue;
                }

                if (!parameter.Type.IsAssignableFrom(symbol.ValueType))
                {
                    errors.Add($"{Indent}Values for {symbol.GetDisplayString(quoteIdentities: true)} cannot be bound to " +
                                $"\"{parameter}\" because {symbol.ValueType} is not assignable to {parameter.Type}.");
                }
            }
        }
        
        errors.AddRange(parameters
            .Where(param => !matchedParameters.Contains(param))
            .Select(parameter =>
            {
                var customBinding = parameter.BindingId != parameter.MetadataName
                    ? $" (BindTo=\"{parameter.BindingId}\")"
                    : string.Empty;
                
                return $"{Indent}Binding {parameter}{customBinding} does not bind to any defined symbols.";
            }));

        if (errors.Count == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"Parameter model type {model.Type.FullName} cannot be bound " + 
                               $"in path \"{subject.GetPathString()}\" for the following reason(s):");
            message.AppendLines(errors);
        }); 
    }

    private static void ValidateBindingModelConstructors(BindingModel model, StateHelper state)
    {
        // Verify the model has a single constructor
        
        if (model.ConstructorTarget != null)
            return;

        switch (model.Constructors.Length)
        {
            case 0:
                state.Errors.Add(message => message.Append($"Parameter model type {model.Type.FullName} " +
                                                           "does not have an accessible constructor."));
                break;
            
            default:
                state.Errors.Add(message => message.Append($"Parameter model type {model.Type.FullName} " +
                                                           "has multiple constructors."));
                break;
        }
    }

    private static void ValidatePathConverters(ICommandDefinition subject, StateHelper state)
    {
        // Verify there are converters available for every type of symbol definition.
        var requiredConverterGroups = subject
            .GetAllSymbols()
            .Select(symbol => (symbol, converterType: symbol.ValueType))
            .GroupBy(item => item.converterType, item => item.symbol);

        var availableConvertersTypes = state
            .Options
            .Converters
            .Select(converter => converter.ValueType)
            .ToArray();

        var missingConverterGroups = requiredConverterGroups
            .Where(group => !(availableConvertersTypes.Contains(group.Key) || DefaultConverter.CanConvert(group.Key)))
            .ToArray();

        if (missingConverterGroups.Length == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"One or more symbols aren't bindable in path \"{subject.GetPathString()}\" because their value types " +
                               "aren't supported by a ValueConverter:");

            foreach (var group in missingConverterGroups)
            {
                message.AppendLine($"{Indent}{group.Key}");
                foreach (var symbol in group)
                {
                    message.AppendLine($"{Indent}{Indent}used by {symbol.GetDisplayString(quoteIdentities: true)}");
                }
            }
        });
    }

    private static void ValidateHandler(ICommandDefinition subject, StateHelper state)
    {
        // A command requires a handler if it has scoped symbol definitions in the path
        
        if (subject.HasHandler)
            return;
        
        var hasSymbols = subject.Symbols.Any(symbol => symbol is
        {
            Scope: SymbolScope.Parent or SymbolScope.ParentAndDescendents,
            SpecialType: SymbolSpecialType.None
        });

        if (!hasSymbols)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"Command \"{subject.GetPathString()}\" must implement a handler because it has " +
                               "symbols in scope.");
        });
    }

    private static void ValidatePathSymbols(ICommandDefinition subject, StateHelper state)
    {
        // Verify all identities of all symbols are unique in a path

        var symbols = subject.GetAllSymbols();
        var identityGroups = symbols
            .SelectMany(symbol => symbol.Identities.Select(identity => (symbol, identity)))
            .GroupBy(item => item.identity, item => item.symbol)
            .Where(grouping => grouping.Count() > 1)
            .ToArray();

        if (identityGroups.Length == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine("Duplicate symbol identities found in path:");
            foreach (var group in identityGroups)
            {
                message.AppendLine($"{Indent}Identity \"{group.Key}\"");
                foreach (var symbol in group)
                {
                    message.AppendLine($"{Indent}{Indent}in {symbol.GetDisplayString()}");
                }
            }
        });
    }

    private static void ValidateSubjectCommandChildIdentitySyntax(
        ICommandDefinition command, 
        StateHelper state)
    {
        // Commands must have simple identity names 
        
        var invalidIdentities = command
            .SubCommandIdentities
            .Where(id => SymbolSyntax.Parse(id).Type != SymbolSyntaxType.Simple)
            .ToArray();

        if (invalidIdentities.Length == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"Invalid sub-command identities in command \"{command.GetPathString()}\":");
            foreach (var identity in invalidIdentities)
            {
                message.AppendLine($"{Indent}\"{identity}\"");
            }
        });
    }

    private static void ValidateSubjectCommandChildIdentitySet(
        ICommandDefinition command,
        StateHelper state)
    {
        // All sub-commands must have a unique identity to peers
        
        var duplicateIdentities = command
            .SubCommandIdentities
            .GroupBy(id => id)
            .Where(grouping => grouping.Count() > 1)
            .Select(grouping => grouping.Key)
            .ToArray();

        if (duplicateIdentities.Length == 0)
            return;
        
        state.Errors.Add(message =>
        {
            var csv = string.Join(", ", duplicateIdentities.Select(id => $"\"{id}\""));
            message.AppendLine($"Duplicate sub-command identities in command \"{command.GetPathString()}\": {csv}");
        });
    }

    private static void ValidateSubjectSymbolIdentities(
        ICommandDefinition command, 
        StateHelper state)
    {
        // 1. Argument identities must have simple syntax
        // 2. Option/switch identities must have prefixed syntax
        
        var symbols = command.Symbols;
        var invalidEntries = new List<(SymbolDefinition symbol, string identity)>();
        
        invalidEntries.AddRange(SelectInvalidSyntax(
            symbols, 
            type => type == SymbolKind.Argument,
            syntax => syntax.Type == SymbolSyntaxType.Simple));
        
        invalidEntries.AddRange(SelectInvalidSyntax(
            symbols,
            type => type is SymbolKind.Option or SymbolKind.Switch,
            syntax => syntax.IsPrefixed));

        if (invalidEntries.Count == 0)
            return;
        
        state.Errors.Add(message =>
        {
            message.AppendLine($"Invalid symbol identities found in command \"{command.GetPathString()}\":");
            foreach (var (symbol, identity) in invalidEntries)
            {
                message.AppendLine($"{Indent}\"{identity}\" in {symbol.GetDisplayString()}");
            }
        });
    }

    private static IEnumerable<(SymbolDefinition, string)> SelectInvalidSyntax(
        this IEnumerable<SymbolDefinition> symbols,
        Predicate<SymbolKind> typePredicate,
        Predicate<SymbolSyntax> syntaxPredicate)
    {
        return symbols
            .Where(symbol => typePredicate(symbol.Kind))
            .SelectMany(symbol => symbol.Identities.Select(identity => (symbol, identity)))
            .Where(item => !syntaxPredicate(SymbolSyntax.Parse(item.identity)));
    }
}