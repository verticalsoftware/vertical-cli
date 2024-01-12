using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Invocation;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

internal class CommandBuilder<TModel, TResult> : 
    ICommandBuilder<TModel, TResult>,
    ICommandDefinition<TModel, TResult> 
    where TModel : class
{
    private readonly PositionReference _positionReference;
    private readonly List<SymbolDefinition> _symbols = new(16);
    private readonly List<(string Id, Func<ICommandDefinition<TResult>> Provider)> _commandFactories = new();

    protected CommandBuilder(string id) : this(new PositionReference(), id, parent: null)
    {
    }

    private CommandBuilder(
        PositionReference positionReference, 
        string id, 
        ICommandDefinition? parent)
    {
        _positionReference = positionReference;
        Id = id;
        Parent = parent;
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
    public bool HasHandler => Handler != null;

    /// <inheritdoc />
    public string? Description { get; private set; }

    /// <inheritdoc />
    public Func<TModel, CancellationToken, TResult>? Handler { get; private set; }

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
    public ICallSite<TResult> CreateCallSite()
    {
        // Shouldn't create call sites from here
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerable<string> SubCommandIdentities => _commandFactories.Select(entry => entry.Id);

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition> GetChildDefinitions()
    {
        foreach (var id in SubCommandIdentities)
        {
            _ = TryCreateChild(id, out var command);
            yield return command!;
        }
    }

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
            var builder = new CommandBuilder<TChildModel, TResult>(_positionReference, id, this);
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

        return SetHandler((model, _) => function(model));
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> SetHandler(Func<TModel, CancellationToken, TResult> function)
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
        SymbolScope scope = SymbolScope.Parent,
        Func<T>? defaultProvider = null,
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
            validator);
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> Family(
        string id,
        string[]? aliases = null,
        string? description = null,
        SymbolScope scope = SymbolScope.Parent,
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
        SymbolScope scope = SymbolScope.Parent,
        Func<T>? defaultProvider = null,
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
            validator);
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddDescription(string description)
    {
        Guard.IsNotNullOrEmpty(description);

        Description = description;
        return this;
    }

    /// <summary>
    /// Adds a symbol.
    /// </summary>
    /// <param name="symbol">The symbol definition.</param>
    protected void AddSymbol(SymbolDefinition symbol)
    {
        _symbols.Add(symbol);
    }

    /// <summary>
    /// Gets the position reference.
    /// </summary>
    /// <returns></returns>
    protected int GetInsertPosition() => _positionReference.Next();

    private ICommandBuilder<TModel, TResult> AddSymbol<T>(
        SymbolType type,
        Func<IBinder> binderFactory,
        string id,
        string[]? aliases,
        Arity arity,
        string? description = null,
        SymbolScope scope = SymbolScope.Parent,
        Func<T>? defaultProvider = null,
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
            validator));

        return this;
    }
}