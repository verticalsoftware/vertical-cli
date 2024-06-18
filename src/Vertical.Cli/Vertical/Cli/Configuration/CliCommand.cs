using System.Linq.Expressions;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Metadata;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Super class for all command types.
/// </summary>
public abstract class CliCommand : CliObject
{
    private readonly List<CliSymbol> _symbols = new(6);
    private readonly List<CliCommand> _commands = [];

    private protected CliCommand(
        Type modelType,
        string[] names,
        string? description,
        bool isActionSwitch)
        : base(names, description)
    {
        ModelType = modelType;
        IsActionSwitch = isActionSwitch;
    }

    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Gets whether this command is an action switch.
    /// </summary>
    public bool IsActionSwitch { get; }

    /// <summary>
    /// Gets the parent command, or <c>null</c> if this is the root command.
    /// </summary>
    public CliCommand? ParentCommand { get; private set; }

    /// <inheritdoc />
    public override CliObject? Parent => ParentCommand;
    
    /// <summary>
    /// Gets the symbols defined in the collection.
    /// </summary>
    public IReadOnlyCollection<CliSymbol> Symbols => _symbols;

    /// <summary>
    /// Gets the collection of child commands defined by this instance.
    /// </summary>
    public IEnumerable<CliCommand> Commands => _commands;

    /// <summary>
    /// Performs verbose checking of the configuration.
    /// </summary>
    /// <param name="messages">Message collection to populate.</param>
    internal abstract void VerifyConfiguration(ICollection<string> messages);

    internal abstract void ValidateModel(object model, ValidationContext context);
    
    /// <summary>
    /// Adds a symbol to the collection.
    /// </summary>
    /// <param name="symbol">Symbol to add.</param>
    protected void AddSymbol(CliSymbol symbol) => _symbols.Add(symbol);
    
    /// <summary>
    /// Attaches a child command.
    /// </summary>
    /// <param name="command">Command instance</param>
    protected void AttachChild(CliCommand command)
    {
        command.ParentCommand = this;
        _commands.Add(command);
    }
}

/// <summary>
/// Base class for command definitions.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class CliCommand<TResult> : CliCommand
{
    private protected CliCommand(
        Type modelType,
        string[] names,
        string? description,
        bool isActionSwitch)
        : base(modelType, names, description, isActionSwitch)
    {
    }
}

/// <summary>
/// Represents an object used to define commands.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
public partial class CliCommand<TModel, TResult> : CliCommand<TResult> where TModel : class
{
    private readonly ModelValidator<TModel> _validator = new();
    private Func<TModel, CancellationToken, TResult>? _handler;

    internal CliCommand(
        string[] names,
        string? description,
        Func<TModel, CancellationToken, TResult>? handler,
        bool isActionSwitch = false) 
        : base(typeof(TModel), names, description, isActionSwitch)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets the function that handle's the logic of the command.
    /// </summary>
    public Func<TModel, CancellationToken, TResult> Handler
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
    /// <param name="validation">
    /// An action that provides an evaluator that determines the validity of the value provided by the client.
    /// </param>
    /// <typeparam name="TValue">Value type.</typeparam>
    public CliCommand<TModel, TResult> AddArgument<TValue>(
        Expression<Func<TModel, TValue>> memberExpression,
        Arity? arity = null,
        CliScope scope = CliScope.Self,
        string? description = null,
        Action<ValidationBuilder<TModel, TValue>>? validation = null)
    {
        Guard.IsNotNull(memberExpression, nameof(memberExpression));

        var symbol = new CliArgument<TValue>(
            this,
            memberExpression.GetMemberName(),
            [],
            arity ?? Arity.One,
            scope,
            description);

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
    /// <param name="validation">
    /// An action that provides an evaluator that determines the validity of the value provided by the client.
    /// </param>
    /// <typeparam name="TValue">Value type.</typeparam>
    public CliCommand<TModel, TResult> AddOption<TValue>(
        Expression<Func<TModel, TValue>> memberExpression,
        string[] names,
        Arity? arity = null,
        CliScope scope = CliScope.Self,
        Func<TValue>? defaultProvider = null,
        string? description = null,
        Action<ValidationBuilder<TModel, TValue>>? validation = null)
    {
        Guard.IsNotNull(memberExpression, nameof(memberExpression));
        Guard.IsNotNull(names, nameof(names));
        Guard.IsNotEmpty(names, nameof(names));

        var symbol = new CliOption<TValue>(
            this,
            memberExpression.GetMemberName(),
            names,
            arity ?? Arity.ZeroOrOne,
            scope,
            defaultProvider,
            description);
        
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
    /// A <see cref="CliScope"/> that specifies the applicability of the option in the command path.
    /// </param>
    /// <param name="defaultProvider">A function that provides a default value if the client does not provide one. </param>
    /// <param name="description">A description of the command that can be displayed in help content.</param>
    /// <param name="validation">
    /// An action that provides an evaluator that determines the validity of the value provided by the client.
    /// </param>
    public CliCommand<TModel, TResult> AddSwitch(
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

        var symbol = new CliSwitch(
            this,
            memberExpression.GetMemberName(),
            names,
            scope,
            defaultProvider,
            description);
        
        AddSymbol(symbol);
        TryAddValidator(symbol, memberExpression, validation);
        
        return this;
    }

    /// <summary>
    /// Adds a sub-command to this instance's definition.
    /// </summary>
    /// <param name="command">Sub command</param>
    /// <typeparam name="TChildModel">Child model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel, TResult> AddSubCommand<TChildModel>(SubCommand<TChildModel, TResult> command)
        where TChildModel : class, TModel
    {
        AttachChild(command);
        return this;
    }

    /// <summary>
    /// Adds a switch that invokes an action and is not bound to the model (e.g. --help, --version, etc.)
    /// </summary>
    /// <param name="name">Command name.</param>
    /// <param name="handler">Function that implements the command logic.</param>
    /// <param name="description">Optional command description</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel, TResult> AddActionSwitch(
        string name,
        Func<TResult> handler,
        string? description = null) => AddActionSwitch([name], handler, description);
    
    /// <summary>
    /// Adds a switch that invokes an action and is not bound to the model (e.g. --help, --version, etc.)
    /// </summary>
    /// <param name="names">Command name or names.</param>
    /// <param name="handler">Function that implements the command logic.</param>
    /// <param name="description">Optional command description</param>
    /// <returns>A reference to this instance.</returns>
    public CliCommand<TModel, TResult> AddActionSwitch(
        string[] names,
        Func<TResult> handler,
        string? description = null)
    {
        AttachChild(new CliCommand<Empty, TResult>(
            names, 
            description, 
            (_,_) => handler(),
            isActionSwitch: true));
        return this;
    }

    /// <summary>
    /// Establishes the function that implements the logic of the command using a cancellation token.
    /// </summary>
    /// <param name="handler">Function that accepts the model and returns the result.</param>
    public CliCommand<TModel, TResult> SetHandler(Func<TModel, CancellationToken, TResult> handler)
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

    private TResult ThrowingHandler(TModel _, CancellationToken __)
    {
        throw new NotImplementedException($"Handler for command '{PrimaryIdentifier}' is not implemented");
    }
}