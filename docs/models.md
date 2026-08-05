# Working with the polymorphic model system

## Overview

The framework converts string arguments to strongly typed objects and provides them to command handlers. Model types are defined using interfaces. An application with a single command will likely only define a single model type. However, consider an application with multiple sub commands whose models may share some common options. One example of this would be if the application supports logging. Given it doesn't matter which command is called upon by the user, logging can be considered a _cross-cutting_ concern.

The following example demonstrates the repitition of supporting logging options for each command separately.

```csharp
interface ISubCommand1Options
{
    LogLevel LogLevel { get; }
    FileInfo? LogOutputFile { get; }

    // Specific sub command 1 options
}

interface ISubCommand2Options
{
    LogLevel LogLevel { get; }
    FileInfo? LogOutputFile { get; }
    
    // Specific sub command 2 options
}

interface ISubCommand3Options
{
    LogLevel LogLevel { get; }
    FileInfo? LogOutputFile { get; }
    
    // Specific sub command 3 options
}

// Configure
var app = new CommandLineAppliation(rootCommand);

app.ConfigureParser<ISubCommand1Options>(builder =>
    builder
        .MapOption(
            x => x.LogLevel, 
            ["-log-level"], 
            defaultProvider: () => LogLevel.Information));
        

app.ConfigureParser<ISubCommand2Options>(/* rinse, repeat.. */);

app.ConfigureParser<ISubCommand3Options>(/* rinse, repeat.. */);
```

Interfaces let applications _compose_ option types when needed. Instead of repeating the logging properties and parser configurations across three model types, the application instead could refactor that concern to its own type and have the sub commands inherit from it.

```csharp
interface ILoggingOptions
{
    LogLevel LogLevel { get; }
    FileInfo? LogOutputFile { get; }
}

interface ISubCommand1Options : ILoggingOptions
{
    // Specific sub command 1 options
}

interface ISubCommand2Options : ILoggingOptions
{
    // Specific sub command 2 options
}

interface ISubCommand3Options : ILoggingOptions
{
    // Specific sub command 1 options
}

// Configure
var app = new CommandLineAppliation(rootCommand);

app.ConfigureParser<ILoggingOptions>(builder =>
    builder
        .MapOption(
            x => x.LogLevel, 
            ["-log-level"], 
            defaultProvider: () => LogLevel.Information));

app.ConfigureParser<ISubCommand1Options>(builder =>
    /* configure only specific sub command options */);

app.ConfigureParser<ISubCommand2Options>(builder =>
/* configure only specific sub command options */);

app.ConfigureParser<ISubCommand3Options>(builder =>
    /* configure only specific sub command options */);
```

## Using the source generator

Apply the `GeneratedBinding` attribute to the _final_ command interface type or the command handler's options parameter only. Applying the attribute to super-interfaces will not cause an issue, but useless code will be generated.