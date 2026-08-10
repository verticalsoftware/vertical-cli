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

    private record CallSiteInfo(CallSiteFactory SiteFactory, Type ModelType, bool RequiresServices);
    private readonly List<Command> _subCommands = [];
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

    internal bool RequiresServices => true == _callSiteInfo?.RequiresServices;

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
    /// Registers a method that performs the application function when control flow is routed
    /// to this command.
    /// </summary>
    /// <param name="handler">
    /// A method that performs the application function and returns a result code.
    /// </param>
    /// <typeparam name="TModel">Model type the handler expects.</typeparam>
    public void SetHandler<TModel>(Func<TModel, CancellationToken, Task<int>> handler) where TModel : class
    {
        _callSiteInfo = new CallSiteInfo(
            (context, tokenList) => CallSite.Create(
                context,
                () => HandlerServiceContext.Create(context, handler), tokenList),
            typeof(TModel),
            RequiresServices: false);
    }

    /// <summary>
    /// Registers a method that provides the instance of <see cref="IHandler{TModel}"/> that will perform
    /// the application function when control flow is routed to this command. 
    /// </summary>
    /// <param name="serviceResolver">
    /// A method that provides the command handler instance.
    /// </param>
    /// <typeparam name="TModel">Model type</typeparam>
    public void SetHandler<TModel>(Func<IServiceProvider?, IHandler<TModel>> serviceResolver) where TModel : class
    {
        _callSiteInfo = new CallSiteInfo(
            (context, tokenList) => CallSite.Create(
                context,
                () => HandlerServiceContext.Create(context, serviceResolver), tokenList),
            typeof(TModel),
            RequiresServices: false);
    }

    /// <summary>
    /// Registers a method that seeks an instance of <see cref="IHandler{TModel}"/> from the application's
    /// service provider that will perform the application function when control flow is routed to this
    /// command. 
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="THandler">The handler implementation type</typeparam>
    public void SetHandler<TModel, THandler>() 
        where TModel : class 
        where THandler : class, IHandler<TModel>
    {
        _callSiteInfo = new CallSiteInfo(
            (context, tokenList) => CallSite.Create(
                context,
                () => HandlerServiceContext.Create<TModel, THandler>(context), tokenList),
            typeof(TModel),
            RequiresServices: true);
    }
}