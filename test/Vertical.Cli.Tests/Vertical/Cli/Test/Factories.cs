using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Test;

public class Factories
{
    public static readonly ICommandDefinition DefaultCommand = new Func<ICommandDefinition>(() =>
    {
        var substitute = Substitute.For<ICommandDefinition>();
        substitute.Parent.ReturnsNull();
        return substitute;
    })();
    
    public static SymbolDefinition CreateSymbol<T>(SymbolType type, string id, params string[] aliases)
    {
        return new SymbolDefinition<T>(
            type,
            DefaultCommand,
            () => OptionBinder<T>.Instance,
            0,
            id,
            aliases,
            Arity.One,
            null,
            SymbolScope.Parent,
            null,
            null);
    }

    public static SymbolDefinition CreateSymbol<T>(
        ICommandDefinition command,
        SymbolType type,
        string id,
        string[]? aliases = null,
        Arity? arity = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Parent,
        Func<T>? defaultProvider = null,
        Validator<T>? validator = null,
        Func<IBinder>? binderFactory = null)
    {
        return new SymbolDefinition<T>(
            type,
            command,
            binderFactory ?? (() => OptionBinder<T>.Instance),
            0,
            id,
            aliases ?? Array.Empty<string>(),
            arity ?? Arity.One,
            description,
            scope,
            defaultProvider,
            validator);
    }
}