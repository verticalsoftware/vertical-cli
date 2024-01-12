# Vertical.Cli

## Overview

Use this library to parse application command line arguments, bind the values to a type, and pass control to a handler that implements the application's functionality.

## Configuration

### Commands

Console applications can perform a single function or multiple functions. These functions are represented in the library as commands. A Vertical.Cli application requires a root command, but application's can also be defined that have a hierarchy of sub commands. An example of such a hierarchy is `dotnet nuget push`. While `dotnet` is the utility's executable program name, `nuget` is a sub command, and `push` is sub command of `nuget`.
The following example demonstrates both scenarios. The generic argument indicates the commands will return an `int` value.

```csharp
// Single command application
var rootCommand = RootCommand.Create<int>(
    id: "<prgram>",
    root => 
    {
        // Configure the root command
    }
);

// Application with sub commands
var rootCommand = RootCommand.Create<int>(
    id: "dotnet",
    root =>
    {
        // Configure the root command...

        // Configure sub commands:
        root.ConfigureSubCommand(
            id: "build",
            cmd =>
            {
                // Configure build command...
            });


        root.ConfigureSubCommand(
            id: "restore",
            cmd =>
            {
                // Configure restore command...
            });
    }
);
```

### Defining arguments and options

Arguments, options, and switches are defined using the configuration delegate and the methods available in the fluent builder object. Use the `AddOption<T>`, `AddArgument<T>`, and `AddSwitch` methods. The `<T>` generic argument dictates the value type of the option or argument. The code example shows a verbose example of configuring an option:

```csharp
    // Command configuration delegate
    cmd =>
    {
        cmd.AddOption<FileInfo>(            
            id: "--path",
            aliases: new[]{ "-p" },
            arity: Arity.ZeroOrOne,
            description: "The path to the source file.",
            scope: SymbolScope.Self,
            defaultProvider = () => new FileInfo("/var/opt/log.txt"),
            validator = Validator.Create<FileInfo>(x => x.FileExists())
        );
    }
```

Parameter descriptions:
 
|Parameter|Description|
|---|---|
|id|Required. The primary symbol for the option or argument. This also servers as the primary identity for model binding.|
|aliases|Other symbols the option can be referred to on the CLI (options & switches only).|
|arity|Describes the minimum and maximum number of uses of the symbol on the CLI. Arguments have a default arity of `One` while options and switches have a default arity of `ZeroOrOne`.|
|description|An description of the option that is displayed by the default help formatter.|
|scope|Determines the visibility of the symbol in the sub-command hierachy, and is one of `Parent`, `ParentAndDescendents` or `Descendents`.|
|defaultProvider|A function that provides a default value when one is not otherwise provided.|
|validator|A `Validator` object, that determines the validity of the provided argument.|

> 🗈 Note
>
> Any identities assigned to options and arguments must be unique in the command path.

### Binding to models

Arguments received on the CLI that are semantically matched with a command's symbols must be mapped to a model. The easiest way this can be implemented is to define a `record` with properties whose names align to each symbol's identity. At runtime, the library will create a new instance of the model and populate it with values. Binding is automatically performed to constructor parameters, public properties with a set accessor, and publc non read-only fields. The model type is then specified as a generic argument to the configuration method. The handler receives the model type and performs the command's function.

```csharp
// Model:
record FileCopyParams(FileInfo Source, FileInfo Dest, bool Overwrite);

// Define the command
var rootCommand = new RootCommand.Create<FileCopyParams, Task>(
    id: "cp",
    cmd =>
    {
        // Define options
        cmd.AddArgument<FileInfo>("source")
           .AddArgument<FileInfo>("dest")
           .AddSwitch("--overwrite", new[]{ "-o" });

        // Implement the command's function
        cmd.SetHandler(async (copyParams, cancellationToken) =>
        {
            if (copyParams.Source.Exists)
            {
                Console.WriteLine("Existing file will not be overwritten.");
                return;
            }

            await using var sourceStream = File.OpenRead(copyParams.Source.FullName);
            await using var destStream = File.OpenWrite(copyParams.Dest.FullName);

            await sourceStream.CopyToAsync(destStream, cancellationToken);
        });          
    }
);    
```

The matching behavior to symbols can be explicitly controlled by using the `[BindTo]]` attribute. Apply the attribute to each model member that requires it.

### Value conversion

CLI arguments must be converted to the type expected by the symbol. Out-of-box, the library converts the following types automatically:

- All primitive `System` structs (except for `IntPtr`) and their `Nullable<T>` counterparts.
- `char` and `string`
- Temporal types: `DateTime`, `DateTimeOffset`, `TimeSpan`, `TimeOnly`, `DateOnly`
- Misc types: `FileInfo`, `DirectoryInfo`, `Uri` and `Guid`
- Arrays and any non associative `System.Collections.Generic` collection type.

An application can configure conversion for an unsupported type using one of the following approaches:
- Register a conversion function
- Register a `ValueConverter<T>` implementation
- Make the type `IParsable<TSelf>`

Examples:
```csharp
readonly record struct Point(int X, int Y);

var options = new CliOptions();

// option 1: Register a conversion function
options.AddConverter(value =>
{
    var split = value.Split(',');
    return new Point(int.Parse(split[0], split[1]));
});

// option 2: Define a value converter
public sealed class PointConverter : ValueConverter<Point>
{
    public override Convert(ConversionContext<Point> context)
    {
        var split = context.Value.Split(',');
        return new Point(int.Parse(split[0], split[1]));
    }
}

options.AddConverter(new PointConverter());

// option 3: If the type implements IParsable<TSelf>
options.AddConverter<Point>();

// Options can be specified when creating the root command
var rootCommand = RootCommand.Create<int>(
    id: "program",
    configure: root => {},
    options: options);
```

