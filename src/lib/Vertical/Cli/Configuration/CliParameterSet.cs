using System.Collections;

namespace Vertical.Cli.Configuration;

internal sealed class CliParameterSet : IEnumerable<CliParameter>
{
    private readonly Dictionary<string, CliParameter> parameters = new();

    public bool TryAdd(CliParameter parameter)
    {
        if (!parameter.Identifiers.All(id => parameters.TryAdd(id.Text, parameter)))
            return false;

        Count++;
        return true;
    }

    public CliParameter this[CliParameter other]
    {
        get
        {
            foreach (var identifier in other.Identifiers)
            {
                if (parameters.TryGetValue(identifier.Text, out var existing))
                    return existing;
            }

            throw new KeyNotFoundException();
        }
    }
    
    public int Count { get; private set; }

    /// <inheritdoc />
    public IEnumerator<CliParameter> GetEnumerator() => parameters.Values.Distinct().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}