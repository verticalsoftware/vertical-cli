using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

internal sealed class CommandBuilder<TModel, TResult> : 
    ICommandBuilder<TModel, TResult>,
    ICommandDefinition<TModel, TResult> 
    where TModel : class
{
    private readonly PositionReference _positionReference;
    private readonly List<SymbolDefinition> _symbols = new(16);
    private readonly List<ValueConverter> _converters = new();
    private readonly List<Validator> _validators = new();
    private readonly List<(string Id, Func<ICommandDefinition<TResult>> Provider)> _commandFactories = new();

    internal CommandBuilder(string id, CliOptions options) : this(new PositionReference(), id, parent: null, options)
    {
    }

    internal CommandBuilder(
        PositionReference positionReference, 
        string id, 
        ICommandDefinition? parent,
        CliOptions options)
    {
        _positionReference = positionReference;
        Id = id;
        Parent = parent;
        Options = options;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public Type ModelType => typeof(TModel);

    /// <inheritdoc />
    public Type ResultType => typeof(TResult);

    /// <inheritdoc />
    public IReadOnlyCollection<SymbolDefinition> Symbols => _symbols;

    /// <inheritdoc />
    public IReadOnlyCollection<ValueConverter> Converters => _converters;

    /// <inheritdoc />
    public IReadOnlyCollection<Validator> Validators => _validators;

    /// <inheritdoc />
    public bool HasHandler => Handler != null;

    /// <inheritdoc />
    public string? Description { get; private set; }

    /// <inheritdoc />
    public Func<TModel, TResult>? Handler { get; private set; }

    /// <inheritdoc />
    public ICommandDefinition<TResult> Build() => new CommandDefinition<TModel, TResult>(this);

    /// <inheritdoc />
    public ICommandDefinition? Parent { get; }

    /// <inheritdoc />
    public bool TryCreateChild(string id, [NotNullWhen(true)] out ICommandDefinition<TResult>? child)
    {
        Guard.IsNotNullOrWhiteSpace(id);

        return (child = _commandFactories.FirstOrDefault(entry => entry.Id == id).Provider?.Invoke()) != null;
    }

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition<TResult>> SubCommands => _commandFactories.Select(entry => entry.Provider());

    /// <inheritdoc />
    public IEnumerable<string> SubCommandIdentities => _commandFactories.Select(entry => entry.Id);

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition> CreateChildDefinitions()
    {
        foreach (var id in SubCommandIdentities)
        {
            _ = TryCreateChild(id, out var command);
            yield return command!;
        }
    }

    /// <inheritdoc />
    public CliOptions Options { get; }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> ConfigureSubCommand(
        string id,
        Action<ICommandBuilder<TModel, TResult>> configure)
    {
        return ConfigureSubCommand<TModel>(id, configure);
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> ConfigureSubCommand<TChildModel>(
        string id,
        Action<ICommandBuilder<TChildModel, TResult>> configure) 
        where TChildModel : class
    {
        Guard.IsNotNullOrWhiteSpace(id);
        Guard.IsNotNull(configure);

        var factory = new Func<ICommandDefinition<TResult>>(() =>
        {
            var builder = new CommandBuilder<TChildModel, TResult>(_positionReference, id, this, Options);
            configure(builder);
            return builder.Build();
        });
        
        _commandFactories.Add((id, factory));
        return this;
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> SetHandler(Func<TModel, TResult> function)
    {
        Guard.IsNotNull(function);
        Handler = function;
        return this;
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddOption<T>(
        string id,
        string[]? aliases = null,
        Arity? arity = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<T>? defaultProvider = null,
        ValueConverter<T>? converter = null,
        Validator<T>? validator = null)
        
    {
        return AddSymbol(
            SymbolType.Option,
            () => OptionBinder<T>.Instance,
            id,
            aliases,
            arity ?? Arity.ZeroOrOne,
            description,
            scope,
            defaultProvider,
            converter,
            validator);
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddHelpOption(
        string? id = null,
        string[]? aliases = null,
        SymbolScope scope = SymbolScope.Self,
        IHelpRenderer? helpRenderer = null,
        TResult returnValue = default!)
    {
        _symbols.Add(new HelpSymbolDefinition<TResult>(
            this,
            _positionReference.Next(),
            id ?? "--help",
            aliases ?? Array.Empty<string>(),
            scope,
            helpRenderer ?? DefaultHelpRenderer.Instance,
            returnValue));

        return this;
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddSwitch(
        string id,
        string[]? aliases = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<bool>? defaultProvider = null)
    {
        return AddSymbol(
            SymbolType.Switch,
            () => SwitchBinder.Instance,
            id,
            aliases,
            Arity.ZeroOrOne,
            description,
            scope,
            defaultProvider ?? (() => false));
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddArgument<T>(
        string id,
        Arity? arity = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<T>? defaultProvider = null,
        ValueConverter<T>? converter = null,
        Validator<T>? validator = null)
    {
        return AddSymbol(
            SymbolType.Argument,
            () => ArgumentBinder<T>.Instance,
            id,
            aliases: null,
            arity ?? Arity.One,
            description,
            scope,
            defaultProvider,
            converter,
            validator);
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddConverter<T>(ValueConverter<T> converter)
    {
        Guard.IsNotNull(converter);
        
        _converters.Add(converter);
        return this;
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddValidator<T>(Validator<T> validator)
    {
        Guard.IsNotNull(validator);
        
        _validators.Add(validator);
        return this;
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddDescription(string description)
    {
        Guard.IsNotNullOrEmpty(description);

        Description = description;
        return this;
    }

    private ICommandBuilder<TModel, TResult> AddSymbol<T>(
        SymbolType type,
        Func<IBinder> binderFactory,
        string id,
        string[]? aliases,
        Arity arity,
        string? description = null,
        SymbolScope scope = SymbolScope.Self,
        Func<T>? defaultProvider = null,
        ValueConverter<T>? converter = null,
        Validator<T>? validator = null)
    {
        Guard.IsNotNullOrWhiteSpace(id);
        
        _symbols.Add(new SymbolDefinition<T>(
            type,
            this,
            binderFactory,
            _positionReference.Next(),
            id,
            aliases ?? Array.Empty<string>(),
            arity,
            description,
            scope,
            defaultProvider,
            converter,
            validator));

        return this;
    }
}