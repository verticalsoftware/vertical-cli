# vertical-cli

This library is used in console applications to bind string arguments to strongly typed models. The application is responsible for defining the models, the command or commands the application supports, the option and argument symbols, and the command logic implementation.

## Lexicon

### Commands

A command represents a distinct function of the application. A `vertical-cli` application must define a command in order to integrate parsing. The command defines the model type the framework binds to, the options and arguments, and a function that implements the application's logic.

Applications can define a hierarchy, where commands can have nested commands. The easiest way to illustrate this is to look at an example of .net CLI tooling:

```shell
$ dotnet nuget push ./lib/vertical-cli.1.0.0.nupkg
```

In this example, a command structure would be:
- `dotnet` is defined as the root command (the application)
- `nuget` is a subcommand of `dotnet`
- `push` is a subcommand of `nuget`

### Symbols

Command define symbols that instruct the parser what arguments get to bind to what model properties. Conceptually, the parser can identify the following command line syntaxes:

- Posix options: A single dash followed by one or more single character identifiers (e.g. `-a`, `-abc`). Multiple characters are expanded into their own symbols, for example `-abc` would be treated as `-a -b -c`.
- GNU long-form options: A double dash followed by an identifier (e.g. `--user-id`).
- Options with operand values: Posix or GNU option followed by `:` or `=` and a value (e.g. `-u:admin`, `--user-id=admin`).
- Positional arguments: Arguments not associated with an option. Context is inferred by their position.
- Operand arguments: Arguments associated with an option. Context is inferred by their position after the option symbol.
- Option terminator: A double dash (`--`) that instructs the parser that the values that follow are not option symbols.

### Arity & Scope

Symbol definitions define arity and scope. Arity refers to the minimum required and maximum allowed values for a symbol.
- Switches are automatically assigned an arity of `(1,1)` with a default value of false.
- Options can be variadic. Multiple values can be provided by repeating the syntax, e.g. `archive --in ./src --in ./test`.
- The last positional argument can be variadic.

Commands can propagate symbol definitions to subcommands by specifying a scope, which can one of:
- `Self` - The symbol applies only to the command where it was defined
- `SelfAndDescendents` - The symbol applies to the command where it was defined and any subcommands
- `Descendents` - The symbol applies only to subcommands. This is useful when the command itself performs no function.

## Configuration

### Basic

The following example demonstrates setup of a fictitious compression tool called `arc`. It demonstrates most of the capability within the setup. Example usage as an application would be:

```shell
> arc create --compression zip ---check -out ./archive.arc ./**/*.mov
> arc extract --check --out ./unzipped ./archive.arc
```

```csharp
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Validation;

// Defines the root command (aka the app)
var root = new RootCommand<Options>("arc", "An archiving utility.");
root
    // Adds a switch [--help, ?, -?] that triggers the help system
    .AddHelpSwitch()

    // Only applies to sub-commands
    .AddOption(x => x.Compression,
        names: ["--compression"],
        scope: CliScope.Descendants,
        defaultProvider: () => Compression.Zip,
        description: "Compression type to use (None, AutoDetect, Zip, Deflate)"); 
    
// Add the create mode command
root
    .AddSubCommand<CreateOptions>("create", "Creates a new archive.")
    .AddSwitch(x => x.ComputeChecksum,
        names: ["--check", "--compute-checksum"],
        description: "Compute and display the checksum on the output file.")
    .AddOption(x => x.OutputFile,
        names: ["-o", "--out"],
        description: "Path/name of the output file.",
        arity: Arity.One, // require value
        validation: value => value.DoesNotExist()) // don't overwrite existing file
    .AddArgument(x => x.InputFiles,
        Arity.OneOrMany,
        description: "One or more input files or glob patterns to add to the archive")
    .HandleAsync(async (options, cancellationToken) =>
    {
        // Create the archive...
        await Task.CompletedTask;
    });

// Add the extract mode command
root
    .AddSubCommand<ExtractOptions>("extract", "Extracts an archive.")
    .AddOption(x => x.Checksum,
        names: ["--check"],
        description: "Validates the archive's checksum")
    .AddArgument(x => x.InputFile,
        arity: Arity.One,
        description: "Path to the input file",
        validation: value => value.Exists())
    .AddOption(x => x.OutputDirectory,
        names: ["-o", "--out"],
        defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
        description: "Path to the output directory")
    .HandleAsync(async (options, cancellationToken) =>
    {
        // Extract the archive
        await Task.CompletedTask;
    });

try
{
    return await root.InvokeAsync(args);
}
catch (CommandLineException exception)
{
    Console.WriteLine(exception.Message);
    return -1;
}

// Models
enum Compression
{
    None,
    AutoDetect,
    Zip,
    Deflate
}

abstract class Options
{
    public Compression Compression { get; set; }
}

class CreateOptions : Options
{
    public bool ComputeChecksum { get; set; }
    public FileInfo OutputFile { get; set; } = default!;
    public IEnumerable<FileInfo> InputFiles { get; set; } = default!;
}

class ExtractOptions : Options
{
    public string? Checksum { get; set; }
    public FileInfo InputFile { get; set; } = default!;
    public DirectoryInfo OutputDirectory { get; set; } = default!;
}
```

### Handlers

Commands that have symbols that are scoped `Self` or `SelfAndDescendents`, or inherit any commands from its parent must implement a handler. Implementations are defined by using `command.Handle` and `command.HandleAsync`. These implementations are functions that receive the model and perform the application's logic. 

### Value Conversion

The library must convert `string` arguments to types exposed by models. Out-of-box, the following types are supported:

- Any type that implements `IParseable<TSelf>`, and for value types, `Nullable<T>` variants. This covers most integral types in the `System` namespace.
- `string`
- `FileInfo` and `DirectoryInfo`
- `Uri`

Binding is also supported for any collection type or interface in the `System.Collections.Generic` and `System.Collections.Immutable` namespace (except for dictionary types).

The easiest way to have the parser bind to a custom type is to implement `IParseable<TSelf>`. If that is not an option, a custom converter can be added to options.

```csharp
readonly record struct Point(int X, int Y);

var rootCommand = new RootCommand<Model>("program")
    .ConfigureOptions(options => options.ValueConverters.Add(str => {
        var match = Regex.Match(str, @"^(\d+),(\d+)$");
        return new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
    }));
```

### Custom Validation

Many standard validations are built into the library, but if custom validation is required, a contextual evaluation can be performed by configuring a symbol's validator.

```csharp
var rootCommand = new RootCOmmand<Model>("program")
    // Assume model.Source and x.Dest are both FileInfo
    .AddOption(x => x.Dest,
        validation: value.Must(
            (model, dest) => model.Source.FullName != dest.FullName,
            "Source and destination file paths cannot be the same."));
```

### Built-in Help System

Application's can be configured to display help by adding a help switch to the root command. By default, the framework will display help for the identified command. Applications can provide customized content by implementing `IHelpProvider` and replacing the default instance in the root options object. Help can also be manually invoked at any time by calling `DisplayHelp` on any `CliCommand` instance.

### Handling Client Errors

The parser will throw `CommandLineException` for invalid client input. The following produce errors:
- Value type conversion fails
- A symbol's arity requirement is not met
- Validation of a value fails one or more configured rules
- An argument could not be mapped to a configured symbol

The exception object provides the error type, message, command, and symbol (if available).