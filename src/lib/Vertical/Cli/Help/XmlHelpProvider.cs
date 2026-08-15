using System.Xml.XPath;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Implements a <see cref="IHelpProvider"/> using an xml resource.
/// </summary>
public sealed class XmlHelpProvider : IHelpProvider
{
    private readonly Lazy<(XPathDocument Document, XPathNavigator Navigator)> _lazyResources;
    
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
            Command command => GetNode(command)?.SelectSingleNode("remarks")?.Value,
            CliSymbol symbol => GetNode(symbol)?.Value,
            IDirectiveSymbol directive => GetNode(directive)?.Value,
            UnboundSymbol unbound => GetNode(unbound)?.Value,
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
            var sectionNodesIterator = GetNode(command)?
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
                GetNode(argument)?.GetAttribute(ParameterNameAttribute, string.Empty)
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
                GetNode(option)?.GetAttribute(ParameterNameAttribute, namespaceURI: string.Empty)
                ?? option.GetParameterName(),
            
            _ => subject.GetParameterName()
        };
    }

    private XPathNavigator? GetNode(IHelpSubject subject)
    {
        var key = subject.HelpTopicKey;
        var path = $"/help/topic[@type='{key.TypeId}' and @id='{key.Topic}']";
        return Navigator.SelectSingleNode(path);
    }
}