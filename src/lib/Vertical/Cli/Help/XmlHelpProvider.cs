using System.Xml.XPath;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Implements a <see cref="IHelpProvider"/> using an xml resource.
/// </summary>
public sealed class XmlHelpProvider : IHelpProvider
{
    private readonly Lazy<(XPathDocument Document, XPathNavigator Navigator)> _lazyResources;
    
    private const string CommandTypeName = "command";
    private const string SymbolTypeName = "symbol";
    private const string DirectiveTypeName = "directive";
    private const string ParameterNameAttribute = "parameter-name";

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlHelpProvider"/> class.
    /// </summary>
    /// <param name="resourceStreamProvider">
    /// A function invoked by the help system that returns the xml content stream.
    /// </param>
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
            Command command => GetCommandNode(command)?.SelectSingleNode("remarks")?.Value,
            CliSymbol symbol => GetSymbolNode(symbol)?.Value,
            IDirectiveSymbol directive => GetDirectiveNode(directive)?.Value,
            UnboundSymbol unbound => GetUnboundSymbolNode(unbound)?.Value,
            _ => null
        } ?? subject.GetRemarks();
    }

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarks(Command command)
    {
        var result = GetResult().ToArray();
        return result.Length > 0 ? result : command.GetExtendedRemarksSections();
        
        IEnumerable<ExtendedRemarksSection> GetResult()
        {
            var sectionNodesIterator = GetCommandNode(command)?
                .SelectSingleNode("sections")?
                .SelectChildren(XPathNodeType.Element);

            if (sectionNodesIterator is null)
                yield break;

            while (sectionNodesIterator.MoveNext())
            {
                var current = sectionNodesIterator.Current;
                if (current is null)
                    continue;

                var title = current.GetAttribute("title", string.Empty);
                var remarks = current.Value;

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(remarks))
                    continue;

                yield return new ExtendedRemarksSection(title, remarks);
            }
        }
    }

    /// <inheritdoc />
    public string GetIdentifier(IHelpSubject subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } argument  => 
                GetSymbolNode(argument)?.GetAttribute(ParameterNameAttribute, string.Empty)
                ??  argument.GetListIdentifier(),
            
            _ => subject.GetListIdentifier()
        };
    }

    /// <inheritdoc />
    public string? GetParameterName(ICliSymbol subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.Option } option =>
                GetSymbolNode(option)?.GetAttribute(ParameterNameAttribute, namespaceURI: string.Empty)
                ?? option.GetParameterName(),
            
            _ => subject.GetParameterName()
        };
    }

    private XPathNavigator? GetNode(string type, string id)
    {
        var path = $"/help/topic[@type='{type}' and @id='{id}']";
        return Navigator.SelectSingleNode(path);
    }

    private XPathNavigator? GetCommandNode(Command command) => 
        GetNode(CommandTypeName, command.Path);
    
    private XPathNavigator? GetSymbolNode(CliSymbol symbol) => 
        GetNode(SymbolTypeName, $"{symbol.ModelType.FullName}.{symbol.BindingName}");
    
    private XPathNavigator? GetUnboundSymbolNode(UnboundSymbol symbol) => 
        GetNode(SymbolTypeName, $"(Unbound).{symbol.Identifier}");

    private XPathNavigator? GetDirectiveNode(IDirectiveSymbol directive) => 
        GetNode(SymbolTypeName, $"(Directive).{directive.Identifier}");
}