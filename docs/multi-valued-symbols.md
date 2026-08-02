## Multi-value/variadic symbol support

### Overview

In addition to being able to bind a single value to a scalar type model property, the library supports binding multi-valued and variadic arguments to collection type properties. The source generator can bind properties to the following without additional configuration:
- Arrays
- Generic collections `List<T>`, `LinkedList<T>`, `HashSet<T>`, `SortedSet<T>`, `Stack<T>`, and `Queue<T>`
- Immutable collections `ImmutableArray<T>`, `ImmutableList<T>`, `ImmutableHashSet<T>`, `ImmutabledSortedSet<T>`, `ImmutableStack<T>`, and `ImmutableQueue<T>`
- Interfaces `IEnumerable<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IList<T>`, `IReadOnlyList<T>`, `ISet<T>`, and `IReadOnlySet<T>`

Using multi-valued arguments requires the following implementation:
- Introduce an array or collection type property to the model
- Configure the parser with the `MapMultiValuedArgument` or `MapMultiValuedOption` methods of the `ModelBuilder<TModel>` type.

The following example prints the colors specified by the user. It binds to a string array in the options model.

```csharp
var rootCommand = new RootCommand("print-colors");

rootCommand.SetHandler(([GeneratedBinding] IOptions options, CancellationToken token) => 
{
    foeach (var color in options.Colors)
    {
        Console.WriteLine(color);
    }
});

var app = new CommandLineApplication(rootCommand);

app.ConfigureParser<IOptions>(builder => builder
    .MapMultiValuedArgument(
        x => x.Colors), 
        ordinalPosition: 0, 
        arity: Arity.OneOrMore));
        
return await app.ConfigureAndRunAsync(args);

public interface IOptions
{
    string[] Colors { get; }
}
```

```shell
> dotnet run -- red green blue

# output...
red
green
blue
```

### Ordinal position of arguments

Since position arguments aren't named, their semantic meaning is inferred by their position in the client's input token list. When parsing position arguments, the ordinal position value is used to sort the argument symbols in the ascending order in which they should be parsed.

### Arity

The arity requirement of options and arguments that are bound to multi-valued properties can be set using the `Arity` structure which defines the following applicable constants and methods:

|Member|Description|
|---|---|
|`ZeroOrMore`|Don't require an occurrence; variadic maximum|
|`OneOrMore`|Require one occurrence; variadic maximum|
|`Require(count)`|Require a specific occurrence count|
|`new Arity(min, max)`|Specify both minimum and maximum occurrene constraints|


### Constraints of variadic position arguments

Since the arity of a variadic argument can have no upper limit, the following must be observed:
- A command can only define one variadic position argument
- If a variadic position argument is introduced among other position arguments, it must occupy the last ordinal position

