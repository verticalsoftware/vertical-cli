# Commands

## Overview

Commands can be thought of as functions of the application. At a minimum, an application must define one command, but it may also a command hierarchy. Consider the following .NET CLI command:

```shell
> dotnet nuget push [arguments]
```

In the vertical-cli framework:
- `dotnet` is the application
- `nuget` is a sub-command to the `dotnet` application
- `push` is a sub-command to `nuget` command

This hierarchy demonstrates the concept of _abstract_ commands in the sense that `dotnet nuget` by itself will not perform a function, therefore the following concepts present themselves:
- A command can be abstract and have no set handling function. In this case it must define one or more sub-commands to have any use.
- A command can be concrete with a set handling function.
- Commands can have both a handling function and sub commands.

### Hierarchy example

If `dotnet nuget push` was to be minimally modeled, it may look like this:

```csharp
var rootCommand = new RootCommand("dotnet");

var nugetCommand = new SubCommand("nuget");
rootCommand.AddSubCommand(nugetCommand);

var pushCommand = new SubCommand("push");

nugetCommand.AddSubCommand(pushCommand);
nugetCommand.SetHandler(async (
    [GeneratedBinding] INugetPushOptions options,
    CancellationToken token) => 
{
    // TODO: implement the push command        
});

app.ConfigureParser<INugetPushOptions>(builder => 
    {
        // Configure    
    });

var app = new CommandLineApplication(rootCommand);
return await app.ConfigureAndRunAsync(args);

interface INugetPushOptions
{
    // Options
}
```

### Notes about command names

The parser uses the names of each command to form a unique path that is matched to the user's arguments. The exception to this is the root command, where the name is inconsequential to the parser. However note that command names are displayed by the help system in their usage syntax clauses. Therefore it is recommended to name the root command the name of the application or the command name for a tool.