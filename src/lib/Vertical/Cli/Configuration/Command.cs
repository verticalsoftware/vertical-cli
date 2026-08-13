using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an implementation of an application function or a pathway to one or more subcommands (or both).
/// </summary>
public abstract class Command : IHelpSubject
{
    private delegate Task<int> CallSiteFactory(InvocationContext context, ITokenList tokenList);

    private record CallSiteInfo(CallSiteFactory SiteFactory, Type ModelType);
    private readonly List<Command> _subCommands = [];
    private readonly List<UnboundCommandSymbol> _definedSymbols = [];
    private CallSiteInfo? _callSiteInfo;

    internal Command(string name, CommandHelpTopic? helpTopic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Name = name;
        HelpTopic = helpTopic;
    }

    internal Task<int> CreateCallSite(InvocationContext context, ITokenList tokenList)
    {
        return _callSiteInfo is null
            ? throw Exceptions.CallSiteNotSupported(this)
            : _callSiteInfo.SiteFactory(context, tokenList);
    }

    /// <summary>
    /// Gets the model type for the set handler.
    /// </summary>
    public Type? ModelType => _callSiteInfo?.ModelType;

    /// <summary>
    /// Gets whether a call site can be created by this command.
    /// </summary>
    public bool CanCreateCallSite => _callSiteInfo is not null;

    /// <summary>
    /// Adds a sub command.
    /// </summary>
    /// <param name="command">The sub command instance ot add.</param>
    /// <exception cref="InvalidOperationException">The command is already rooted by a parent.</exception>
    public void AddSubCommand(SubCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        if (command.Parent is not null)
        {
            throw Exceptions.CommandAlreadyParented(command);
        }
        
        _subCommands.Add(command);
        command.Parent = this;
    }

    /// <summary>
    /// Gets the unbound symbols defined for this instance.
    /// </summary>
    public IReadOnlyCollection<UnboundCommandSymbol> DefinedSymbols => _definedSymbols;

    /// <summary>
    /// Gets global scoped unbound symbols inherited by this instance.
    /// </summary>
    public IEnumerable<UnboundCommandSymbol> InheritedSymbols => GetAncestors()
        .SelectMany(command => command.DefinedSymbols)
        .Where(symbol => symbol.Scope == UnboundScope.Global);

    /// <summary>
    /// Gets all symbols defined by this instance or inherited from parent commands.
    /// </summary>
    public IEnumerable<UnboundCommandSymbol> Symbols => DefinedSymbols
        .Concat(InheritedSymbols);

    /// <summary>
    /// Gets the sub commands of this instance.
    /// </summary>
    public IReadOnlyCollection<Command> SubCommands => _subCommands;

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the help topic.
    /// </summary>
    public CommandHelpTopic? HelpTopic { get; }

    /// <inheritdoc />
    public string? GetRemarks() => HelpTopic?.Remarks;

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => HelpTopic?.ExtendedRemarks ?? [];

    /// <inheritdoc />
    public string GetListIdentifier() => Name;

    /// <inheritdoc />
    public string? GetParameterName() => null;

    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;
    
    /// <summary>
    /// Gets the parent to this instance.
    /// </summary>
    public Command? Parent { get; private set; }

    /// <summary>
    /// Gets a string that represents the path to this command.
    /// </summary>
    public string Path => string.Join(" ", GetAncestorsAndSelf().Select(command => command.Name));

    /// <summary>
    /// Enumerates commands beginning with root and ending with the parent of this instance.
    /// </summary>
    /// <returns><see cref="IEnumerable{Command}"/></returns>
    public IEnumerable<Command> GetAncestors() => GetAncestorsAndSelf().SkipLast(1);

    /// <inheritdoc />
    public override string ToString() => $"{Path} ({(CanCreateCallSite ? "call site" : "abstract")})";

    /// <summary>
    /// Enumerates commands beginning with root and ending with this instance.
    /// </summary>
    /// <returns><see cref="IEnumerable{Command}"/></returns>
    public IEnumerable<Command> GetAncestorsAndSelf()
    {
        return EnumerateBackward().Reverse();
        
        IEnumerable<Command> EnumerateBackward()
        {
            for (var command = this; command != null; command = command.Parent)
            {
                yield return command;
            }    
        }
    }

    /// <summary>
    /// Adds an unbound option symbol.
    /// </summary>
    /// <param name="identifier">The identifier of the symbol relevant to help providers.</param>
    /// <param name="aliases">Aliases to associate to the option.</param>
    /// <param name="scope">The scope of the symbol.</param>
    /// <param name="handler">An asynchronous method that reacts when the symbol is specified.</param>
    /// <param name="helpTopic">The help topic to associate with the symbol</param>
    public void AddUnboundOption(
        string identifier,
        AliasList aliases,
        UnboundScope scope,
        Func<InvocationContext, Command, Task> handler,
        HelpTopic? helpTopic = null)
    {
        ArgumentNullException.ThrowIfNull(handler);
        
        var symbol = new UnboundCommandSymbol(identifier, aliases, scope, handler, helpTopic);
        _definedSymbols.Add(symbol);
    }

    /// <summary>
    /// Registers a method that performs the application function when control flow is routed
    /// to this command.
    /// </summary>
    /// <param name="handler">
    /// A method that performs the application function and returns a result code.
    /// </param>
    /// <typeparam name="TModel">Model type the handler expects.</typeparam>
    public void SetHandler<TModel>(Func<TModel, CancellationToken, Task<int>> handler) where TModel : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        
        _callSiteInfo = new CallSiteInfo(
            (context, tokenList) => CallSite.Create(
                context,
                () => new HandlerServiceProvider<TModel>(() => new DelegatedHandler<TModel>(handler)),
                tokenList),
            typeof(TModel));
    }

    /// <summary>
    /// Registers a function that provides the <see cref="IHandler{TModel}"/> instance to use for
    /// servicing the command.
    /// </summary>
    /// <param name="handlerFactory">
    /// A function that returns a handler instance.
    /// </param>
    /// <typeparam name="TModel">Model type</typeparam>
    public void SetHandler<TModel>(Func<InvocationContext, IHandler<TModel>> handlerFactory) where TModel : class
    {
        ArgumentNullException.ThrowIfNull(handlerFactory);
        
        _callSiteInfo = new CallSiteInfo(
            (context, tokenList) => CallSite.Create(
                context,
                () => new HandlerServiceProvider<TModel>(() => handlerFactory(context)),
                tokenList),
            typeof(TModel));
    }

    /// <summary>
    /// Registers a function that provides the <see cref="HandlerServiceProvider{TModel}"/> to use when
    /// the call site requests the handler instance.
    /// </summary>
    /// <param name="providerFactory">A function that returns the provider.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    public void SetHandlerProvider<TModel>(Func<InvocationContext, HandlerServiceProvider<TModel>> providerFactory)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(providerFactory);

        _callSiteInfo = new CallSiteInfo(
            (context, tokenList) => CallSite.Create(
                context,
                () => providerFactory(context),
                tokenList),
            typeof(TModel));
    }
}