### Validation

CLI arguments can be validated prior to binding them to a model. If validation fails, a message is written to the console. Validation is performed by the using implementations of the abstract `Validator<T>` type. This example shows extending the class to validate `TimeSpan` values:

```csharp
// Application defined validator
public sealed class TimeoutValidator : Validator<TimeSpan>
{
    public override void Validate(ValidationContext<TimeSpan> context)
    {
        if (context.Value.TotalSeconds <= 60)
            return;

        context.AddError("Timeout must be 60 seconds or less.");
    }
}
```

Validator instances can be used in several places:

```csharp
var timeoutValidator = new TimeoutValidator();

// Use validator with specific symbols
var rootCommand = new RootCommand<int>(
    id: "program",
    configure: root =>
    {
        // Values provided for this symbol will be validated (specific approach)
        root.AddOption<TimeSpan>("--connect-timeout", validator: timeoutValidator);        
    });

// All TimeSpan symbol values validated (global approach)
var options = new CliOptions();
options.AddValidator(timeoutValidator);
```

> 🗈 Note
>
> A validator configured for a symbol will be chosen over an instance of the same type defined globally.

The library also has methods defined on the `ValidationBuilder<T>` type to implement common validation patterns. For example, the following example demonstrates how to avoid writing validator classes in most cases:

```csharp
// Add a global TimeSpan validator
options.AddValidator<TimeSpan>(
    x => x.LessThanOrEqualTo(TimeSpan.FromSeconds(60)), 
    () => "Timeout must be 60 seconds or less.");

// Alternatively, create using configuration methods.
var timeoutValidator = Validator.Configure<TimeSpan>(
    x => x.LessThanOrEqualTo(TimeSpan.FromSeconds(60)), 
    () => "Timeout must be 60 seconds or less.");
```

### Implementing command handlers

The handler of a command is a delegate that receives the bound model object and returns the result value. Handlers are required except when a command delegates all control to sub commands. The handler type is determined by the type of model arguments are bound to and the result type.

> 🗈 Note
>
> Commands in the hierarchy each can have different binding models, but all handlers must return the same type.

Call `SetHandler` on the configuration builder for each command.

```csharp
var rootCommand = RootCommand.Create<ProgramArgs, int>(
    id: "program",
    configure: root =>
    {
        // Configure options...
        root.SetHandler(args => 
        {
            Console.WriteLine("No command specified");
            return 0;
        })

        root.configureSubCommand<PaintArgs, int>(
            id: "paint",
            configure: cmd =>
            {
                // Configure options...

                cmd.SetHandler(args => 
                {
                    Console.WriteLine("I guess we're painting.");
                    return 0;                            
                });
            }
        );
    });
```

### Displaying help

The library has built-in functionality to display help to the user. The default formatter displays a command grammar statement and the symbol list. This experience can be improved by specifying descriptions for commands and options. The following example demonstrates enabling the help system. when the user starts the application and specifies the `--help` option, help content will be displayed.

```csharp
record Model(double[] Values);

var rootCommand = RootCommand.Create<int>(
    id: "program",
    configure: root =>
    {
        root.AddDescription("Finds the lowest value of a set of numbers.");

        root.AddArgument<double[]>(
            id: "values", 
            arity: new Arity(minCount: 2, maxCount: 16),
            description: "The number whose square root is to be found.");

        // Enable the help system, the default symbol is --help, but a different id
        // and aliases can be used.
        root.AddHelpOption();

        root.SetHandler(param => 
        {
            Console.WriteLine(param.Values.Min());
            return 0;
        });
    });
```


### Testing the configuration

A root command object can be checked for configuration errors by using the `ConfigurationValidator` class as followings:

```csharp
// Get the errors in the configuration
IReadOnlyCollection<string> errors = rootCommand.GetErrors();

// Throws an exception if errors are found (great for unit tests)
rootCommand.ThrowIfInvalid();
```

> 🗈 Note
>
> The generated source code will automatically call the `ThrowIfInvalid` method when the build is in the `DEBUG` configuration. In the `RELEASE` configuration, this method is removed from the `Invoke` and `InvokeAsync` method bodies. This behavior can be controlled further by defining `ENABLE_CLI_VALIDATION` or `DISABLE_CLI_VALIDATION symbols`.

Validation ensures the following:
- Options object does not have ambigous value converters, validators, and model binders.
- All identifiers comply with naming rules
- Symbol identifiers are all unique with each command path
- Symbol value types can be converted from `string`
- Binding models have a single, public constructor
- All binding model constructor parameters, properties, and fields are matched with an option or argument
- Binding model types are compatible with the symbol value type
- Handlers are set on the required commands

## Running the application

### Using the root command

After a root command is defined, program control can be delegated to the library by calling `Invoke` or `InvokeAsync` on the object and passing along the application's `args`. The string arguments will be parsed and bound to the model type and the appropriate command handler will be invoked.

```csharp
var rootCommand = RootCommand.Create<CliArgs, int>(
    id: "program",
    root =>
    {
        // Configuration
    });


rootCommand.Invoke(args);
```

