using System.Collections.Immutable;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.SourceGenerator.Tests;

[GeneratedBinding]
public interface IModel
{
    public string String { get; }
    public int Int { get; }
    public int? NullableInt { get; }
    public ConsoleKey Key { get; }
    public ConsoleKey? NullableKey { get; }
    public FileInfo File { get; }
    public DirectoryInfo Directory { get; }
    public Uri Link { get; }
    public string[] Array { get; }
    public List<string> List { get; }
    public LinkedList<string> LinkedList { get; }
    public HashSet<string> Set { get; }
    public SortedSet<string> SortedSet { get; }
    public IEnumerable<string> Enumerable { get; }
    public ICollection<string> Collection { get; }
    public IReadOnlyCollection<string> ReadOnlyCollection { get; }
    public IList<string> ListInterface { get; }
    public IReadOnlyList<string> ReadOnlyListInterface { get; }
    public ISet<string> SetInterface { get; }
    public IReadOnlySet<string> ReadOnlySetInterface { get; }
    public ImmutableArray<string> ImmutableArray { get; }
    public ImmutableList<string> ImmutableList { get; }
    public ImmutableHashSet<string> ImmutableHashSet { get; }
    public ImmutableSortedSet<string> ImmutableSortedSet { get; }
    public ImmutableStack<string> ImmutableStack { get; }
    public ImmutableQueue<string> ImmutableQueue { get; }
    public Stack<string> Stack { get; }
    public Queue<string> Queue { get; }
}

public interface IParameterModel
{
    public bool Switch { get; }
}

public static class TestClass
{
    public static void Configure()
    {
        var command = new RootCommand("app");
        
        command.SetHandler<IParameterModel>((
            [GeneratedBinding] IParameterModel options,
            CancellationToken CancellationToken) => Task.FromResult(0));
    }
}