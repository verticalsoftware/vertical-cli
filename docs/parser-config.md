# Configuring the parser

### Overview

Configuring the parser is the processing of informing it what arguments and options are available to the user of the application, and what properties of the options model the parsed values are mapped to.

### Conventions

The argument parser understands the following conventions:

- _Options_ are short or long form GNU style identifiers that associate with a parameter value. A parameter can be concatenated to the symbol itself using `:` or `=`, or it can follow as a separate argument. Examples: `-u`, `--user`, `--user-id`, `--user-id:sa`.
- _Switches_ are options that are always represented as the value `true`.
- _Option/switch groups_ are syntatic shortcuts for users where multiple switches and a single option can be combined into one token. For example, `-abc:red` is treated equivalently as input `-a -b -c red`.
- _Position arguments_ are parameter values whose semantic meaning is inferred by their positions in the input.
- _Variadic_ arguments are position arguments that can be repeated one or more times. An application can only define one variadic position argument at most for each unique command path. When used with other position arguments, it must be in the last parsing position.
- _Directives_ are symbols that invoke applcation functions outside of the command/model framework. The parser recognizes the following pattern for directives: `\[(?<identifier>\w+)([:=](?<parameter>.+))?\]`.
- _Annotations_ support response files. When a file path is preceded by the `@` character, the tokens in the file are injected into the input.

### Configuration

Options, switches, and position arguments are configured by pairing each with a model property and specifying the following characterstics:
- _Aliases_ identify options and switches and GNU short/long form identifiers. If omitted, the binding property's name in lower/kebab cased form is used (`UserId` becomes `--user-id`).
- _Ordinal position_ determines what order the parser evaluates position arguments (required).
- _Default values_ are used in the absence of user input. Provide a default for optional symbols that map to nullable types (or expected `default(T)!` as the mapped value).
- _Required_ indicates an argument value must be provided, whether by user input or a default value.
- _Arity_ defines the expected and allowed number of arguments for a multi-valued symbol. An arity with no maximum count is variadic.
- Input values can be _validated_ using contextual evaluation.
- A _help topic_ provides information to the user for the option or argument.

The following example illustrates configuring the parser for a command model type:

```csharp
// Commmand model:
[GeneratedBinding]
interface IUploadOptions
{
    string UserId { get; }
    string Password { get; }
    bool UseBrowser { get; }
    string[] FilePaths { get; }
}

// Parser configuration:
var app = new CommandLineApplication(rootCommand);

app.ConfigureParser<IUploadOptions>(parser => parser
    .ParseOption(
        expression: x => x.UserId,
        aliases: ["-u", "--user-id"],
        required: true,
        helpTopic: "User ID that has access to the storage account")
    .ParseOption(
        expression: x => x.Password,
        aliases: ["-p", "--password"],
        required: true,
        helpTopic: "Password to the account")
    .ParseSwitch(
        expression: x => x.UseBrowser,
        helpTopic: "Whether to use the browser for authorization flow.")
    .ParseRepeatableArgument(
        expression: x => x.FilePaths,
        arity: Arity.OneOrMore,
        helpTopic: "Path to one or more files to upload.",
        validate: fileInfo => fileInfo.MustExist()));
```

### Supported property types

The parser will automatically convert and set the following property types:
- Types that implement `IParsable<T>` or `IParsable<T?>`. This covers `System` primitives and their nullable value-type variants.
- Enums/nullable enums using case-insensitive matching.
- `string`, `FileInfo`, `DirectoryInfo`, and `Uri`.

Additionally, the parser can set the following multi-value property types:
- Arrays
- `List<T>`, `LinkedList<T>`, `HashSet<T>`, `SortedSet<T>`, `Stack<T>`, and `Queue<T>`
- `ImmutableArray<T>`, `ImmutableList<T>`, `ImmutableHashSet<T>`, `ImmutableSortedSet<T>`, `ImmutableStack<T>`, and `ImmutableQueue<T>`
- `IEnumerable<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IList<T>`, `IReadOnlyList<T>`, `ISet<T>`, and `IReadOnlySet<T>`

### Variadic arugment limitations

When the maxium arity of a multi-valued symbol is undefined, it is considered variadic. The parser places the following constraints on their use:
- A command model may only have one variadic symbol defined
- When multiple position arguments are used in conjunction with a variadic argument, the variadic argument must have the highest ordinal position in relation to the other arguments.

### Custom value conversion

For each model property, the parser ultimately convert a `string` to the expected type. In the case of mutli-valued properties that are backed by arrays or collections, the parser must then convert the value types into collections of value types.

For scalar value types, conversion can be implemented on a type in one of two ways:
- Implement `IParsable<T>` on the scalar type. The source generator will automatically configure the conversion service for the target type.
- When the application does not control the scalar type, register a `Converter<string, T>` delegate to the conversion service.

For collection types, conversion can be implemented by registering a `Converter<TElement, TCollection>` delegate to the conversion service where:
- `TElement` is the scalar value type.
- `TCollection` is the collection type that implements `IEnumerable<TElement>`.

In both types of implementation, applications should throw an exception if the conversion complete.

The following example demonstrates how to bind argument values to a dictionary:

```csharp
// Model
interface IOptions
{
    IReadOnlyDictionary<string, sring> Properties { get; }
}

// Configuration
app.ConfigureParser<IOptions>(parser => parser
    .AddRepeatableOption<
        KeyValuePair<string, string>,
        IReadOnlyDictionary<string, string>(
            x => x.Properties,
            aliases: "--prop",
            arity: Arity.ZeroOrMore,
            useDefault: new Dictionary<string, string>(),
            helpTopic: "A key/value pair property"));

// Convert from string argument to key/value pair
app.AddArgumentConverter(str => 
{
    if (Regex.Match(str, @"(?<key>\w+)[:=](?<value>.+") is not { Success: true } match)
        throw new ArgumentException("invalid key/value pair format.");
    
    return new KeyValuePair<string, string>(
        match.Groups["key"].Value,
        match.Groups["value"].Value);
});

// Create dictionary with key/value pairs. Note the collection type
// must match the interface's property type
app.AddCollectionConverter<
    KeyValuePair<string, string>, 
    IReadOnlyDictionary<string, string>>(keyValuePairs => new Dictionary(keyValuePairs));
```