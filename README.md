# vertical-cli

Develop CLI applications using a rich argument binding framework.

## Features

- Parses position arguments and GNU style short and long from options.
- Binds string arguments to strongly typed application option models.
- Performs value conversion, collection creation, and model composition using a source generator.
- Supports application directives and response file annotations.
- Supports hierarchical command structures.
- Provides a customizable help system.
- Offers optional integration with the application's services for command handler resolution.

## Installation

```shell
> dotnet add package vertical-cli --prerelease
```

## Quick start

```csharp
// Create a command with a handler
var command = new RootCommand("hello");
command.SetHandler((IOptions options, CancellationToken cancellationToken) => 
{
    Console.WriteLine($"Hello {options.UserName}!");
    return Task.FromResult(0);
});

// Define the application and the options model
var app = new CommandLineApplication(command);
app.ConfigureModel<IOptions>(builder => builder.ParseArgument(x => x.UserName, required: true));

// 
app.Configure();
return await app.RunAsync(args);

[GeneratedBinding]
interface IOptions
{
    string UserName { get; }
}
```

```shell
> dotnet run -- world

# output
Hello world!
```

## Application Setup

### Overview

For an application to setup the framework, it must perform the following:
- Define the model types command handling functions expect to receive.
- Associate each property of the model types to a position argument or named option or switch symbol.
- Define one or more commands the application will perform and their implementations.
- Pass control to the framework with the application client's arguments.

For a setup walkthrough example, we'll build an application that performs the simple task of compression a file using either the gzip or brotli algorithm.

### Defining models

Command handlers are provided instances of application defined model types that have property values derived from the client's arguments. The library's source generator will convert and bind string arguments to the following types automatically:

- Any type that implements `IParsable<T>`, and any nullable value type that has an `IParsable<T>` type argument (this covers most `System` primitive types suchs as booleans, integers, floats, decimals, and the temporal structs.)
- Strings
- Enums and their nullable variants.
- File system types `FileInfo` and `DirectoryInfo`.
- `System.Uri`

Models are defined by the application using interfaces. This was chosen so that common symbols can be reused throughout command hierarchies and model types can be composed across multiple interface base types. The source generator will create a class that complies with the interface and bind argument values to it.

```csharp
// Define an interface that models the compress commands options
public enum CompressionType
{
    GZip,
    Brotli
}

public interface ICompressOptions
{
    FileInfo InputFile { get; }
    FileInfo OutputFile { get; }
    bool Overwrite { get; }
    CompressionType CompressionType { get; }
}
```

### Defining commands

The example will setup the application's root command and define its handling function. Command handlers accept an instance of the model type and a `CancellationToken`. It asynchronously performs the application's function and returns an integer exit code.

```csharp
var rootCommand = new RootCommand("compress", 
    helpTopic: "Compress a file using the gzip or brotli algorithms.");

rootCommand.SetHandler(async (
    [GeneratedBinding] ICompressOptions options, 
    CancellationToken token) =>
{
   if (options.OutputFile.Exists && !options.Overwrite)
   {
       Console.WriteLine("Output file already exists; use the --overwrite option.");
   } 
   
   await using var inputStream = File.OpenRead(options.InputFile.FullName);
   await using var outputStream = File.OpenWrite(options.OutputFile.FullName);
   
   using Stream compressionStream = options.CompressionType switch 
   {
        CompressionType.GZip => new GZipStream(outputStream, ComrpessionLevel.Compress),
        _ => new BrotliStream(outputStream, CompressionLevel.Compress)
   };
   
   await inputStream.CopyToAsync(compressionStream, token);
   await compressionStream.FlushAsync(token);
   
   Console.WriteLine($"Compressed file {options.InputFile} successfully.");
   return 0;
});
```

> 🗒️ Note
> 
> The `GeneratedBinding` attribute informs the source generator that `ICompressOptions` is a type expected by a command handler. It will generate an implementation and perform value binding to all properties the interface defines, whether directly or indirectly.  

### Associating position arguments and option symbols to the model type

The parser needs to know what property the converted value of each argument should be assigned to. It must be also be configured with the following:
- The _arity_ requirement of the argument or option. This informs the parser whether use of the argument or option is required, or in the case of repeatable symbols, the minimum and maximum number of occurrences are expected and allowed.
- The ordinal index of position arguments, since they are not named. The first argument should have a position of `0`, the second `1`, and so on.
- One or more _alias_ assignments to options and switches. These are the GNU option identifiers such as `-u` or `--user-id`. If an alias isn't defined by the application, the library will create an alias using a lower case kebab format of the property name.

Additionally, the parser can be configured with the following optional data:
- A default value expressed as a function that is used in the event a matching argument isn't found in the client's input.
- One or more data validation rules.
- Content for the help system.

The code example continues by configuring the parser for the `ICompressOptions` type. Here the `CommandLineApplication` instance is introduced.

```csharp
var app = new CommandLineApplication(rootCommand);

app.ConfigureParser<ICompressOptions>(builder => builder
    .ParseArgument(x => x.InputFile,
        ordinalPosition: 0,
        required: true)
    .ParseArgument(x => x.OutputFile,
        ordinalPosition: 1,
        required: true)
    .ParseOption(x => x.CompressionType,
        aliases: ["--compression"],
        defaultProvider: () => CompressionType.GZip)
    .ParseSwitch(x => x.Overwrite)
);
```

### Invoking the source generator and running the application

The minimal example concludes by invoking the configuration of the source generator and running the application. The `Configure()` method is provided by the source generated code and is implemented as an extension method. When called, the generated code will add the following to the configuration:
- Conversion delegates that transform `string` arguments into types expected by the model's properties.
- A file scoped implementation of the model interface type.
- A binding function that creates an instance of the model type using the parse results configured by the application.

```csharp
// Add generated configuration.
app.Configure();

// Run the application, passing in args to be parsed
return await app.RunAsync(args);
```

> 💡Tip
> 
> The source generator also defines a method called `ConfigureAndRunAsync` which combines the `Configure()` and `RunAsync(args)` methods.

## Design & usage cookbook

- [Defining and structuring commands](docs/coommands.md)
- [Using multi valued & variadic symbols](docs/multi-valued-symbols.md)
- [Validating user input](docs/validation.md)
- [Working with the polymorphic model system](docs/models.md)
- [Showing help for a command](docs/help-system.md)
- [Application data & services](docs/services.md)
- [Implement directives for ancillary application control](docs/directives.md)
- [Tapping into framework flow with middleware](docs/middleware.md)
- [Implementing advanced model binding](docs/binding.md)
- [Unit testing the application's configuration](docs/unit-testing.md)