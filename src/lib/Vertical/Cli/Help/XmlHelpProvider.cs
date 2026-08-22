using System.Text.RegularExpressions;
using System.Xml.XPath;
using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Implements a <see cref="IHelpProvider"/> using an xml resource.
/// </summary>
public sealed class XmlHelpProvider : IHelpProvider
{
    private readonly Lazy<(XPathDocument Document, XPathNavigator Navigator)> _lazyResources;
    private readonly Lazy<(string Key, string Value)[]> _lazyIdPrefixes;
    
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

        _lazyIdPrefixes = new Lazy<(string Key, string Value)[]>(() =>
        {
            return Navigator
                .Select("/help/prefixes/prefix")
                .Cast<XPathNavigator>()
                .Select(node => (
                    key: $"({node.GetAttribute("key", string.Empty)})",
                    value: node.GetAttribute("value", string.Empty)
                ))
                .Where(t => t is { key.Length: > 2, value.Length: > 0 })
                .ToArray();
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
            _ => GetNode(subject)?.Value
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
        return subject is CliSymbol { Kind: SymbolKind.PositionArgument } symbol
               && GetAttribute(symbol, ParameterNameAttribute) is { } identifier
            ? identifier
            : subject.GetListIdentifier();
    }

    /// <inheritdoc />
    public string? GetParameterName(ICliSymbol subject)
    {
        return subject is { Kind: SymbolKind.Option or SymbolKind.Directive }
               && GetAttribute(subject, ParameterNameAttribute) is { } parameterName
            ? parameterName
            : subject.GetParameterName();
    }

    private XPathNavigator? GetNode(IHelpSubject subject)
    {
        var key =  subject.HelpTopicKey;
        var id = CreateLookupId(key);
        var path = $"/help/topic[@type='{key.TypeId}' and @id='{id}']";
        return Navigator.SelectSingleNode(path);
    }

    private string CreateLookupId(HelpTopicKey subject)
    {
        return _lazyIdPrefixes
            .Value
            .Aggregate(
                subject.TopicId,
                (id, prefix) => id.Replace(prefix.Key, prefix.Value));
    }

    private string? GetAttribute(IHelpSubject subject, string attributeName)
    {
        var node = GetNode(subject);
        var attribute = node?.GetAttribute(attributeName, namespaceURI: string.Empty);
        return attribute.NonWhiteSpaceOrNull();
    }
}