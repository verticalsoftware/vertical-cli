using System.Xml.XPath;
using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

public sealed class XmlHelpProvider : IHelpProvider
{
    private readonly Lazy<(XPathDocument Document, XPathNavigator Navigator)> _lazyResources;
    
    private const string CommandTypeName = "command";
    private const string SymbolTypeName = "symbol";
    private const string DirectiveTypeName = "directive";
    private const string ParameterNameAttribute = "parameter-name";

    public XmlHelpProvider(Func<Stream> resourceStreamProvider)
    {
        _lazyResources = new Lazy<(XPathDocument Document, XPathNavigator Navigator)>(() =>
        {
            var document = new XPathDocument(resourceStreamProvider());
            var navigator = document.CreateNavigator();
            return (document, navigator);
        });
    }

    private XPathNavigator Navigator => _lazyResources.Value.Navigator;

    /// <inheritdoc />
    public string? GetRemarks(IHelpSubject subject)
    {
        return subject switch
        {
            Command command => GetCommandNode(command)?.Value.Trim(),
            CliSymbol symbol => GetSymbolNode(symbol)?.Value.Trim(),
            IDirectiveSymbol directive => GetDirectiveNode(directive)?.Value.Trim(),
            _ => null
        };
    }

    /// <inheritdoc />
    public int GetCommandSectionsCount(Command command)
    {
        return 0;
    }

    /// <inheritdoc />
    public string GetCommandSectionHeading(Command command, int sectionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetCommandSectionRemarks(Command command, int sectionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetListIdentifier(IHelpSubject subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } argument  => 
                GetSymbolNode(argument)?.GetAttribute(ParameterNameAttribute, string.Empty)
                ??  argument.BindingName.ToKebabCase(),
            
            CliSymbol { Kind: SymbolKind.Option or SymbolKind.Switch } named => 
                string.Join(", ", named.Aliases),
            
            IDirectiveSymbol directive => directive.Identifier,
            
            _ => throw new NotSupportedException()
        };
    }

    /// <inheritdoc />
    public string GetParameterName(IHelpSubject subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } => GetListIdentifier(subject),
            CliSymbol { Kind: SymbolKind.Option } option => GetSymbolNode(option)?.GetAttribute(ParameterNameAttribute, string.Empty)
                ?? option.BindingName.ToKebabCase(),
            CliSymbol { Kind: SymbolKind.Switch } => string.Empty,
            IDirectiveSymbol { ParameterArity: not ParameterArity.Zero } directive => 
                GetDirectiveNode(directive)?.GetAttribute(ParameterNameAttribute, string.Empty) ?? "value",
            _ => throw new NotSupportedException()
        };
    }

    private XPathNavigator? GetNode(string type, string id)
    {
        var path = $"/help/topic[@type='{type}' and @id='{id}']";
        return Navigator.SelectSingleNode(path);
    }

    private XPathNavigator? GetCommandNode(Command command) => GetNode(CommandTypeName, command.Path);

    private XPathNavigator? GetSymbolNode(CliSymbol symbol) => GetNode(SymbolTypeName, $"{symbol.ModelType.FullName}.{symbol.BindingName}");

    private XPathNavigator? GetDirectiveNode(IDirectiveSymbol directive) => GetNode(DirectiveTypeName, directive.Identifier);
}