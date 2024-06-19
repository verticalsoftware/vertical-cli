# vertical-cli

Minimal, AOT friendly command line arguments parser.

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

// Define a command
var command = new RootCommand<Arguments>("fcopy");
command
    .AddArgument(x => x.Source)
    .AddArgument(x => x.Destination)
    .AddSwitch(x => x.Overwrite, ["-o", "--overwrite"])
    .HandleAsync(async (model, cancellationToken) => 
    {
        if (model.Source.Exists() && !model.Overwrite)
        {
            Console.WriteLine("Desintation file exists and will not be overwritten.");
        }
        await using var source = model.Source.OpenRead();
        await using var dest = model.Destination.OpenWrite();
        await source.CopyToAsync(dest, cancellationToken);
    });

await command.InvokeAsync(arguments);

// Define a model that will receive the arguments
record Arguments(FileInfo Source, FileInfo Destination, bool Overwrite);
```

## Features

- Binds command line arguments to strongly typed models
- Configure positional arguments. options and switches using posix and GNU notation
- Configure contextual parameter validation
- Define a hierarchy of commands each with derived models
- Uses a source generator to bind models removing the need for reflection (AOT friendly)
- Display automatically generated help content