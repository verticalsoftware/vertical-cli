﻿# Vertical.Cli

A command line arguments parser.

## Features

- Define a single command or hierarchy of commands.
- Binds parsed arguments to the public constructors, properties and fields of classes and records.
- Suports POSIX, GNU, and Microsoft option syntax.
- Built-in, configurable help system.
- Defines validation methods for verbose unit testing of configuration.
- Uses a source generator for efficient runtime execution and AOT trimming.

## Quick look

```
> dotnet add package vertical-cli
```

```csharp

// Define a type to receive parameters
record Parameters(FileInfo Source, FileInfo Dest, bool Overwrite);

// Define a root command
var rootCommand = RootCommand.Create<Parameters, Task>(
    id: "copy",
    root =>
    {
        // Configure options
        root.AddArgument(
            id: "source",
            validator: Validator.Configure<FileInfo>(x => x.FileExists()),
            description: "Path to the source file to copy")

            .AddArgument<FileInfo>(
                id: "dest",
                description: "Path to the destination file")
                
            .AddSwitch(
                id: "--overwrite", 
                description: "Whether to overwrite existing files");

        // Enables the "--help" option
        root.AddHelpOption();                

        root.AddDescription("Copies files");                

        // Receives the arguments and implements the application's logic
        root.SetHandler(async (Parameters param) => 
        {
            if (param.Dest.Exists && !param.Overwrite)
            {
                Console.WriteLine("Skipping operation (destination file exists).");
                return;
            }

            await File.Copy(param.source.FullName, param.dest.FullName);
        });            
    }
);

// Parse arguments & pass control to the handler delegate
await rootCommand.InvokeAsync(args);

``` 

Read the [full docs](https://github.com/verticalsoftware/vertical-cli/blob/main/docs/intro.md).
