using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a parameter collection, with efficient retrieval by binding name or parameter
/// identifier.
/// </summary>
public sealed class ParameterCollection : IReadOnlyCollection<CliParameter>
{
    private readonly IReadOnlyDictionary<string, CliParameter> identifierDictionary;
    private readonly IReadOnlyDictionary<string, CliParameter> bindingDictionary;

    private ParameterCollection(
        IReadOnlyDictionary<string, CliParameter> identifierDictionary, 
        IReadOnlyDictionary<string, CliParameter> bindingDictionary)
    {
        this.identifierDictionary = identifierDictionary;
        this.bindingDictionary = bindingDictionary;
    }

    internal static ParameterCollection Create(CliApplication application, RouteTarget routeTarget)
    {
        var parameters = GetModelTypeParameters(application, routeTarget.Route.ModelType);
        var identifierDictionary = CreateIdentifierDictionary(parameters);
        var bindingDictionary = parameters.ToDictionary(parameter => parameter.BindingName);

        return new ParameterCollection(identifierDictionary, bindingDictionary);
    }

    /// <summary>
    /// Tries to get a parameter by identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the parameter.</param>
    /// <param name="parameter">When the method returns and the parameter was located, a reference to
    /// the <see cref="CliParameter"/></param>
    /// <returns><c>true</c> if the identifier was contained in the collection</returns>
    public bool TryGetByIdentifier(string identifier, [NotNullWhen(true)] out CliParameter? parameter)
    {
        return identifierDictionary.TryGetValue(identifier, out parameter);
    }

    /// <summary>
    /// Tries to get a parameter by identifier.
    /// </summary>
    /// <param name="binding">The name of the binding.</param>
    /// <param name="parameter">When the method returns and the parameter was located, a reference to
    /// the <see cref="CliParameter"/></param>
    /// <returns><c>true</c> if the identifier was contained in the collection</returns>
    public bool TryGetByBinding(string binding, [NotNullWhen(true)] out CliParameter? parameter)
    {
        return bindingDictionary.TryGetValue(binding, out parameter);
    }

    /// <inheritdoc />
    public int Count => bindingDictionary.Count;

    /// <inheritdoc />
    public IEnumerator<CliParameter> GetEnumerator() => bindingDictionary.Values.GetEnumerator();

    private static CliParameter[] GetModelTypeParameters(CliApplication application, Type modelType)
    {
        var dictionary = new Dictionary<string, CliParameter>();
        var configurations = application.ModelConfigurations;
        
        foreach (var type in modelType.GetTypeFamily().Reverse())
        {
            if (!configurations.TryGetValue(type, out var configuration))
                continue;
            
            dictionary.ReplaceRange(configuration.Parameters, parameter => parameter.BindingName);
        }

        return dictionary.Values.ToArray();
    }

    private static Dictionary<string, CliParameter> CreateIdentifierDictionary(IEnumerable<CliParameter> parameters)
    {
        return new Dictionary<string, CliParameter>(parameters
            .SelectMany(parameter => parameter
                .Identifiers
                .Select(identifier => new KeyValuePair<string, CliParameter>(identifier.Text, parameter)))
        );
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}