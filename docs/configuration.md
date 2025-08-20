# Configuration

Configuring the application requires declaring argument mapping models, defining commands, and configuring the parser. The following examples demonstrates the basic concepts.

## Basic setup

### Defining models

The examples demonstrate configuring a utility that compresses and decompresses files and will use the following models:

```csharp
// Define models
enum CompressionType
{
    GZip, Deflate
}

interface ILogOptions
{
    LogLevel LogLevel { get; }
}

interface IArchivingOptions : ILogOptions
{
    bool Encrypt { get; }
    Secret? Passphrase { get; }
    FileInfo SourcePath { get; }
    FileInfo OutputPath { get; }
    CompressionType Compression { get; }
}
```

### Creating commands

Commands are created by instantiating a root command and any sub commands. Abstract commands use the `RootCommand` and `Command` classes, while invokable commands use the generic `RootCommand<TModel>` and `Command<TModel>` classes.

Commands have a name, a help tag (discussed later), and a handler function if it is invokable. A handler function is delegate that accepts the model instance and a cancellation token and returns the appropriate integer exit code.

```csharp
// Create commands
var rootCommand = new RootCommand(name: "archive");

// Define he 'archive create' command
rootCommand.AddCommand(new Command<IArchivingOptions>(
    name: "create",
    handler: async (
        [GeneratedBinding] IArchivingOptions options,
        CancellationToken cancellationToken) => 
        {
            // TODO: Implement application functionality
            Console.WriteLine($"Compressing {options.SourcePath}...");
            await Task.CompletedTask;
            return 0;
        }));

// Define the 'archive extract' command
rootCommand.AddCommand(new Command<IArchivingOptions>(
    name: "extract",
    handler: async (
        [GeneratedBinding] IArchivingOptions options,
        CancellationToken cancellationToken) => 
        {
            // TODO: Implement application functionality
            Console.WriteLine($"Extracing {options.SourcePath}...");
            await Task.CompletedTask;
            return 0;
        }));    
```

### Configuring the parser

The parser is configured using the `CommandLineBuilder` object. This object is constructed with an instance of the application's root command. The `ConfigureModel` method defines the symbols for a specific model type. Scalar value symbols are added using the `AddArgument`, `AddOption`, and the `AddSwitch` methods. multi valued symbols are added using the `AddCollectionArgument` and `AddCollectionOption` methods (there's no use case for a multi-valued switch). 

Notice the model configurations are decoupled from the command objects. This makes a particular model's symbol definitions global throughout the application and available to all commands that have a model type that implements or derives them as base types.

```csharp
// Configure the parser
var builder = new CommandLineBuilder(rootCommand)
    .ConfigureModel<ILogOptions>(model => model
        .AddOption(
            propertyExpression: x => x.LogLevel,
            aliases: ["--log-level"]))
    .ConfigureModel<IArchivingOptions>(model => model
        .AddSwitch(
            propertyExpression: x => x.Encrypt,
            aliases: ["-e", "--encrypt"])
        .AddOption(
            propertyExpression: x => x.Secret,
            aliases: ["--secret"])
        .AddOption(
            propertyExpression: x => x.Compression,
            aliases: ["-c", "--compression"])
        .AddArgument(
            propertyExpression: x => x.SourcePath,
            precedence: 0,
            name: "SOURCE")
         .AddArgument(
            propertyExpression: x => x.DestPath,
            precedence: 1,
            name: "DEST"));
```

### Invoking the framework

Lastly, control is passed to the framework. If the application leverages the source generator (by using the `[GeneratedBinding]` attribute), then an extension method is also generated on the `CommandLineBuilder` type. Pass control to the framework by invoking the `BuildAndRunAsync` method.

```csharp
// Program.cs

return await builder.BuildAndRunAsync(args);
```