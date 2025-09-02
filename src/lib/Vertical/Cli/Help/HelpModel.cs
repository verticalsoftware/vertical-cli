using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Help;

/// <summary>
/// Defines a model used in building help content.
/// </summary>
public sealed class HelpModel
{
    internal HelpModel(InvocationContext context, ICommand subject)
    {
        _context = context;

        Subject = subject;
        Symbols = GetSymbols(Subject);
    }

    private readonly InvocationContext _context;
    private readonly Dictionary<ICommand, SymbolCollection> _symbolCache = [];

    /// <summary>
    /// Gets the subject command.
    /// </summary>
    public ICommand Subject { get; }
    
    /// <summary>
    /// Gets the symbols for the subject.
    /// </summary>
    public SymbolCollection Symbols { get; }
    
    /// <summary>
    /// Gets the symbols associated with the command invocation.
    /// </summary>
    /// <param name="command">Command instance.</param>
    /// <returns>List of <see cref="ISymbol"/> objects.</returns>
    public SymbolCollection GetSymbols(ICommand command)
    {
        return _symbolCache.GetOrAdd(command, () => ResolveSymbols(command));
    }

    private SymbolCollection ResolveSymbols(ICommand command)
    {
        var builder = command switch
        {
            { IsInvocationTarget: true } => command.CreateRequestBuilder(
                _context.Configuration,
                ModelConfiguration.CreateFactory(_context.Parser)),
            
            _ => _context.Configuration.ConfigureSymbolBuilder(new ContextBuilder())
        };

        var symbols = builder.Symbols;
        var coreSymbols = symbols.Where(symbol => symbol is not AncillaryOptionSymbol);
        
        return new SymbolCollection(
            ArgumentSymbols: coreSymbols
                .Where(symbol => symbol.Behavior == SymbolBehavior.Argument)
                .ToArray(),
            OptionSymbols: coreSymbols
                .Where(symbol => symbol.Behavior is SymbolBehavior.Option or SymbolBehavior.Switch)
                .ToArray(),
            AncillarySymbols: symbols
                .OfType<AncillaryOptionSymbol>()
                .ToArray(),
            builder.DirectiveHelpTags);
    }
}