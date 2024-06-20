using System.Linq.Expressions;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Metadata;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Super class for all command types.
/// </summary>
[NoGeneratorBinding]
public abstract class CliCommand : CliObject, ICliSymbol
{
    private readonly List<CliSymbol> _symbols = new(6);
    private readonly List<CliCommand> _subCommands = [];
    private readonly List<ModelessTaskConfiguration> _modelessTasks = [];

    private protected CliCommand(
        Type modelType,
        int index,
        string[] names,
        string? description,
        SymbolId symbolId,
        CliCommand? parent)
        : base(names, description)
    {
        ModelType = modelType;
        Index = index;
        Parent = parent;
        SymbolId = symbolId;
    }

    internal SymbolId SymbolId { get; }

    /// <summary>
    /// Adds a symbol to the underlying collection.
    /// </summary>
    /// <param name="symbol">Symbol</param>
    protected void AddSymbol(CliSymbol symbol) => _symbols.Add(symbol);

    /// <summary>
    /// Adds a sub command to the underlying collection.
    /// </summary>
    /// <param name="command">Command</param>
    protected void AddSubCommand(CliCommand command) => _subCommands.Add(command);

    /// <summary>
    /// Adds a short task to the underlying collection.
    /// </summary>
    /// <param name="configuration">Configuration</param>
    protected void AddModelessTask(ModelessTaskConfiguration configuration) => _modelessTasks.Add(configuration);

    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Gets the parent command, or <c>null</c> if this is the root command.
    /// </summary>
    public CliCommand? Parent { get; }

    /// <summary>
    /// Gets the symbols defined in the collection.
    /// </summary>
    public IReadOnlyCollection<CliSymbol> Symbols => _symbols;

    /// <summary>
    /// Gets the collection of child commands defined by this instance.
    /// </summary>
    public IEnumerable<CliCommand> SubCommands => _subCommands;

    /// <summary>
    /// Gets the collection of short tasks.
    /// </summary>
    public IEnumerable<ModelessTaskConfiguration> ModelessTasks => _modelessTasks;
    
    /// <summary>
    /// Displays help content to the standard output stream.
    /// </summary>
    /// <param name="command">Command to show help content for, defaults to the current instance.</param>
    public void DisplayHelp(CliCommand? command = null) => HelpTaskConfiguration
        .WriteHelpToConsole(command ?? this, this.GetOptions());

    /// <summary>
    /// Performs verbose checking of the configuration.
    /// </summary>
    /// <param name="messages">Message collection to populate.</param>
    internal abstract void VerifyConfiguration(ICollection<string> messages);

    internal abstract void ValidateModel(object model, ValidationContext context);

    /// <inheritdoc />
    public override string ToString() => Names.Length > 1 ? $"[{string.Join(',', Names)}]" : Names[0];

    /// <inheritdoc />
    public string? OperandSyntax => null;

    /// <inheritdoc />
    public int Index { get; }
}

