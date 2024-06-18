# vertical-cli

Minimal command line arguments parser.

## Quick start

### Install

```shell
dotnet add package vertical-cli --prerelease
```
### Configure and run

```csharp
using System;
using System.IO;
using Vertical.Cli.Configuration;

// Define a model that will receive the arguments
record Arguments(FileInfo Source, FileInfo Destination, bool Overwrite);

// Define the command
var command = new RootCommand<Arguments, Task>("fcopy");
command
    .AddArgument(x => x.Source)
    .AddArgument(x => x.Destination)
    .AddSwitch(x => x.Overwrite, ["-o", "--overwrite"])
    .SetHandler(async (model, cancellationToken) => 
    {
        if (model.Overwrite || model.Source.Exists())
        {
            Console.WriteLine("Desintation file exists and will not be overwritten.");
        }
        await using var source = model.Source.OpenRead();
        await using var dest = model.Destination.OpenWrite();
        await source.CopyToAsync(dest, cancellationToken);        
    });

await command.InvokeAsync(arguments);
```

## Features

- Binds command line arguments to strongly typed models
- Configure positional arguments. options and switches using posix and GNU notation
- Configure contextual parameter validation
- Define a hierarchy of commands each with distinct or inherited models
- Invoke synchronous and asynchronous workloads
- Uses a source generator to bind models removing the need for reflection (AOT friendly)
- Generates basic help content (better if descriptions are given)

## Symbol types

The library will parse the following:

- Positional arguments with no identifier when they are located before or after options.
- Posix style identifiers beginning with a single dash, e.g. `-a`, or `-abc` (same as `-a`, `-b, and `-c`).
- GNU style options beginning with a double dash, e.g. `--long-option`.
- Operands that follow option identifiers as a separate argument, or joined by `:` or `=`, e.g. `--user=admin`.
- Command names, e.g. `git commit`.

## Arity

Arguments and options can specify an arity that determines how many times they can be referenced on the command line.
Common constants include:
- `Arity.ZeroOrOne`
- `Arity.ZeroOrMany`
- `Arity.One`
- `Arity.OneOrMany`

## Setup

### Commands

Commands represent a control path in the application. At a minimum, the configuration requires a root command. Commands may define subcommands, and this is how a hierarchical structure is formed.
An example of a hierarchical command path is in the .net CLI tooling:

```shell
$ dotnet nuget push
```

### Models

Each command requires a model to bind arguments to. Models must have a public constructor. The binder will bind arguments to constructor arguments and public writeable properties. Bindings are configured in each argument, option, or switch, and use strongly typed expressions.
Models of subcommands must be a subtype of the model of the parent command. This constraint supports the notion of symbol _scopes_, and is discussed shortly.

### Result Type

All commands in an application must return the same result type from their handlers. The result type may be any value type, reference type, `Task`, or `Task<T>`. The only type that cannot be used is `void`.

### Handlers

Handlers are functions that receive the model, perform the application's logic, and return a result.

### Symbols

Symbols represent a way to associate command line arguments with their respective bindings in the model. Symbols are either positional arguments, options, and switches. Symbols have the following characteristics:
- A value type that is inferred by the model binding expression.
- An arity that describes the minimum and maximum number of values that can be bound.
- In the case of options or switches, one or more names the symbol can be referred to in command line arguments.
- A _scope_ that determines if the symbol applies to the command where it is defined, sub commands, or both.
- A function that provides a default value in case an argument is not provided (optional).
- A description that can be displayed in help content (optional).
- A set of rules that can be used to validate input values (optional).

### Example

This example simulates the setup of two .net CLI tools.

```csharp
public enum Verbosity { Verbose, Normal, Minimal }

// Base model
public abstract class BaseModel
{
    public Verbosity Verbosity { get; set; }
}

// Model for the 'build' command
public class BuildModel : BaseModel
{
    public FileInfo ProjectPath { get; set; } = default!;
    public string Configuration { get; set; } = default!;
    public DirectoryInfo OutputDirectory { get; set; } = default!;
    public bool Force { get; set; }
    public bool NoRestore { get; set; }
}

// Create the root command. The type arguments are the model type and
// the handler result type
var rootCommand = new RootCommand<BaseModel, Task>("dotnet");

rootCommand
    // Expressions are used so the binder knows what properties to
    // assign
    .AddOption(x => x.Verbosity, ["--verbosity"],
        description: "Output verbosity (Verbose, Normal, Minimal)")
    
    // Adds --help as an option using the default implementation
    .AddHelpSwitch(Task.CompletedTask);

var buildCommand = new SubCommand<BuildModel, Task>("build");
buildCommand
    .AddArgument(x => x.ProjectPath, Arity.One,
        description: "Path to the project file",
        validation: v => v.Exists())
    .AddOption(x => x.Configuration, ["-c", "--configuration"],
        defaultProvider: () => "Debug",
        description: "Configuration name to build")
    .AddOption(x => x.OutputDirectory, ["-o", "--output"],
        defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
        description: "Directory where the built artifacts will be written")
    .AddSwitch(x => x.Force, ["-f", "--force"],
        description: "Forces all dependencies to be resolved")
    .AddSwitch(x => x.NoRestore, ["--no-restore"],
        description: "Don't execute implicit restore")
    .SetHandler(async (model, cancellation) => {
        // Build the project        
    })
    .AddHelpSwitch(Task.CompletedTask);

rootCommand.AddSubCommand(buildCommand);

try
{
    await rootCommand.InvokeAsync(args, CancellationToken.None);
}
catch (CommandLineException exception)
{
    Console.WriteLine(exception.Message);
}
```