/// <summary>
/// Represents an object used to define commands.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public partial class CliCommand<TModel> : CliCommand where TModel : class
{
    private readonly ModelValidator<TModel> _validator = new();
    private Func<TModel, CancellationToken, Task<int>>? _handler;

    internal CliCommand(
        int index,
        string[] names,
        string? description,
        SymbolId symbolId,
        Func<TModel, CancellationToken, Task<int>>? handler = null,
        CliCommand? parent = null) 
        : base(typeof(TModel), index, names, description, symbolId, parent)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets the function that handle's the logic of the command.
    /// </summary>
    public Func<TModel, CancellationToken, Task<int>> Handler
    {
        get => _handler ?? ThrowingHandler;
        private set => _handler = value;
    }

    /// <summary>
    /// Adds a positional argument.
    /// </summary>
    /// <param name="memberExpression">Expression that identifies the model property to bind to.</param>
    /// <param name="arity">Arity of the argument's operands (defaults to <see cref="Arity.One"/>).</param>
    /// <param name="scope">
    /// A <see cref="CliScope"/> that specifies the applicability of the option in the
    /// command path.
    /// </param>
    /// <param name="description">A description of the command that can be displayed in help content.</param>
    /// <param name="operandSyntax">The syntax to display for the value operand.</param>
    /// <param name="validation">
    /// An action that provides an evaluator that determines the validity of the value provided by the client.
    /// </param>
    /// <typeparam name="TValue">Value type.</typeparam>
    public CliCommand<TModel> AddArgument<TValue>(
        Expression<Func<TModel, TValue>> memberExpression,
        Arity? arity = null,
        CliScope scope = CliScope.Self,
        string? description = null,
        string? operandSyntax = null,
        Action<ValidationBuilder<TModel, TValue>>? validation = null)
    {
        Guard.IsNotNull(memberExpression, nameof(memberExpression));

        var symbol = new CliSymbol<TValue>(
            this,
            SymbolType.Argument,
            SymbolId.Next(),
            memberExpression.GetMemberName(),
            [],
            arity ?? Arity.One,
            scope,
            null,
            description,
            operandSyntax);

        AddSymbol(symbol);
        TryAddValidator(symbol, memberExpression, validation);
        
        return this;
    }

    /// <summary>
    /// Adds an option.
    /// </summary>
    /// <param name="memberExpression">Expression that identifies the model property to bind to.</param>
    /// <param name="names">Names the option can be referred during argument input.</param>
    /// <param name="arity">Arity of the option (defaults to <see cref="Arity.ZeroOrOne"/>.</param>
    /// <param name="scope">
    /// A <see cref="CliScope"/> that specifies the applicability of the option in the
    /// command path.
    /// </param>
    /// <param name="defaultProvider">A function that provides a default value if the client does not provide one. </param>
    /// <param name="description">A description of the command that can be displayed in help content.</param>
    /// <param name="operandSyntax">The notation to display for the operand value.</param>
    /// <param name="validation">
    /// An action that provides an evaluator that determines the validity of the value provided by the client.
    /// </param>
    /// <typeparam name="TValue">Value type.</typeparam>
    public CliCommand<TModel> AddOption<TValue>(
        Expression<Func<TModel, TValue>> memberExpression,
        string[] names,
        Arity? arity = null,
        CliScope scope = CliScope.Self,
        Func<TValue>? defaultProvider = null,
        string? description = null,
        string? operandSyntax = null,
        Action<ValidationBuilder<TModel, TValue>>? validation = null)
    {
        Guard.IsNotNull(memberExpression, nameof(memberExpression));
        Guard.IsNotNull(names, nameof(names));
        Guard.IsNotEmpty(names, nameof(names));

        var symbol = new CliSymbol<TValue>(
            this,
            SymbolType.Option,
            SymbolId.Next(),
            memberExpression.GetMemberName(),
            names,
            arity ?? Arity.ZeroOrOne,
            scope,
            defaultProvider,
            description,
            operandSyntax);
        
        AddSymbol(symbol);
        TryAddValidator(symbol, memberExpression, validation);
        
        return this;
    }

    /// <summary>
    /// Adds a switch.
    /// </summary>
    /// <param name="memberExpression">Expression that identifies the model property to bind to.</param>
    /// <param name="names">Names the option can be referred during argument input.</param>
    /// <param name="scope">
    /// A <see cref="CliScope"/> that specifies the applicability of the option in the command path.</param>
    /// <param name="defaultProvider">A function that provides a default value if the client does not provide one. </param>
    /// <param name="description">A description of the command that can be displayed in help content.</param>
    /// <param name="validation">
    /// An action that provides an evaluator that determines the validity of the value provided by the client.
    /// </param>
    public CliCommand<TModel> AddSwitch(
        Expression<Func<TModel, bool>> memberExpression,
        string[] names,
        CliScope scope = CliScope.Self,
        Func<bool>? defaultProvider = null,
        string? description = null,
        Action<ValidationBuilder<TModel, bool>>? validation = null)
    {
        Guard.IsNotNull(memberExpression, nameof(memberExpression));
        Guard.IsNotNull(names, nameof(names));
        Guard.IsNotEmpty(names, nameof(names));

        var symbol = new CliSymbol<bool>(
            this,
            SymbolType.Switch,
            SymbolId.Next(),
            memberExpression.GetMemberName(),
            names,
            Arity.One,
            scope,
            defaultProvider ?? (() => false),
            description,
            null);
        
        AddSymbol(symbol);
        TryAddValidator(symbol, memberExpression, validation);
        
        return this;
    }
    
    /// <summary>
    /// Adds a sub command to this instance.
    /// </summary>
    /// <param name="name">Name the command is identified by.</param>
    /// <param name="description">A description of the command.</param>
    /// <typeparam name="TChildModel">Model type.</typeparam>
    /// <returns>A reference to a new <see cref="CliCommand"/> instance.</returns>
    public CliCommand<TChildModel> AddSubCommand<TChildModel>(string name, string? description = null)
        where TChildModel : class, TModel
    {
        return AddSubCommand<TChildModel>([name], description);
    }

    /// <summary>
    /// Adds a sub command to this instance.
    /// </summary>
    /// <param name="names">Name or names the command is identified by.</param>
    /// <param name="description">A description of the command.</param>
    /// <typeparam name="TChildModel">Model type.</typeparam>
    /// <returns>A reference to a new <see cref="CliCommand"/> instance.</returns>
    public CliCommand<TChildModel> AddSubCommand<TChildModel>(string[] names, string? description = null)
        where TChildModel : class, TModel
    {
        var subCommand = new CliCommand<TChildModel>(SymbolId.Next(), names, description, SymbolId, parent: this);
        base.AddSubCommand(subCommand);
        
        return subCommand;
    }
    
    /// <summary>
    /// Adds an action for this command.
    /// </summary>
    /// <param name="names">Name or names the action is identified by on the command line.</param>
    /// <param name="handler">A function that performs the action and returns a result.</param>
    /// <param name="scope">Scope of the action.</param>
    /// <param name="description">Description of the action.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> AddAction(
        string[] names,
        Action handler,
        CliScope scope = CliScope.Self,
        string? description = null)
    {
        Guard.IsNotNull(names);
        Guard.IsNotEmpty(names);
        Guard.IsNotNull(handler);

        AddModelessTask(new ActionTaskConfiguration(
            SymbolId.Next(),
            names,
            description, 
            scope,
            (_, _) =>
            {
                handler();
                return Task.FromResult(0);
            }));

        return this;
    }
    
    /// <summary>
    /// Adds an action for this command.
    /// </summary>
    /// <param name="names">Name or names the action is identified by on the command line.</param>
    /// <param name="handler">A function that performs the action and returns a result.</param>
    /// <param name="scope">Scope of the action.</param>
    /// <param name="description">Description of the action.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> AddAction(
        string[] names,
        Func<int> handler,
        CliScope scope = CliScope.Self,
        string? description = null)
    {
        Guard.IsNotNull(names);
        Guard.IsNotEmpty(names);
        Guard.IsNotNull(handler);

        AddModelessTask(new ActionTaskConfiguration(
            SymbolId.Next(),
            names,
            description,
            scope,
            (_, _) => Task.FromResult(handler())));

        return this;
    }

    /// <summary>
    /// Adds an action for this command.
    /// </summary>
    /// <param name="names">Names the action is identified by on the command line.</param>
    /// <param name="handler">A function that performs the action and returns a result.</param>
    /// <param name="scope">Scope of the action.</param>
    /// <param name="description">Description of the action.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> AddAction(
        string[] names,
        Func<CliCommand, CliOptions, int> handler,
        CliScope scope = CliScope.Self,
        string? description = null)
    {
        Guard.IsNotNull(names);
        Guard.IsNotEmpty(names);
        Guard.IsNotNull(handler);
        
        AddModelessTask(new ActionTaskConfiguration(
            SymbolId.Next(),
            names,
            description,
            scope,
            (command, options) => Task.FromResult(handler(command, options))));

        return this;
    }

    /// <summary>
    /// Establishes the function that implements the logic of the command.
    /// </summary>
    /// <param name="handler">Function that accepts the model.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> Handle(Action<TModel> handler)
    {
        return HandleAsync(async (model, _) =>
        {
            await Task.CompletedTask;
            handler(model);
            return 0;
        });
    }
    
    /// <summary>
    /// Establishes the function that implements the logic of the command.
    /// </summary>
    /// <param name="handler">Function that accepts the model and returns a result.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> Handle(Func<TModel, int> handler)
    {
        return HandleAsync(async (model, _) =>
        {
            await Task.CompletedTask;
            return handler(model);
        });
    }
    
    /// <summary>
    /// Establishes the function that implements the logic of the command asynchronously.
    /// </summary>
    /// <param name="handler">Function that accepts the model and returns a <see cref="Task"/>.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> HandleAsync(Func<TModel, CancellationToken, Task> handler)
    {
        return HandleAsync(async (model, cancelToken) =>
        {
            await handler(model, cancelToken);
            return 0;
        });
    }

    /// <summary>
    /// Establishes the function that implements the logic of the command asynchronously.
    /// </summary>
    /// <param name="handler">Function that accepts the model and returns the result.</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel> HandleAsync(Func<TModel, CancellationToken, Task<int>> handler)
    {
        Handler = handler;
        return this;
    }

    private void TryAddValidator<TValue>(
        CliSymbol symbol,
        Expression<Func<TModel, TValue>> memberExpression,
        Action<ValidationBuilder<TModel, TValue>>? configure)
    {
        if (configure == null)
            return;

        var builder = new ValidationBuilder<TModel, TValue>();
        configure(builder);
        _validator.AddEvaluation(symbol, memberExpression, builder.Build());
    }

    /// <inheritdoc />
    internal override void ValidateModel(object model, ValidationContext context)
    {
        _validator.Validate((TModel)model, context);
    }

    private Task<int> ThrowingHandler(TModel _, CancellationToken __)
    {
        throw new NotImplementedException($"Handler for command '{PrimaryIdentifier}' is not implemented");
    }
}